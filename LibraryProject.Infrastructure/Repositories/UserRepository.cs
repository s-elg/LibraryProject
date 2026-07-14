using LibraryProject.Application.Interfaces.Repositories;
using LibraryProject.Domain.Entities;
using LibraryProject.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LibraryProject.Infrastructure.Repositories;

public class UserRepository : GenericRepository<User>, IUserRepository
{
    public UserRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        var normalizedEmail = email.ToLower();

        return await _dbSet
            .FirstOrDefaultAsync(u => u.Email.ToLower() == normalizedEmail);
    }

    public async Task<bool> IsEmailUniqueAsync(string email)
    {
        var normalizedEmail = email.ToLower();

        return !await _dbSet
            .AnyAsync(u => u.Email.ToLower() == normalizedEmail);
    }

    public async Task<IEnumerable<User>> GetByRoleAsync(UserRole role)
    {
        return await _dbSet
            .Where(u => u.Role == role)
            .ToListAsync();
    }

    public async Task<IEnumerable<User>> GetSuspendedUsersAsync()
    {
        var now = DateTime.UtcNow;

        return await _dbSet
            .Where(u => u.Penalties.Any(p =>
                p.Status == PenaltyStatus.Active &&
                p.SuspensionEndDate > now))
            .ToListAsync();
    }
}