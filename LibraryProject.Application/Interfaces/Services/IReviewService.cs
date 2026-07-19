using LibraryProject.Application.DTOs;

namespace LibraryProject.Application.Interfaces.Services;

public interface IReviewService
{
    Task<ReviewResponseDto> CreateReviewAsync(Guid userId, CreateReviewDto dto);
    Task<ReviewResponseDto> UpdateReviewAsync(Guid reviewId, Guid userId, UpdateReviewDto dto);
    Task DeleteReviewAsync(Guid reviewId, Guid userId, bool isAdmin);
    Task<IEnumerable<ReviewResponseDto>> GetByBookAsync(Guid bookId);
    Task<IEnumerable<ReviewResponseDto>> GetByUserAsync(Guid userId);
    Task<double> GetAverageRatingAsync(Guid bookId);
}