using LibraryProject.Application.DTOs.Loan;

namespace LibraryProject.Application.Interfaces.Services;

public interface ILoanService
{
    Task<LoanResponseDto> BorrowBookAsync(Guid userId, Guid bookId);
    Task<LoanResponseDto> ReturnBookAsync(Guid loanId);
    Task<IEnumerable<LoanResponseDto>> GetUserLoansAsync(Guid userId);
    Task<IEnumerable<LoanResponseDto>> GetActiveLoansAsync();
    Task<LoanResponseDto> GetByIdAsync(Guid loanId);
}