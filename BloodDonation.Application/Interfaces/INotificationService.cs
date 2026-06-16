using BloodDonation.Application.DTOs;

namespace BloodDonation.Application.Interfaces;

public interface INotificationService
{
    Task<List<NotificationDto>> GetUserNotificationsAsync(Guid userId);
    Task MarkAsReadAsync(Guid notificationId, Guid userId);
    Task MarkAllAsReadAsync(Guid userId);

    Task CreateAsync(Guid userId, string title,string message,Guid? bloodRequestId, string type, CancellationToken cancellationToken);
}
