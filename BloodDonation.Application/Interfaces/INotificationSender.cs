using BloodDonation.Application.DTOs;

namespace BloodDonation.Application.Interfaces;

public interface INotificationSender
{
    Task SendToUserAsync(Guid userId, NotificationDto notification, CancellationToken cancellationToken);
}
