namespace LibraryProject.Application.DTOs;

public record CreateReviewDto(Guid BookId, int Rating, string Comment);

public record UpdateReviewDto(int Rating, string Comment);

public record ReviewResponseDto(
    Guid Id,
    Guid UserId,
    Guid BookId,
    int Rating,
    string Comment,
    DateTime CreatedDate
);