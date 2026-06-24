using BloodDonation.Application.DTOs;

namespace BloodDonation.Application.Interfaces;

public interface INotificationService
{
    Task<List<NotificationDto>> GetUserNotificationsAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<bool> MarkAsReadAsync(Guid notificationId, Guid userId, CancellationToken cancellationToken = default);
    Task<int> MarkAllAsReadAsync(Guid userId, CancellationToken cancellationToken = default);

    Task CreateAsync(
        Guid userId,
        string title,
        string message,
        Guid? bloodRequestId,
        string type,
        CancellationToken cancellationToken);
}
