using LibraryProject.Domain.Entities;

namespace LibraryProject.Application.Interfaces.Repositories;

public interface ILoanRepository : IGenericRepository<Loan>
{
    Task<Loan?> GetByIdWithDetailsAsync(Guid id);
    Task<IEnumerable<Loan>> GetActiveLoansAsync();
    Task<IEnumerable<Loan>> GetOverdueLoansAsync();
    Task<IEnumerable<Loan>> GetByUserAsync(Guid userId);
    Task<IEnumerable<Loan>> GetByBookAsync(Guid bookId);
}