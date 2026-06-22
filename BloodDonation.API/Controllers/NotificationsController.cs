using BloodDonation.Application.Interfaces;
using BloodDonation.Application.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BloodDonation.API.Controllers;

[ApiController]
[Route("api/notifications")]
[Authorize]
public class NotificationsController : ControllerBase
{
    private readonly INotificationService _notificationService;

    public NotificationsController(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    [HttpPost("send")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Send(
        [FromBody] SendNotificationRequestDto request,
        CancellationToken cancellationToken)
    {
        try
        {
            var notification = await _notificationService.SendAsync(
                request,
                cancellationToken);

            return Ok(notification);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    // GET /api/notifications/my
    [HttpGet]
    [HttpGet("my")]
    public async Task<IActionResult> GetMyNotifications(CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        if (userId == null)
            return Unauthorized(new { message = "يجب تسجيل الدخول أولاً" });

        var notifications = await _notificationService.GetUserNotificationsAsync(
            userId.Value,
            cancellationToken);

        return Ok(notifications);
    }

    [HttpGet("admin")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAdminNotifications(CancellationToken cancellationToken)
    {
        var notifications = await _notificationService.GetAdminNotificationsAsync(cancellationToken);
        return Ok(notifications);
    }

    [HttpPut("{id:guid}/read")]
    public async Task<IActionResult> MarkAsRead(
        Guid id,
        CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        if (userId == null)
            return Unauthorized(new { message = "يجب تسجيل الدخول أولاً" });

        var marked = await _notificationService.MarkAsReadAsync(
            id,
            userId.Value,
            cancellationToken);

        if (!marked)
            return NotFound(new { message = "Notification not found." });

        return Ok(new { message = "تم تعليم الإشعار كمقروء" });
    }

    [HttpPut("read-all")]
    public async Task<IActionResult> MarkAllAsRead(CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        if (userId == null)
            return Unauthorized(new { message = "يجب تسجيل الدخول أولاً" });

        var count = await _notificationService.MarkAllAsReadAsync(
            userId.Value,
            cancellationToken);

        return Ok(new { message = "تم تعليم كل الإشعارات كمقروءة", count });
    }

    [HttpPost("retry-failed")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> RetryFailed(CancellationToken cancellationToken)
    {
        var result = await _notificationService.RetryFailedAsync(cancellationToken);
        return Ok(result);
    }

    private Guid? GetCurrentUserId()
    {
        var idClaim = User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? User.FindFirstValue("sub");

        if (idClaim == null) return null;
        if (Guid.TryParse(idClaim, out var id)) return id;
        return null;
    }
}
