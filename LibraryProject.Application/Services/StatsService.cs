using System.Globalization;
using LibraryProject.Application.DTOs;
using LibraryProject.Application.Interfaces;
using LibraryProject.Application.Interfaces.Services;

namespace LibraryProject.Application.Services;

public class StatsService : IStatsService
{
    private readonly IUnitOfWork _unitOfWork;

    public StatsService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<DashboardStatsDto> GetDashboardStatsAsync()
    {
        // pageSize=1 veriyoruz çünkü gerçek kitap listesine ihtiyacımız yok,
        // sadece PagedResult.TotalCount'u okumak için en ucuz sorgu bu
        var bookPage = await _unitOfWork.Books.GetAllPagedAsync(1, 1);

        var activeLoans = await _unitOfWork.Loans.GetActiveLoansAsync();
        var overdueLoans = await _unitOfWork.Loans.GetOverdueLoansAsync();
        var activePenalties = await _unitOfWork.Penalties.GetActivePenaltiesAsync();

        return new DashboardStatsDto(
            bookPage.TotalCount,
            activeLoans.Count(),
            overdueLoans.Count(),
            activePenalties.Count()
        );
    }

    public async Task<IEnumerable<MonthlyTrendDto>> GetMonthlyTrendAsync(int months)
    {
        var since = DateTime.UtcNow.AddMonths(-(months - 1));
        since = new DateTime(since.Year, since.Month, 1, 0, 0, 0, DateTimeKind.Utc);

        var loans = await _unitOfWork.Loans.GetLoansSinceAsync(since);

        // Son N ay için sabit bir iskelet oluşturuyoruz; bu sayede o ay hiç
        // ödünç işlemi olmasa bile grafikte 0 olarak görünür, ay hiç kaybolmaz
        var result = new List<MonthlyTrendDto>();
        for (int i = months - 1; i >= 0; i--)
        {
            var monthDate = DateTime.UtcNow.AddMonths(-i);
            var count = loans.Count(l =>
                l.LoanDate.Year == monthDate.Year && l.LoanDate.Month == monthDate.Month);

            result.Add(new MonthlyTrendDto(
                monthDate.ToString("MMM", new CultureInfo("tr-TR")),
                count));
        }

        return result;
    }

    public async Task<IEnumerable<RecentActivityDto>> GetRecentActivityAsync(int count)
    {
        var loans = await _unitOfWork.Loans.GetRecentAsync(count);

        return loans.Select(l => new RecentActivityDto(
            l.Id,
            l.Book?.Title ?? string.Empty,
            l.User?.FullName ?? string.Empty,
            l.LoanDate,
            l.Status.ToString()
        ));
    }
}