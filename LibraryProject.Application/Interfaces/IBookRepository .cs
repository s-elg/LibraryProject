using LibraryProject.Domain.Entities;

namespace LibraryProject.Application.Interfaces;

public interface IBookRepository : IGenericRepository<Book>
{
    Task<Book?> GetByIdWithCategoryAsync(Guid id);
    Task<IEnumerable<Book>> GetByCategoryAsync(Guid categoryId);
    Task<IEnumerable<Book>> SearchAsync(string searchTerm);
    Task<IEnumerable<Book>> GetAvailableBooksAsync();
    Task<IEnumerable<Book>> GetRecentlyAddedAsync(int count = 10);
}