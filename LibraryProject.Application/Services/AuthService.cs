using LibraryProject.Application.DTOs.Auth;
using LibraryProject.Application.Exceptions;
using LibraryProject.Application.Interfaces;
using LibraryProject.Application.Interfaces.Services;
using LibraryProject.Application.Settings;
using LibraryProject.Domain.Entities;
using Microsoft.Extensions.Options;

namespace LibraryProject.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITokenService _tokenService;
    private readonly JwtSettings _jwtSettings;

    public AuthService(IUnitOfWork unitOfWork, ITokenService tokenService, IOptions<JwtSettings> jwtOptions)
    {
        _unitOfWork = unitOfWork;
        _tokenService = tokenService;
        _jwtSettings = jwtOptions.Value;
    }
    public async Task LogoutAsync(Guid userId)
    {
        await _unitOfWork.RefreshTokens.RevokeAllUserTokensAsync(userId);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<AuthResponseDto> RegisterAsync(RegisterRequestDto request)
    {
        if (!await _unitOfWork.Users.IsEmailUniqueAsync(request.Email))
            throw new EmailAlreadyExistsException(request.Email);

        var user = new User
        {
            FullName = request.FullName,
            Email = request.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            Role = UserRole.Member
        };

        await _unitOfWork.Users.AddAsync(user);
        await _unitOfWork.SaveChangesAsync();

        return await GenerateAuthResponseAsync(user);
    }

    public async Task<AuthResponseDto> LoginAsync(LoginRequestDto request)
    {
        var user = await _unitOfWork.Users.GetByEmailAsync(request.Email);

        if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            throw new InvalidCredentialsException();

        return await GenerateAuthResponseAsync(user);
    }

    public async Task<AuthResponseDto> RefreshTokenAsync(RefreshTokenRequestDto request)
    {
        var userId = _tokenService.GetUserIdFromExpiredToken(request.AccessToken);

        var storedToken = await _unitOfWork.RefreshTokens.GetByTokenAsync(request.RefreshToken);

        if (storedToken == null || storedToken.UserId != userId)
            throw new InvalidRefreshTokenException();

        if (storedToken.IsRevoked)
        {
            await _unitOfWork.RefreshTokens.RevokeAllUserTokensAsync(userId);
            await _unitOfWork.SaveChangesAsync();
            throw new InvalidRefreshTokenException();
        }

        if (!storedToken.IsActive)
            throw new InvalidRefreshTokenException();

        var user = await _unitOfWork.Users.GetByIdAsync(userId);
        if (user == null)
            throw new InvalidRefreshTokenException();

        var newRefreshToken = _tokenService.GenerateRefreshToken();
        storedToken.RevokedAt = DateTime.UtcNow;

        var newRefreshTokenEntity = new RefreshToken
        {
            UserId = user.Id,
            Token = newRefreshToken,
            ExpiresAt = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpirationDays)
        };

        await _unitOfWork.RefreshTokens.AddAsync(newRefreshTokenEntity);
        storedToken.ReplacedByTokenId = newRefreshTokenEntity.Id;

        await _unitOfWork.SaveChangesAsync();

        var newAccessToken = _tokenService.GenerateAccessToken(user);

        return new AuthResponseDto(newAccessToken, newRefreshToken, user.Email, user.FullName, user.Role);
    }

    private async Task<AuthResponseDto> GenerateAuthResponseAsync(User user)
    {
        var accessToken = _tokenService.GenerateAccessToken(user);
        var refreshToken = _tokenService.GenerateRefreshToken();

        var refreshTokenEntity = new RefreshToken
        {
            UserId = user.Id,
            Token = refreshToken,
            ExpiresAt = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpirationDays)
        };

        await _unitOfWork.RefreshTokens.AddAsync(refreshTokenEntity);
        await _unitOfWork.SaveChangesAsync();

        return new AuthResponseDto(accessToken, refreshToken, user.Email, user.FullName, user.Role);
    }
}