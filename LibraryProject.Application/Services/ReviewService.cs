using LibraryProject.Application.DTOs;
using LibraryProject.Application.Exceptions;
using LibraryProject.Application.Interfaces;
using LibraryProject.Application.Interfaces.Services;
using LibraryProject.Domain.Entities;

namespace LibraryProject.Application.Services;

public class ReviewService : IReviewService
{
    private readonly IUnitOfWork _unitOfWork;

    public ReviewService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ReviewResponseDto> CreateReviewAsync(Guid userId, CreateReviewDto dto)
    {
        ValidateRating(dto.Rating);

        // Kullanıcı bu kitabı daha önce ödünç almış mı?
        var userLoans = await _unitOfWork.Loans.GetByUserAsync(userId);
        var hasBorrowed = userLoans.Any(l => l.BookId == dto.BookId);
        if (!hasBorrowed)
        {
            throw new BookNotBorrowedException();
        }

        // Zaten yorum yapmış mı?
        var alreadyReviewed = await _unitOfWork.Reviews.HasUserReviewedBookAsync(userId, dto.BookId);
        if (alreadyReviewed)
        {
            throw new DuplicateReviewException();
        }

        var review = new Review
        {
            UserId = userId,
            BookId = dto.BookId,
            Rating = dto.Rating,
            Comment = dto.Comment
        };

        await _unitOfWork.Reviews.AddAsync(review);
        await _unitOfWork.SaveChangesAsync();

        return MapToDto(review);
    }

    public async Task<ReviewResponseDto> UpdateReviewAsync(Guid reviewId, Guid userId, UpdateReviewDto dto)
    {
        ValidateRating(dto.Rating);

        var review = await _unitOfWork.Reviews.GetByIdAsync(reviewId);
        if (review is null)
        {
            throw new ReviewNotFoundException(reviewId);
        }

        if (review.UserId != userId)
        {
            throw new UnauthorizedReviewAccessException();
        }

        review.Rating = dto.Rating;
        review.Comment = dto.Comment;

        _unitOfWork.Reviews.Update(review);
        await _unitOfWork.SaveChangesAsync();

        return MapToDto(review);
    }

    public async Task DeleteReviewAsync(Guid reviewId, Guid userId)
    {
        var review = await _unitOfWork.Reviews.GetByIdAsync(reviewId);
        if (review is null)
        {
            throw new ReviewNotFoundException(reviewId);
        }

        if (review.UserId != userId)
        {
            throw new UnauthorizedReviewAccessException();
        }

        _unitOfWork.Reviews.Delete(review);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<IEnumerable<ReviewResponseDto>> GetByBookAsync(Guid bookId)
    {
        var reviews = await _unitOfWork.Reviews.GetByBookAsync(bookId);
        return reviews.Select(MapToDto);
    }

    public async Task<IEnumerable<ReviewResponseDto>> GetByUserAsync(Guid userId)
    {
        var reviews = await _unitOfWork.Reviews.GetByUserAsync(userId);
        return reviews.Select(MapToDto);
    }

    public async Task<double> GetAverageRatingAsync(Guid bookId)
    {
        return await _unitOfWork.Reviews.GetAverageRatingAsync(bookId);
    }

    private static void ValidateRating(int rating)
    {
        if (rating < 1 || rating > 5)
        {
            throw new InvalidRatingException(rating);
        }
    }

    private static ReviewResponseDto MapToDto(Review review)
    {
        return new ReviewResponseDto(
            review.Id,
            review.UserId,
            review.BookId,
            review.Rating,
            review.Comment,
            review.CreatedDate
        );
    }
}