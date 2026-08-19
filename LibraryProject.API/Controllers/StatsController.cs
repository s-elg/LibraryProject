using LibraryProject.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryProject.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class StatsController : ControllerBase
{
    private readonly IStatsService _statsService;

    public StatsController(IStatsService statsService)
    {
        _statsService = statsService;
    }

    // GET: api/stats/dashboard
    [HttpGet("dashboard")]
    public async Task<IActionResult> GetDashboard()
    {
        var result = await _statsService.GetDashboardStatsAsync();
        return Ok(result);
    }

    // GET: api/stats/monthly-trend?months=6
    [HttpGet("monthly-trend")]
    public async Task<IActionResult> GetMonthlyTrend([FromQuery] int months = 6)
    {
        var result = await _statsService.GetMonthlyTrendAsync(months);
        return Ok(result);
    }

    // GET: api/stats/recent-activity?count=5
    [HttpGet("recent-activity")]
    public async Task<IActionResult> GetRecentActivity([FromQuery] int count = 5)
    {
        var result = await _statsService.GetRecentActivityAsync(count);
        return Ok(result);
    }
}