using LibraryProject.Domain.Entities;

namespace LibraryProject.Application.DTOs.Auth;

public record RegisterRequestDto(string FullName, string Email, string Password);

public record LoginRequestDto(string Email, string Password);

public record RefreshTokenRequestDto(string AccessToken, string RefreshToken);

public record AuthResponseDto(string AccessToken, string RefreshToken, string Email, string FullName, UserRole Role);