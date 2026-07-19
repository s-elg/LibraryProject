using System.Security.Claims;
using LibraryProject.Application.DTOs;
using LibraryProject.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryProject.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class LoanController : ControllerBase
{
    private readonly ILoanService _loanService;

    public LoanController(ILoanService loanService)
    {
        _loanService = loanService;
    }

    private Guid GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                           ?? User.FindFirst("sub")?.Value;
        return Guid.Parse(userIdClaim!);
    }

    private bool IsAdmin() => User.IsInRole("Admin");

    // POST: api/loan/borrow
    [HttpPost("borrow")]
    public async Task<IActionResult> BorrowBook([FromBody] BorrowBookRequestDto request)
    {
        var userId = GetCurrentUserId();
        var loan = await _loanService.BorrowBookAsync(userId, request.BookId);
        return CreatedAtAction(nameof(GetById), new { id = loan.Id }, loan);
    }

    // PUT: api/loan/{id}/return
    [HttpPut("{id:guid}/return")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> ReturnBook(Guid id)
    {
        var loan = await _loanService.ReturnBookAsync(id);
        return Ok(loan);
    }

    // GET: api/loan/my-loans
    [HttpGet("my-loans")]
    public async Task<IActionResult> GetMyLoans()
    {
        var userId = GetCurrentUserId();
        var loans = await _loanService.GetUserLoansAsync(userId);
        return Ok(loans);
    }

    // GET: api/loan/user/{userId}
    [HttpGet("user/{userId:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetUserLoans(Guid userId)
    {
        var loans = await _loanService.GetUserLoansAsync(userId);
        return Ok(loans);
    }

    // GET: api/loan/active
    [HttpGet("active")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetActiveLoans()
    {
        var loans = await _loanService.GetActiveLoansAsync();
        return Ok(loans);
    }

    // GET: api/loan/{id}
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var loan = await _loanService.GetByIdAsync(id);

        if (!IsAdmin() && loan.UserId != GetCurrentUserId())
        {
            return Forbid();
        }

        return Ok(loan);
    }
}