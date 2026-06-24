using BloodDonation.Application.DTOs;
using BloodDonation.Domain.Entities;

namespace BloodDonation.Application.Interfaces;

public interface IDonorMatchingService
{
    Task<List<AvailableDonorDto>> GetAvailableDonorsAsync(
         BloodRequest bloodRequest,
         CancellationToken cancellationToken);
}
