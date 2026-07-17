using LibraryProject.Application.Interfaces.Repositories;
using LibraryProject.Domain.Entities;
using LibraryProject.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LibraryProject.Infrastructure.Repositories;

public class LoanRepository : GenericRepository<Loan>, ILoanRepository
{
    public LoanRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Loan>> GetActiveLoansAsync()
    {
        var now = DateTime.UtcNow;
        return await _dbSet
            .Include(l => l.Book)
            .Where(l => l.Status == LoanStatus.Active && l.DueDate >= now)
            .ToListAsync();
    }

    public async Task<IEnumerable<Loan>> GetOverdueLoansAsync()
    {
        var now = DateTime.UtcNow;

        return await _dbSet
            .Where(l => l.Status == LoanStatus.Overdue ||
                        (l.Status == LoanStatus.Active && l.DueDate < now))
            .ToListAsync();
    }

    public async Task<IEnumerable<Loan>> GetByUserAsync(Guid userId)
    {
        return await _dbSet
            .Include(l => l.Book)
            .Where(l => l.UserId == userId)
            .OrderByDescending(l => l.LoanDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<Loan>> GetByBookAsync(Guid bookId)
    {
        return await _dbSet
            .Where(l => l.BookId == bookId)
            .OrderByDescending(l => l.LoanDate)
            .ToListAsync();
    }

    public async Task<Loan?> GetByIdWithDetailsAsync(Guid id)
    {
        return await _dbSet
            .Include(l => l.Book)
            .FirstOrDefaultAsync(l => l.Id == id);
    }
}