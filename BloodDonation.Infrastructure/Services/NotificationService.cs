using BloodDonation.Application.DTOs;
using BloodDonation.Application.Interfaces;
using BloodDonation.Domain.Entities;
using BloodDonation.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BloodDonation.Infrastructure.Services;

public class NotificationService : INotificationService
{
    private readonly AppDbContext _context;
    private readonly INotificationSender _notificationSender;
    private readonly ILogger<NotificationService> _logger;

    public NotificationService(
        AppDbContext context,
        INotificationSender notificationSender,
        ILogger<NotificationService> logger)
    {
        _context = context;
        _notificationSender = notificationSender;
        _logger = logger;
    }

    public async Task<List<NotificationDto>> GetUserNotificationsAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var notifications = await _context.Notifications
            .Where(n => n.UserId == userId)
            .OrderByDescending(n => n.CreatedAt)
            .ToListAsync(cancellationToken);

        return notifications.Select(MapToDto).ToList();
    }

    public async Task<bool> MarkAsReadAsync(
        Guid notificationId,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var notification = await _context.Notifications
            .FirstOrDefaultAsync(
                n => n.Id == notificationId && n.UserId == userId,
                cancellationToken);

        if (notification == null)
        {
            return false;
        }

        notification.IsRead = true;

        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<int> MarkAllAsReadAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var notifications = await _context.Notifications
            .Where(n => n.UserId == userId && !n.IsRead)
            .ToListAsync(cancellationToken);

        foreach (var n in notifications)
        {
            n.IsRead = true;
        }

        await _context.SaveChangesAsync(cancellationToken);
        return notifications.Count;
    }

    public async Task CreateAsync(
        Guid userId,
        string title,
        string message,
        Guid? bloodRequestId,
        string type,
        CancellationToken cancellationToken)
    {
        var notification = new Notification
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            BloodRequestId = bloodRequestId,
            Title = title,
            Message = message,
            Type = type,
            IsRead = false,
            CreatedAt = DateTime.UtcNow
        };

        await _context.Notifications.AddAsync(notification, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        try
        {
            await _notificationSender.SendToUserAsync(
                userId,
                MapToDto(notification),
                cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(
                ex,
                "SignalR notification send failed for notification {NotificationId}.",
                notification.Id);
        }
    }

    private static NotificationDto MapToDto(Notification notification)
    {
        return new NotificationDto
        {
            Id = notification.Id,
            Title = notification.Title,
            Message = notification.Message,
            IsRead = notification.IsRead,
            CreatedAt = notification.CreatedAt,
            BloodRequestId = notification.BloodRequestId
        };
    }
}
