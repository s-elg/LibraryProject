namespace LibraryProject.Application.DTOs;

public record CreatePenaltyDto(Guid UserId, string Reason);

public record PenaltyResponseDto(
    Guid Id,
    Guid UserId,
    Guid? LoanId,
    string Reason,
    DateTime SuspensionEndDate,
    string Status,
    DateTime CreatedDate
);