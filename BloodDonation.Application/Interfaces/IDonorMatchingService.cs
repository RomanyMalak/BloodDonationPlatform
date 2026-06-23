using BloodDonation.Application.DTOs;

namespace BloodDonation.Application.Interfaces;

public interface IDonorMatchingService
{
    Task<DonorMatchingResultDto> FindMatchesAsync(
        Guid bloodRequestId,
        CancellationToken cancellationToken);
}
