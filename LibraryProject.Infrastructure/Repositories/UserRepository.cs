using LibraryProject.Application.Common;
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

    public async Task<PagedResult<User>> GetPagedAsync(int pageNumber, int pageSize, string? searchTerm, UserRole? role)
    {
        // Penalties'i Include ediyoruz çünkü IsSuspended hesaplaması service katmanında buna ihtiyaç duyacak
        var query = _dbSet.Include(u => u.Penalties).AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = searchTerm.ToLower();
            query = query.Where(u =>
                u.FullName.ToLower().Contains(term) ||
                u.Email.ToLower().Contains(term));
        }

        if (role.HasValue)
        {
            query = query.Where(u => u.Role == role.Value);
        }

        var totalCount = await query.CountAsync();

        var users = await query
            .OrderBy(u => u.FullName)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResult<User>
        {
            Items = users,
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }
}