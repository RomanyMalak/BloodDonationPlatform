namespace BloodDonation.Application.Interfaces;

public interface IAiMatchingPipelineService
{
    Task RunAsync(Guid bloodRequestId, CancellationToken cancellationToken);
}