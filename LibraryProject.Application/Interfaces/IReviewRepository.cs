using LibraryProject.Domain.Entities;

namespace LibraryProject.Application.Interfaces;

public interface IReviewRepository : IGenericRepository<Review>
{
    Task<IEnumerable<Review>> GetByBookAsync(Guid bookId);
    Task<IEnumerable<Review>> GetByUserAsync(Guid userId);
    Task<bool> HasUserReviewedBookAsync(Guid userId, Guid bookId);
    Task<double> GetAverageRatingAsync(Guid bookId);
    Task<IEnumerable<Review>> GetByRatingAsync(Guid bookId, int rating);
}