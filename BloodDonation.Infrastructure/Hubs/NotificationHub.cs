using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace BloodDonation.Infrastructure.Hubs;

[Authorize]
public class NotificationHub : Hub
{
}

public class NotificationUserIdProvider : IUserIdProvider
{
    public string? GetUserId(HubConnectionContext connection)
    {
        return connection.User?.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? connection.User?.FindFirstValue(JwtRegisteredClaimNames.Sub)
            ?? connection.User?.FindFirstValue("sub");
    }
}
