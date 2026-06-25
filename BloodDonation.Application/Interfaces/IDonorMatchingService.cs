using BloodDonation.Application.DTOs;
using BloodDonation.Domain.Entities;

namespace BloodDonation.Application.Interfaces;

public interface IDonorMatchingService
{
     Task<AvailableBloodTypesResponse> GetAvailableBloodTypesAsync(
         Guid bloodRequestId,
         CancellationToken cancellationToken);
    Task<List<AvailableDonorDto>> GetMatchedDonorsAsync(
        Guid bloodRequestId,
        CancellationToken cancellationToken);
}
