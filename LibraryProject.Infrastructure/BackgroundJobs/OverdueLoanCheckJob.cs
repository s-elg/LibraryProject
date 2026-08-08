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

    public async Task CheckExpiredReservationsAsync()
    {
        var expiredReservations = await _context.Loans
            .Include(l => l.Book)
            .Where(l => l.Status == LoanStatus.Reserved
                        && l.PickupDeadline != null
                        && l.PickupDeadline < DateTime.UtcNow)
            .ToListAsync();

        if (!expiredReservations.Any())
        {
            _logger.LogInformation("Reservation expiry check tamamlandı, süresi geçen rezervasyon bulunamadı.");
            return;
        }

        foreach (var loan in expiredReservations)
        {
            loan.Status = LoanStatus.Expired;
            loan.PickupDeadline = null;

            if (loan.Book is not null)
            {
                loan.Book.AvailableCopies += 1;
            }
        }

        await _context.SaveChangesAsync();

        _logger.LogInformation(
            "{Count} adet rezervasyonun süresi doldu, Expired olarak işaretlendi ve stok iade edildi.",
            expiredReservations.Count);
    }
}