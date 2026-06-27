using BloodDonation.Application.DTOs;
using BloodDonation.Domain.Enums;

namespace BloodDonation.Application.Interfaces;

public interface IDonorService
{
    Task<List<DonorNearbyRequestDto>> GetNearbyRequestsAsync(Guid donorId);
    Task UpdateAvailabilityAsync(Guid donorId, bool isAvailable);
    Task<List<DonationHistoryDto>> GetDonationHistoryAsync(Guid donorId);
   

}
