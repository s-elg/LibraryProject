using LibraryProject.Application.Common;
using LibraryProject.Application.DTOs;

namespace LibraryProject.Application.Interfaces.Services;

public interface IBookService
{
    Task<BookDto> GetByIdAsync(Guid id);
    Task<PagedResult<BookDto>> GetAllAsync(int pageNumber, int pageSize);
    Task<PagedResult<BookDto>> SearchAsync(string searchTerm, int pageNumber, int pageSize);
    Task<PagedResult<BookDto>> GetByCategoryAsync(Guid categoryId, int pageNumber, int pageSize);
    Task<IEnumerable<BookDto>> GetAvailableBooksAsync();
    Task<IEnumerable<BookDto>> GetRecentlyAddedAsync(int count = 10);
    Task<BookDto> CreateAsync(CreateBookRequestDto request);
    Task<BookDto> UpdateAsync(Guid id, UpdateBookRequestDto request);
    Task DeleteAsync(Guid id);
}