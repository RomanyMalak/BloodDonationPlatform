using BloodDonation.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BloodDonation.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DashboardController : ControllerBase
{
    private readonly IDashboardService _dashboardService;

    public DashboardController(IDashboardService dashboardService)
    {
        _dashboardService = dashboardService;
    }

    // GET /api/dashboard/stats
    // إحصائيات عامة للـ Admin
    [HttpGet("stats")]
    public async Task<IActionResult> GetStats()
    {
        var stats = await _dashboardService.GetStatsAsync();
        return Ok(stats);
    }

    // GET /api/dashboard/ai-logs
    // سجل نتائج الـ AI Pipeline
    [HttpGet("ai-logs")]
    public async Task<IActionResult> GetAiLogs()
    {
        var logs = await _dashboardService.GetAiLogsAsync();
        return Ok(logs);
    }
}
