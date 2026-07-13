using LibraryProject.Application.Interfaces;
using LibraryProject.Domain.Entities;
using LibraryProject.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LibraryProject.Infrastructure.Repositories;

public class PenaltyRepository : GenericRepository<Penalty>, IPenaltyRepository
{
    public PenaltyRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<bool> HasActivePenaltyAsync(Guid userId)
    {
        var now = DateTime.UtcNow;

        return await _dbSet
            .AnyAsync(p => p.UserId == userId &&
                           p.Status == PenaltyStatus.Active &&
                           p.SuspensionEndDate > now);
    }

    public async Task<IEnumerable<Penalty>> GetByUserAsync(Guid userId)
    {
        return await _dbSet
            .Where(p => p.UserId == userId)
            .OrderByDescending(p => p.CreatedDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<Penalty>> GetActivePenaltiesAsync()
    {
        return await _dbSet
            .Where(p => p.Status == PenaltyStatus.Active)
            .ToListAsync();
    }

    public async Task<IEnumerable<Penalty>> GetExpiredButActivePenaltiesAsync()
    {
        var now = DateTime.UtcNow;

        return await _dbSet
            .Where(p => p.Status == PenaltyStatus.Active && p.SuspensionEndDate <= now)
            .ToListAsync();
    }
}