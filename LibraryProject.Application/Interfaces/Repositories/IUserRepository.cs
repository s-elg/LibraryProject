using LibraryProject.Application.Common;
using LibraryProject.Domain.Entities;

namespace LibraryProject.Application.Interfaces.Repositories;

public interface IUserRepository : IGenericRepository<User>
{
    Task<User?> GetByEmailAsync(string email);
    Task<bool> IsEmailUniqueAsync(string email);
    Task<IEnumerable<User>> GetByRoleAsync(UserRole role);
    Task<IEnumerable<User>> GetSuspendedUsersAsync();

    Task<PagedResult<User>> GetPagedAsync(int pageNumber, int pageSize, string? searchTerm, UserRole? role);
}