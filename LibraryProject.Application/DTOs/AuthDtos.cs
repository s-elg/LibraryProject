using LibraryProject.Domain.Entities;

namespace LibraryProject.Application.DTOs;

public record RegisterRequestDto(string FullName, string Email, string Password);

public record LoginRequestDto(string Email, string Password);

public record RefreshTokenRequestDto(string AccessToken, string RefreshToken);

public record AuthResponseDto(string AccessToken, string RefreshToken, string Email, string FullName, UserRole Role);

public record UserProfileDto(string FullName, string Email, UserRole Role);

public record UpdateProfileRequestDto(string FullName, string Email);

public record ChangePasswordRequestDto(string CurrentPassword, string NewPassword);