using BloodDonation.Application.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace BloodDonation.Infrastructure.Services;

public sealed class NotificationAgentBackgroundService : BackgroundService
{
    private readonly INotificationAgentQueue _queue;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<NotificationAgentBackgroundService> _logger;

    public NotificationAgentBackgroundService(
        INotificationAgentQueue queue,
        IServiceScopeFactory scopeFactory,
        ILogger<NotificationAgentBackgroundService> logger)
    {
        _queue = queue;
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await foreach (var bloodRequestId in _queue.ReadAllAsync(stoppingToken))
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var pipeline = scope.ServiceProvider.GetRequiredService<IAiMatchingPipelineService>();

                await pipeline.RunAsync(bloodRequestId, stoppingToken);
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                _logger.LogError(
                    ex,
                    "Notification agent failed for blood request {BloodRequestId}",
                    bloodRequestId);
            }
        }
    }
}
