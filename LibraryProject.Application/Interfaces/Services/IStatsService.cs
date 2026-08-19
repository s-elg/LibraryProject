using LibraryProject.Application.DTOs;

namespace LibraryProject.Application.Interfaces.Services;

public interface IStatsService
{
    Task<DashboardStatsDto> GetDashboardStatsAsync();
    Task<IEnumerable<MonthlyTrendDto>> GetMonthlyTrendAsync(int months);
    Task<IEnumerable<RecentActivityDto>> GetRecentActivityAsync(int count);
}