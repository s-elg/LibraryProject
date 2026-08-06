using LibraryProject.Application.DTOs;
using LibraryProject.Application.Exceptions;
using LibraryProject.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace LibraryProject.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<ActionResult<AuthResponseDto>> Register(RegisterRequestDto request)
    {
        try
        {
            var result = await _authService.RegisterAsync(request);
            return Ok(result);
        }
        catch (EmailAlreadyExistsException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponseDto>> Login(LoginRequestDto request)
    {
        try
        {
            var result = await _authService.LoginAsync(request);
            return Ok(result);
        }
        catch (InvalidCredentialsException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
    }

    [HttpPost("refresh-token")]
    public async Task<ActionResult<AuthResponseDto>> RefreshToken(RefreshTokenRequestDto request)
    {
        try
        {
            var result = await _authService.RefreshTokenAsync(request);
            return Ok(result);
        }
        catch (InvalidRefreshTokenException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
    }

    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        var idClaim = User.FindFirst(ClaimTypes.NameIdentifier) ?? User.FindFirst(JwtRegisteredClaimNames.Sub);
        if (idClaim is null) return Unauthorized();

        if (!Guid.TryParse(idClaim.Value, out var userId)) 
            return BadRequest(new { message = "Invalid user id in token." });

        await _authService.LogoutAsync(userId);
        return NoContent();
    }

    [Authorize]
    [HttpGet("me")]
    public async Task<ActionResult<UserProfileDto>> GetProfile()
    {
        var userId = GetUserId();
        if (userId is null) return Unauthorized();

        try
        {
            var result = await _authService.GetProfileAsync(userId.Value);
            return Ok(result);
        }
        catch (UserNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [Authorize]
    [HttpPut("profile")]
    public async Task<ActionResult<UserProfileDto>> UpdateProfile(UpdateProfileRequestDto request)
    {
        var userId = GetUserId();
        if (userId is null) return Unauthorized();

        try
        {
            var result = await _authService.UpdateProfileAsync(userId.Value, request);
            return Ok(result);
        }
        catch (EmailAlreadyExistsException ex)
        {
            return Conflict(new { message = ex.Message });
        }
        catch (UserNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [Authorize]
    [HttpPut("change-password")]
    public async Task<IActionResult> ChangePassword(ChangePasswordRequestDto request)
    {
        var userId = GetUserId();
        if (userId is null) return Unauthorized();

        try
        {
            await _authService.ChangePasswordAsync(userId.Value, request);
            return NoContent();
        }
        catch (WrongCurrentPasswordException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
        catch (UserNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    private Guid? GetUserId()
    {
        var idClaim = User.FindFirst(ClaimTypes.NameIdentifier) ?? User.FindFirst(JwtRegisteredClaimNames.Sub);
        if (idClaim is null || !Guid.TryParse(idClaim.Value, out var userId))
            return null;

        return userId;
    }
}