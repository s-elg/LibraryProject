using LibraryProject.Application.Common;
using LibraryProject.Domain.Entities;

namespace LibraryProject.Application.Interfaces.Repositories;

public interface IBookRepository : IGenericRepository<Book>
{
    Task<Book?> GetByIdWithCategoryAsync(Guid id);
    Task<PagedResult<Book>> GetAllPagedAsync(int pageNumber, int pageSize);
    Task<PagedResult<Book>> GetByCategoryAsync(Guid categoryId, int pageNumber, int pageSize);
    Task<PagedResult<Book>> SearchAsync(string searchTerm, int pageNumber, int pageSize);
    Task<IEnumerable<Book>> GetAvailableBooksAsync();
    Task<IEnumerable<Book>> GetRecentlyAddedAsync(int count = 10);
}