namespace BloodDonation.Application.Interfaces;

public interface INotificationAgentQueue
{
    Task EnqueueAsync(Guid bloodRequestId, CancellationToken cancellationToken);
    IAsyncEnumerable<Guid> ReadAllAsync(CancellationToken cancellationToken);
}
