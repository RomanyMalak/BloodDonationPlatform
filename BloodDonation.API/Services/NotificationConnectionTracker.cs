using System.Collections.Concurrent;

namespace BloodDonation.API.Services;

public class NotificationConnectionTracker
{
    private readonly ConcurrentDictionary<Guid, ConcurrentDictionary<string, byte>> _connections = new();

    public void AddConnection(Guid userId, string connectionId)
    {
        var userConnections = _connections.GetOrAdd(
            userId,
            _ => new ConcurrentDictionary<string, byte>());

        userConnections[connectionId] = 0;
    }

    public void RemoveConnection(Guid userId, string connectionId)
    {
        if (!_connections.TryGetValue(userId, out var userConnections))
        {
            return;
        }

        userConnections.TryRemove(connectionId, out _);

        if (userConnections.IsEmpty)
        {
            _connections.TryRemove(userId, out _);
        }
    }

    public bool HasActiveConnection(Guid userId)
    {
        return _connections.TryGetValue(userId, out var userConnections)
            && !userConnections.IsEmpty;
    }
}
