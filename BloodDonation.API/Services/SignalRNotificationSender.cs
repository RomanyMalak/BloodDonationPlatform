using BloodDonation.API.Hubs;
using BloodDonation.Application.DTOs;
using BloodDonation.Application.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace BloodDonation.API.Services;

public class SignalRNotificationSender : INotificationSender
{
    private readonly IHubContext<NotificationHub> _hubContext;
    private readonly NotificationConnectionTracker _connectionTracker;

    public SignalRNotificationSender(
        IHubContext<NotificationHub> hubContext,
        NotificationConnectionTracker connectionTracker)
    {
        _hubContext = hubContext;
        _connectionTracker = connectionTracker;
    }

    public async Task SendToUserAsync(
        Guid userId,
        NotificationDto notification,
        CancellationToken cancellationToken = default)
    {
        if (!_connectionTracker.HasActiveConnection(userId))
        {
            return;
        }

        await _hubContext.Clients
            .Group(NotificationHub.GetUserGroupName(userId))
            .SendAsync("ReceiveNotification", notification, cancellationToken);
    }
}
