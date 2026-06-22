using BloodDonation.Application.DTOs;
using BloodDonation.Application.Interfaces;
using BloodDonation.Infrastructure.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace BloodDonation.Infrastructure.Services;

public class SignalRNotificationSender : INotificationSender
{
    private readonly IHubContext<NotificationHub> _hubContext;

    public SignalRNotificationSender(IHubContext<NotificationHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public Task SendToUserAsync(
        Guid userId,
        NotificationDto notification,
        CancellationToken cancellationToken)
    {
        return _hubContext.Clients
            .User(userId.ToString())
            .SendAsync("ReceiveNotification", notification, cancellationToken);
    }
}
