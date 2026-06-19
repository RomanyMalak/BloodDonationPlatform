using BloodDonation.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BloodDonation.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class NotificationsController : ControllerBase
{
    private readonly INotificationService _notificationService;

    public NotificationsController(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    // GET /api/notifications
    // جيب كل إشعارات المستخدم الحالي
    [HttpGet]
    public async Task<IActionResult> GetMyNotifications()
    {
        var userId = GetCurrentUserId();
        if (userId == null)
            return Unauthorized(new { message = "يجب تسجيل الدخول أولاً" });

        var notifications = await _notificationService.GetUserNotificationsAsync(userId.Value);
        return Ok(notifications);
    }

    // PUT /api/notifications/{id}/read
    // علّم على إشعار واحد كمقروء
    [HttpPut("{id:guid}/read")]
    public async Task<IActionResult> MarkAsRead(Guid id)
    {
        var userId = GetCurrentUserId();
        if (userId == null)
            return Unauthorized(new { message = "يجب تسجيل الدخول أولاً" });

        await _notificationService.MarkAsReadAsync(id, userId.Value);
        return Ok(new { message = "تم تعليم الإشعار كمقروء" });
    }

    // PUT /api/notifications/read-all
    // علّم على كل الإشعارات كمقروءة
    [HttpPut("read-all")]
    public async Task<IActionResult> MarkAllAsRead()
    {
        var userId = GetCurrentUserId();
        if (userId == null)
            return Unauthorized(new { message = "يجب تسجيل الدخول أولاً" });

        await _notificationService.MarkAllAsReadAsync(userId.Value);
        return Ok(new { message = "تم تعليم كل الإشعارات كمقروءة" });
    }

    private Guid? GetCurrentUserId()
    {
        var idClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (idClaim == null) return null;
        if (Guid.TryParse(idClaim, out var id)) return id;
        return null;
    }
}
