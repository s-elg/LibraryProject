using LibraryProject.Domain.Entities;

namespace LibraryProject.Application.Interfaces.Repositories;

public interface IRefreshTokenRepository : IGenericRepository<RefreshToken>
{
    Task<RefreshToken?> GetByTokenAsync(string token);
    Task<IEnumerable<RefreshToken>> GetActiveTokensByUserAsync(Guid userId);
    Task RevokeAllUserTokensAsync(Guid userId);
}