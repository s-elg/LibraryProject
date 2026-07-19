using LibraryProject.Application.DTOs;
using LibraryProject.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LibraryProject.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReviewController : ControllerBase
{
    private readonly IReviewService _reviewService;

    public ReviewController(IReviewService reviewService)
    {
        _reviewService = reviewService;
    }

    private Guid GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
        return Guid.Parse(userIdClaim);
    }

    // Kitap ödünç almış her üye yorum yazabilir (kontrol ReviewService içinde yapılıyor)
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> CreateReview([FromBody] CreateReviewDto dto)
    {
        var userId = GetCurrentUserId();
        var result = await _reviewService.CreateReviewAsync(userId, dto);
        return CreatedAtAction(nameof(GetByBook), new { bookId = result.BookId }, result);
    }

    // Kullanıcı sadece kendi yorumunu güncelleyebilir (kontrol service içinde)
    [Authorize]
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateReview(Guid id, [FromBody] UpdateReviewDto dto)
    {
        var userId = GetCurrentUserId();
        var result = await _reviewService.UpdateReviewAsync(id, userId, dto);
        return Ok(result);
    }

    // Kullanıcı kendi yorumunu, Admin herhangi bir yorumu silebilir
    [Authorize]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteReview(Guid id)
    {
        var userId = GetCurrentUserId();
        var isAdmin = User.IsInRole("Admin");
        await _reviewService.DeleteReviewAsync(id, userId, isAdmin);
        return NoContent();
    }

    // Bir kitabın tüm yorumları - herkese açık
    [AllowAnonymous]
    [HttpGet("book/{bookId:guid}")]
    public async Task<IActionResult> GetByBook(Guid bookId)
    {
        var result = await _reviewService.GetByBookAsync(bookId);
        return Ok(result);
    }

    // Bir kitabın ortalama puanı - herkese açık
    [AllowAnonymous]
    [HttpGet("book/{bookId:guid}/average-rating")]
    public async Task<IActionResult> GetAverageRating(Guid bookId)
    {
        var result = await _reviewService.GetAverageRatingAsync(bookId);
        return Ok(new { BookId = bookId, AverageRating = result });
    }

    // Giriş yapmış kullanıcının kendi yorumları
    [Authorize]
    [HttpGet("my-reviews")]
    public async Task<IActionResult> GetMyReviews()
    {
        var userId = GetCurrentUserId();
        var result = await _reviewService.GetByUserAsync(userId);
        return Ok(result);
    }

    // Belirli bir kullanıcının yorumları - Admin-only
    [Authorize(Roles = "Admin")]
    [HttpGet("user/{userId:guid}")]
    public async Task<IActionResult> GetByUser(Guid userId)
    {
        var result = await _reviewService.GetByUserAsync(userId);
        return Ok(result);
    }
}