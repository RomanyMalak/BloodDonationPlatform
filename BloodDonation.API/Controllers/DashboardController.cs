using BloodDonation.Application.Features.Auth.Commands.CreateAdmin;
using BloodDonation.Application.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BloodDonation.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class DashboardController : ControllerBase
{
    private readonly IDashboardService _dashboardService;
    private readonly IMediator _mediator;

    public DashboardController(IDashboardService dashboardService,IMediator mediator)
    {
        _dashboardService = dashboardService;
        _mediator = mediator;
    }

    [HttpPost("create-admin")]
    public async Task<IActionResult> CreateAdmin(
    [FromBody] CreateAdminCommand command,
    CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);

        return result
            ? Ok("Admin created successfully.")
            : BadRequest("Email already exists.");
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
