using LibraryProject.Application.Common;
using LibraryProject.Application.DTOs;
using LibraryProject.Application.Exceptions;
using LibraryProject.Application.Interfaces;
using LibraryProject.Application.Interfaces.Services;
using LibraryProject.Domain.Entities;

namespace LibraryProject.Application.Services;

public class UserService : IUserService
{
    private readonly IUnitOfWork _unitOfWork;

    public UserService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<PagedResult<UserListItemDto>> GetUsersAsync(int pageNumber, int pageSize, string? searchTerm, UserRole? role)
    {
        var paged = await _unitOfWork.Users.GetPagedAsync(pageNumber, pageSize, searchTerm, role);
        var now = DateTime.UtcNow;

        var items = paged.Items.Select(u => new UserListItemDto(
            u.Id,
            u.FullName,
            u.Email,
            u.Role,
            u.CreatedDate,
            u.Penalties.Any(p => p.Status == PenaltyStatus.Active && p.SuspensionEndDate > now)
        )).ToList();

        return new PagedResult<UserListItemDto>
        {
            Items = items,
            TotalCount = paged.TotalCount,
            PageNumber = paged.PageNumber,
            PageSize = paged.PageSize
        };
    }

    public async Task<UserListItemDto> UpdateUserRoleAsync(Guid userId, UpdateUserRoleDto request)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(userId);
        if (user is null)
            throw new UserNotFoundException(userId);

        user.Role = request.Role;
        _unitOfWork.Users.Update(user);
        await _unitOfWork.SaveChangesAsync();

        return new UserListItemDto(
            user.Id,
            user.FullName,
            user.Email,
            user.Role,
            user.CreatedDate,
            IsSuspended: false // Not: GetByIdAsync Penalties'i include etmiyor, bkz. aşağıdaki not
        );
    }
}