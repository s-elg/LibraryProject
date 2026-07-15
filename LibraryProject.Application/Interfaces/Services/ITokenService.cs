using LibraryProject.Domain.Entities;

namespace LibraryProject.Application.Interfaces;

public interface ITokenService
{
    string GenerateAccessToken(User user);
    string GenerateRefreshToken();
    Guid GetUserIdFromExpiredToken(string token);
}