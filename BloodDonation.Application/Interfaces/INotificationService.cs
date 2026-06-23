using BloodDonation.Application.DTOs;
using BloodDonation.Domain.Enums;

namespace BloodDonation.Application.Interfaces;

public interface INotificationService
{
    Task<NotificationDto> SendAsync(SendNotificationRequestDto request, CancellationToken cancellationToken);
    Task<List<NotificationDto>> GetUserNotificationsAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<List<AdminNotificationDto>> GetAdminNotificationsAsync(CancellationToken cancellationToken);
    Task<bool> MarkAsReadAsync(Guid notificationId, Guid userId, CancellationToken cancellationToken = default);
    Task<int> MarkAllAsReadAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<RetryFailedNotificationsResultDto> RetryFailedAsync(CancellationToken cancellationToken);

    Task<NotificationDto> CreateAsync(
        Guid userId,
        string title,
        string message,
        Guid? bloodRequestId,
        string type,
        CancellationToken cancellationToken,
        string? reason = null,
        NotificationPriority priority = NotificationPriority.Normal);
}
