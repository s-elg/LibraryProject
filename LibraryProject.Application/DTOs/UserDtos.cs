using LibraryProject.Domain.Entities;

namespace LibraryProject.Application.DTOs;

public record UserListItemDto(
    Guid Id,
    string FullName,
    string Email,
    UserRole Role,
    DateTime CreatedDate,
    bool IsSuspended);

public record UpdateUserRoleDto(UserRole Role);