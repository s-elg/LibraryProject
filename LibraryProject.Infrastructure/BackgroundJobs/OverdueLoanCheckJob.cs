using LibraryProject.Domain.Entities;
using LibraryProject.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace LibraryProject.Infrastructure.BackgroundJobs;

public class OverdueLoanCheckJob
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<OverdueLoanCheckJob> _logger;

    public OverdueLoanCheckJob(ApplicationDbContext context, ILogger<OverdueLoanCheckJob> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task CheckOverdueLoansAsync()
    {
        var overdueLoans = await _context.Loans
            .Where(l => l.Status == LoanStatus.Active && l.DueDate < DateTime.UtcNow)
            .ToListAsync();

        if (!overdueLoans.Any())
        {
            _logger.LogInformation("Overdue loan check tamamlandı, işaretlenecek loan bulunamadı.");
            return;
        }

        foreach (var loan in overdueLoans)
        {
            loan.Status = LoanStatus.Overdue;
        }

        await _context.SaveChangesAsync();

        _logger.LogInformation("{Count} adet loan Overdue olarak işaretlendi.", overdueLoans.Count);
    }
}