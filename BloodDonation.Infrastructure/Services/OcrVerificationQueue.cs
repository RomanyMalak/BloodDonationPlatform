using BloodDonation.Application.Interfaces;
using System.Threading.Channels;

namespace BloodDonation.Infrastructure.Services;

public class OcrVerificationQueue : IOcrVerificationQueue
{
    private readonly Channel<Guid> _channel =
        Channel.CreateUnbounded<Guid>();

    public async Task EnqueueAsync(Guid bloodRequestId, CancellationToken cancellationToken)
        => await _channel.Writer.WriteAsync(bloodRequestId, cancellationToken);

    public IAsyncEnumerable<Guid> ReadAllAsync(CancellationToken cancellationToken)
        => _channel.Reader.ReadAllAsync(cancellationToken);
}