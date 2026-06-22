using BloodDonation.Application.DTOs;
using BloodDonation.Application.Interfaces;
using BloodDonation.Domain.Entities;
using BloodDonation.Domain.Enums;
using BloodDonation.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BloodDonation.Infrastructure.Services;

public class NotificationService : INotificationService
{
    private readonly AppDbContext _context;
    private readonly INotificationSender _notificationSender;

    public NotificationService(
        AppDbContext context,
        INotificationSender notificationSender)
    {
        _context = context;
        _notificationSender = notificationSender;
    }

    public async Task<NotificationDto> SendAsync(
        SendNotificationRequestDto request,
        CancellationToken cancellationToken)
    {
        var priority = ParsePriority(request.Priority);

        var userExists = await _context.Users
            .AnyAsync(u => u.Id == request.UserId, cancellationToken);

        if (!userExists)
        {
            throw new KeyNotFoundException("User not found.");
        }

        if (request.BloodRequestId.HasValue)
        {
            var bloodRequestExists = await _context.BloodRequests
                .AnyAsync(r => r.Id == request.BloodRequestId.Value, cancellationToken);

            if (!bloodRequestExists)
            {
                throw new KeyNotFoundException("Blood request not found.");
            }
        }

        var notification = new Notification
        {
            Id = Guid.NewGuid(),
            UserId = request.UserId,
            BloodRequestId = request.BloodRequestId,
            Title = request.Title.Trim(),
            Message = request.Message.Trim(),
            Reason = string.IsNullOrWhiteSpace(request.Reason) ? null : request.Reason.Trim(),
            Type = "Manual",
            Priority = priority,
            Status = NotificationStatus.Pending,
            IsRead = false,
            CreatedAt = DateTime.UtcNow
        };

        await _context.Notifications.AddAsync(notification, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        await TrySendAsync(notification, cancellationToken);

        return MapToDto(notification);
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

    public async Task<List<AdminNotificationDto>> GetAdminNotificationsAsync(
        CancellationToken cancellationToken)
    {
        var notifications = await _context.Notifications
            .Include(n => n.User)
            .OrderByDescending(n => n.CreatedAt)
            .ToListAsync(cancellationToken);

        return notifications
            .Select(n => new AdminNotificationDto
            {
                Id = n.Id,
                UserId = n.UserId,
                UserFullName = n.User.FullName,
                UserEmail = n.User.Email,
                Title = n.Title,
                Message = n.Message,
                Reason = n.Reason,
                Type = n.Type,
                Priority = n.Priority.ToString(),
                Status = n.Status.ToString(),
                IsRead = n.IsRead,
                CreatedAt = n.CreatedAt,
                SentAt = n.SentAt,
                ReadAt = n.ReadAt,
                BloodRequestId = n.BloodRequestId,
                RetryCount = n.RetryCount,
                ErrorMessage = n.ErrorMessage
            })
            .ToList();
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
        notification.ReadAt ??= DateTime.UtcNow;

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
            n.ReadAt ??= DateTime.UtcNow;
        }

        await _context.SaveChangesAsync(cancellationToken);
        return notifications.Count;
    }

    public async Task<RetryFailedNotificationsResultDto> RetryFailedAsync(
        CancellationToken cancellationToken)
    {
        var failedNotifications = await _context.Notifications
            .Where(n => n.Status == NotificationStatus.Failed)
            .OrderBy(n => n.CreatedAt)
            .ToListAsync(cancellationToken);

        var result = new RetryFailedNotificationsResultDto
        {
            Attempted = failedNotifications.Count
        };

        foreach (var notification in failedNotifications)
        {
            await TrySendAsync(notification, cancellationToken);

            if (notification.Status == NotificationStatus.Sent)
            {
                result.Sent++;
            }
            else
            {
                result.Failed++;
            }
        }

        return result;
    }

    public async Task<NotificationDto> CreateAsync(
        Guid userId,
        string title,
        string message,
        Guid? bloodRequestId,
        string type,
        CancellationToken cancellationToken,
        string? reason = null,
        NotificationPriority priority = NotificationPriority.Normal)
    {
        var notification = new Notification
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            BloodRequestId = bloodRequestId,
            Title = title.Trim(),
            Message = message.Trim(),
            Reason = string.IsNullOrWhiteSpace(reason) ? type : reason.Trim(),
            Type = type,
            Priority = priority,
            Status = NotificationStatus.Pending,
            IsRead = false,
            CreatedAt = DateTime.UtcNow
        };

        await _context.Notifications.AddAsync(notification, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        await TrySendAsync(notification, cancellationToken);

        return MapToDto(notification);
    }

    private async Task TrySendAsync(
        Notification notification,
        CancellationToken cancellationToken)
    {
        try
        {
            notification.Status = NotificationStatus.Pending;
            await _context.SaveChangesAsync(cancellationToken);

            await _notificationSender.SendToUserAsync(
                notification.UserId,
                MapToDto(notification),
                cancellationToken);

            notification.Status = NotificationStatus.Sent;
            notification.SentAt = DateTime.UtcNow;
            notification.ErrorMessage = null;
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            notification.Status = NotificationStatus.Failed;
            notification.RetryCount++;
            notification.ErrorMessage = ex.Message.Length > 500
                ? ex.Message[..500]
                : ex.Message;
        }

        await _context.SaveChangesAsync(cancellationToken);
    }

    private static NotificationPriority ParsePriority(string priority)
    {
        if (Enum.TryParse<NotificationPriority>(
                priority,
                ignoreCase: true,
                out var parsed))
        {
            return parsed;
        }

        throw new ArgumentException(
            $"Invalid notification priority '{priority}'. Allowed values: Low, Normal, High, Critical.");
    }

    private static NotificationDto MapToDto(Notification notification)
    {
        return new NotificationDto
        {
            Id = notification.Id,
            Title = notification.Title,
            Message = notification.Message,
            Reason = notification.Reason,
            Type = notification.Type,
            Priority = notification.Priority.ToString(),
            Status = notification.Status.ToString(),
            IsRead = notification.IsRead,
            CreatedAt = notification.CreatedAt,
            SentAt = notification.SentAt,
            ReadAt = notification.ReadAt,
            BloodRequestId = notification.BloodRequestId
        };
    }
}
