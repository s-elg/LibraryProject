using LibraryProject.Application.DTOs;
using LibraryProject.Application.Exceptions;
using LibraryProject.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LibraryProject.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PenaltyController : ControllerBase
{
    private readonly IPenaltyService _penaltyService;

    public PenaltyController(IPenaltyService penaltyService)
    {
        _penaltyService = penaltyService;
    }

    private Guid GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
        return Guid.Parse(userIdClaim);
    }

    // Manuel ceza oluşturma - Admin-only
    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> CreateManualPenalty([FromBody] CreatePenaltyDto dto)
    {
        var result = await _penaltyService.CreateManualPenaltyAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    // Cezayı kaldırma/tamamlama - Admin-only
    [Authorize(Roles = "Admin")]
    [HttpPut("{id:guid}/remove")]
    public async Task<IActionResult> RemovePenalty(Guid id)
    {
        var result = await _penaltyService.RemovePenaltyAsync(id);
        return Ok(result);
    }

    // Tüm aktif cezalar - Admin-only
    [Authorize(Roles = "Admin")]
    [HttpGet("active")]
    public async Task<IActionResult> GetActivePenalties()
    {
        var result = await _penaltyService.GetActivePenaltiesAsync();
        return Ok(result);
    }

    // Giriş yapmış kullanıcının kendi cezaları
    [Authorize]
    [HttpGet("my-penalties")]
    public async Task<IActionResult> GetMyPenalties()
    {
        var userId = GetCurrentUserId();
        var result = await _penaltyService.GetByUserAsync(userId);
        return Ok(result);
    }

    // Belirli bir kullanıcının cezaları - Admin-only
    [Authorize(Roles = "Admin")]
    [HttpGet("user/{userId:guid}")]
    public async Task<IActionResult> GetByUser(Guid userId)
    {
        var result = await _penaltyService.GetByUserAsync(userId);
        return Ok(result);
    }

    // Tek bir cezanın detayı - sahibi veya Admin
    [Authorize]
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var currentUserId = GetCurrentUserId();
        var isAdmin = User.IsInRole("Admin");
        var result = await _penaltyService.GetByIdAsync(id, currentUserId, isAdmin);
        return Ok(result);
    }
}