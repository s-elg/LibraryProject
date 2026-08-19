namespace LibraryProject.Application.DTOs;

public record DashboardStatsDto(
    int TotalBooks,
    int ActiveLoans,
    int OverdueLoans,
    int ActivePenalties
);

public record MonthlyTrendDto(
    string Month,
    int Count
);

public record RecentActivityDto(
    Guid LoanId,
    string BookTitle,
    string UserFullName,
    DateTime LoanDate,
    string Status
);