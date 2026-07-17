using LibraryProject.Application.Common;
using LibraryProject.Application.Interfaces.Repositories;
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

    public async Task<PagedResult<Book>> GetAllPagedAsync(int pageNumber, int pageSize)
    {
        var query = _dbSet.Include(b => b.Category).OrderBy(b => b.Title);

        var totalCount = await query.CountAsync();
        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResult<Book>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }

    public async Task<PagedResult<Book>> GetByCategoryAsync(Guid categoryId, int pageNumber, int pageSize)
    {
        var query = _dbSet.Include(b => b.Category).Where(b => b.CategoryId == categoryId);

        var totalCount = await query.CountAsync();
        var items = await query
            .OrderBy(b => b.Title)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResult<Book>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }

    public async Task<PagedResult<Book>> SearchAsync(string searchTerm, int pageNumber, int pageSize)
    {
        var normalizedTerm = searchTerm.ToLower();

        var query = _dbSet.Include(b => b.Category).Where(b =>
            b.Title.ToLower().Contains(normalizedTerm) ||
            b.Author.ToLower().Contains(normalizedTerm));

        var totalCount = await query.CountAsync();
        var items = await query
            .OrderBy(b => b.Title)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResult<Book>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }

    public async Task<IEnumerable<Book>> GetAvailableBooksAsync()
    {
        return await _dbSet
            .Include(b => b.Category)
            .Where(b => b.AvailableCopies > 0)
            .ToListAsync();
    }

    public async Task<IEnumerable<Book>> GetRecentlyAddedAsync(int count = 10)
    {
        return await _dbSet
            .Include(b => b.Category)
            .OrderByDescending(b => b.CreatedDate)
            .Take(count)
            .ToListAsync();
    }
}