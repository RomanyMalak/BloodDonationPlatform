using BloodDonation.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace BloodDonation.API.Hubs;

[Authorize]
public class NotificationHub : Hub
{
    private readonly NotificationConnectionTracker _connectionTracker;

    public NotificationHub(NotificationConnectionTracker connectionTracker)
    {
        _connectionTracker = connectionTracker;
    }

    public override async Task OnConnectedAsync()
    {
        var userId = GetCurrentUserId();

        if (userId.HasValue)
        {
            var groupName = GetUserGroupName(userId.Value);
            _connectionTracker.AddConnection(userId.Value, Context.ConnectionId);
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        }

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var userId = GetCurrentUserId();

        if (userId.HasValue)
        {
            _connectionTracker.RemoveConnection(userId.Value, Context.ConnectionId);
        }

        await base.OnDisconnectedAsync(exception);
    }

    public static string GetUserGroupName(Guid userId) => userId.ToString();

    private Guid? GetCurrentUserId()
    {
        var idClaim = Context.User?.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? Context.User?.FindFirstValue("sub");

        return Guid.TryParse(idClaim, out var userId) ? userId : null;
    }
}
