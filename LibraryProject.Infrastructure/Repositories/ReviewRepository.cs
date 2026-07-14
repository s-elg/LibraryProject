using LibraryProject.Application.Interfaces.Repositories;
using LibraryProject.Domain.Entities;
using LibraryProject.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LibraryProject.Infrastructure.Repositories;

public class ReviewRepository : GenericRepository<Review>, IReviewRepository
{
    public ReviewRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Review>> GetByBookAsync(Guid bookId)
    {
        return await _dbSet
            .Where(r => r.BookId == bookId)
            .OrderByDescending(r => r.CreatedDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<Review>> GetByUserAsync(Guid userId)
    {
        return await _dbSet
            .Where(r => r.UserId == userId)
            .OrderByDescending(r => r.CreatedDate)
            .ToListAsync();
    }

    public async Task<bool> HasUserReviewedBookAsync(Guid userId, Guid bookId)
    {
        return await _dbSet
            .AnyAsync(r => r.UserId == userId && r.BookId == bookId);
    }

    public async Task<double> GetAverageRatingAsync(Guid bookId)
    {
        var reviews = _dbSet.Where(r => r.BookId == bookId);

        if (!await reviews.AnyAsync())
            return 0;

        return await reviews.AverageAsync(r => r.Rating);
    }

    public async Task<IEnumerable<Review>> GetByRatingAsync(Guid bookId, int rating)
    {
        return await _dbSet
            .Where(r => r.BookId == bookId && r.Rating == rating)
            .ToListAsync();
    }
}