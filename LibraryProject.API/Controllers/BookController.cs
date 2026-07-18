using LibraryProject.Application.DTOs;
using LibraryProject.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryProject.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BookController : ControllerBase
{
    private readonly IBookService _bookService;

    public BookController(IBookService bookService)
    {
        _bookService = bookService;
    }

    // GET: api/book/{id}
    [HttpGet("{id:guid}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetById(Guid id)
    {
        var book = await _bookService.GetByIdAsync(id);
        return Ok(book);
    }

    // GET: api/book?pageNumber=1&pageSize=10
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        var result = await _bookService.GetAllAsync(pageNumber, pageSize);
        return Ok(result);
    }

    // GET: api/book/search?searchTerm=harry&pageNumber=1&pageSize=10
    [HttpGet("search")]
    [AllowAnonymous]
    public async Task<IActionResult> Search([FromQuery] string searchTerm, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        var result = await _bookService.SearchAsync(searchTerm, pageNumber, pageSize);
        return Ok(result);
    }

    // GET: api/book/category/{categoryId}?pageNumber=1&pageSize=10
    [HttpGet("category/{categoryId:guid}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetByCategory(Guid categoryId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        var result = await _bookService.GetByCategoryAsync(categoryId, pageNumber, pageSize);
        return Ok(result);
    }

    // GET: api/book/available
    [HttpGet("available")]
    [AllowAnonymous]
    public async Task<IActionResult> GetAvailableBooks()
    {
        var result = await _bookService.GetAvailableBooksAsync();
        return Ok(result);
    }

    // GET: api/book/recent?count=10
    [HttpGet("recent")]
    [AllowAnonymous]
    public async Task<IActionResult> GetRecentlyAdded([FromQuery] int count = 10)
    {
        var result = await _bookService.GetRecentlyAddedAsync(count);
        return Ok(result);
    }

    // POST: api/book
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] CreateBookRequestDto request)
    {
        var book = await _bookService.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = book.Id }, book);
    }

    // PUT: api/book/{id}
    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateBookRequestDto request)
    {
        var book = await _bookService.UpdateAsync(id, request);
        return Ok(book);
    }

    // DELETE: api/book/{id}
    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _bookService.DeleteAsync(id);
        return NoContent();
    }
}