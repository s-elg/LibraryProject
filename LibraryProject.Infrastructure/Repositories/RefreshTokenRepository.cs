using LibraryProject.Application.Interfaces.Repositories;
using LibraryProject.Domain.Entities;
using LibraryProject.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LibraryProject.Infrastructure.Repositories;

public class RefreshTokenRepository : GenericRepository<RefreshToken>, IRefreshTokenRepository
{
    public RefreshTokenRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<RefreshToken?> GetByTokenAsync(string token)
    {
        return await _dbSet
            .FirstOrDefaultAsync(rt => rt.Token == token);
    }

    public async Task<IEnumerable<RefreshToken>> GetActiveTokensByUserAsync(Guid userId)
    {
        var now = DateTime.UtcNow;

        return await _dbSet
            .Where(rt => rt.UserId == userId &&
                         rt.RevokedAt == null &&
                         rt.ExpiresAt > now)
            .ToListAsync();
    }

    public async Task RevokeAllUserTokensAsync(Guid userId)
    {
        var activeTokens = await GetActiveTokensByUserAsync(userId);

        foreach (var token in activeTokens)
        {
            token.RevokedAt = DateTime.UtcNow;
        }
    }
}