using BloodDonation.Application.DTOs;

namespace BloodDonation.Application.Interfaces;

public interface IDonorService
{
    Task<List<DonorNearbyRequestDto>> GetNearbyRequestsAsync(Guid donorId);
    Task UpdateAvailabilityAsync(Guid donorId, bool isAvailable);
    Task<List<DonationHistoryDto>> GetDonationHistoryAsync(Guid donorId);
}
