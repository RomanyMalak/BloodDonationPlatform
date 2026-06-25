using BloodDonation.Application.DTOs;
using BloodDonation.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BloodDonation.Application.Interfaces
{
    public interface IDonorQueryService
    {
        Task<List<AvailableDonorDto>> GetAvailableDonorsAsync(
        CancellationToken cancellationToken);
        Task<List<User>> GetNearbyAvailableDonorsAsync(
    BloodRequest bloodRequest,
    CancellationToken cancellationToken);
    }
}
