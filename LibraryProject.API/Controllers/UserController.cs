using LibraryProject.Application.Common;
using LibraryProject.Application.DTOs;
using LibraryProject.Application.Exceptions;
using LibraryProject.Application.Interfaces.Services;
using LibraryProject.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryProject.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    public async Task<ActionResult<PagedResult<UserListItemDto>>> GetUsers(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? searchTerm = null,
        [FromQuery] UserRole? role = null)
    {
        var result = await _userService.GetUsersAsync(pageNumber, pageSize, searchTerm, role);
        return Ok(result);
    }

    [HttpPut("{id}/role")]
    public async Task<ActionResult<UserListItemDto>> UpdateUserRole(Guid id, UpdateUserRoleDto request)
    {
        try
        {
            var result = await _userService.UpdateUserRoleAsync(id, request);
            return Ok(result);
        }
        catch (UserNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }
}