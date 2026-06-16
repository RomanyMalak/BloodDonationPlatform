using BloodDonation.Application.Features.BloodRequests.Commands.VerifyBloodRequestDocument;
using BloodDonation.Application.Interfaces;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace BloodDonation.Infrastructure.Services;

public class OcrBackgroundService : BackgroundService
{
    private readonly IOcrVerificationQueue _queue;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<OcrBackgroundService> _logger;

    public OcrBackgroundService(
        IOcrVerificationQueue queue,
        IServiceScopeFactory scopeFactory,
        ILogger<OcrBackgroundService> logger)
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
                var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

                await mediator.Send(
                    new VerifyBloodRequestDocumentCommand(bloodRequestId),
                    stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "OCR failed for request {Id}", bloodRequestId);
            }
        }
    }
}