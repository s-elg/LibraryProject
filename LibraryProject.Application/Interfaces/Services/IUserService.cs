using LibraryProject.Application.Common;
using LibraryProject.Application.DTOs;
using LibraryProject.Domain.Entities;

namespace LibraryProject.Application.Interfaces.Services;

public interface IUserService
{
    Task<PagedResult<UserListItemDto>> GetUsersAsync(int pageNumber, int pageSize, string? searchTerm, UserRole? role);
    Task<UserListItemDto> UpdateUserRoleAsync(Guid userId, UpdateUserRoleDto request);
}