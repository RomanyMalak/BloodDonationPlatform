namespace BloodDonation.Application.Interfaces;

public interface IOcrVerificationQueue
{
    Task EnqueueAsync(Guid bloodRequestId, CancellationToken cancellationToken);
    IAsyncEnumerable<Guid> ReadAllAsync(CancellationToken cancellationToken);
}