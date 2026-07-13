using LibraryProject.Domain.Entities;

namespace LibraryProject.Application.Interfaces;

public interface ILoanRepository : IGenericRepository<Loan>
{
    Task<IEnumerable<Loan>> GetActiveLoansAsync();
    Task<IEnumerable<Loan>> GetOverdueLoansAsync();
    Task<IEnumerable<Loan>> GetByUserAsync(Guid userId);
    Task<IEnumerable<Loan>> GetByBookAsync(Guid bookId);
}