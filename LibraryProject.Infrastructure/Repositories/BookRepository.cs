using LibraryProject.Application.Interfaces;
using LibraryProject.Domain.Entities;
using LibraryProject.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LibraryProject.Infrastructure.Repositories;

public class BookRepository : GenericRepository<Book>, IBookRepository
{
    public BookRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Book?> GetByIdWithCategoryAsync(Guid id)
    {
        return await _dbSet
            .Include(b => b.Category)
            .FirstOrDefaultAsync(b => b.Id == id);
    }

    public async Task<IEnumerable<Book>> GetByCategoryAsync(Guid categoryId)
    {
        return await _dbSet
            .Where(b => b.CategoryId == categoryId)
            .ToListAsync();
    }

    public async Task<IEnumerable<Book>> SearchAsync(string searchTerm)
    {
        var normalizedTerm = searchTerm.ToLower();

        return await _dbSet
            .Where(b => b.Title.ToLower().Contains(normalizedTerm)
                     || b.Author.ToLower().Contains(normalizedTerm))
            .ToListAsync();
    }

    public async Task<IEnumerable<Book>> GetAvailableBooksAsync()
    {
        return await _dbSet
            .Where(b => b.AvailableCopies > 0)
            .ToListAsync();
    }

    public async Task<IEnumerable<Book>> GetRecentlyAddedAsync(int count = 10)
    {
        return await _dbSet
            .OrderByDescending(b => b.CreatedDate)
            .Take(count)
            .ToListAsync();
    }
}