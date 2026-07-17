namespace LibraryProject.Application.DTOs.Loan;

public record BorrowBookRequestDto(Guid BookId);

public record LoanResponseDto(
    Guid Id,
    Guid UserId,
    Guid BookId,
    string BookTitle,
    DateTime LoanDate,
    DateTime DueDate,
    DateTime? ReturnDate,
    string Status
);