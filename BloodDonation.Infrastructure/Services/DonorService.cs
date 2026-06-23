using BloodDonation.Application.DTOs;
using BloodDonation.Application.Interfaces;

using BloodDonation.Domain.Enums;
using BloodDonation.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BloodDonation.Infrastructure.Services;

public class DonorService : IDonorService
{
    private readonly AppDbContext _context;

    public DonorService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<DonorNearbyRequestDto>> GetNearbyRequestsAsync(Guid donorId)
    {
        var donor = await _context.Users.FindAsync(donorId);
        if (donor == null || donor.Latitude == null || donor.Longitude == null)
        {
            return new List<DonorNearbyRequestDto>();
        }

        var donorLatitude = donor.Latitude.Value;
        var donorLongitude = donor.Longitude.Value;

        var requests = await _context.BloodRequests
            .Include(r => r.Hospital)
            .Where(r => r.Status == RequestStatus.Matching)
            .ToListAsync();

        var result = requests.Select(r =>
        {
            // Haversine formula to calculate distance
            var distanceKm = CalculateDistance(donorLatitude, donorLongitude, r.Latitude, r.Longitude);
            return new DonorNearbyRequestDto
            {
                Id = r.Id,
                RequiredBloodType = r.RequiredBloodType.ToString(),
                Urgency = r.Urgency.ToString(),
                Status = r.Status.ToString(),
                HospitalName = r.Hospital?.Name ?? r.CustomHospitalName ?? "غير محدد",
                DistanceKm = Math.Round(distanceKm, 2),
                CreatedAt = r.CreatedAt
            };
        })
        .Where(r => r.DistanceKm <= 50)
        .OrderBy(r => r.DistanceKm)
        .ToList();

        return result;
    }

    public async Task UpdateAvailabilityAsync(Guid donorId, bool isAvailable)
    {
        var donor = await _context.Users.FindAsync(donorId);
        if (donor == null) return;

        donor.IsAvailable = isAvailable;
        await _context.SaveChangesAsync();
    }

    public async Task<List<DonationHistoryDto>> GetDonationHistoryAsync(Guid donorId)
    {
        return await _context.DonationHistories
            .Where(d => d.DonorId == donorId)
            .OrderByDescending(d => d.DonationDate)
            .Select(d => new DonationHistoryDto
            {
                Id = d.Id,
                HospitalName = d.HospitalName,
                DonationDate = d.DonationDate,
                Notes = d.Notes
            })
            .ToListAsync();
    }

    private static double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
    {
        const double R = 6371; // km
        var dLat = ToRad(lat2 - lat1);
        var dLon = ToRad(lon2 - lon1);
        var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos(ToRad(lat1)) * Math.Cos(ToRad(lat2)) *
                Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        return R * c;
    }

    // This method returns a list of eligible donors based on the required blood type.
    public async Task<List<EligibleDonorDto>> GetEligibleDonorsAsync(BloodType bloodType)
    {
        var compatibilityMatrix = new Dictionary<BloodType, List<BloodType>>
    {
        { BloodType.APositive,  new() { BloodType.APositive, BloodType.ANegative, BloodType.OPositive, BloodType.ONegative } },
        { BloodType.ANegative,  new() { BloodType.ANegative, BloodType.ONegative } },
        { BloodType.BPositive,  new() { BloodType.BPositive, BloodType.BNegative, BloodType.OPositive, BloodType.ONegative } },
        { BloodType.BNegative,  new() { BloodType.BNegative, BloodType.ONegative } },
        { BloodType.ABPositive, new() { BloodType.APositive, BloodType.ANegative, BloodType.BPositive, BloodType.BNegative,
                                        BloodType.ABPositive, BloodType.ABNegative, BloodType.OPositive, BloodType.ONegative } },
        { BloodType.ABNegative, new() { BloodType.ANegative, BloodType.BNegative, BloodType.ABNegative, BloodType.ONegative } },
        { BloodType.OPositive,  new() { BloodType.OPositive, BloodType.ONegative } },
        { BloodType.ONegative,  new() { BloodType.ONegative } }
    };

        var compatibleTypes = compatibilityMatrix[bloodType];
        var cutoffDate = DateTime.UtcNow.AddDays(-90);

        return await _context.Users
            .Where(u =>
                u.BloodType.HasValue &&
                compatibleTypes.Contains(u.BloodType.Value) &&
                u.IsAvailable == true &&
                u.Role == UserRole.User &&
                (u.LastDonationDate == null || u.LastDonationDate <= cutoffDate))
            .Select(u => new EligibleDonorDto
            {
                Id = u.Id,
                FullName = u.FullName,
                BloodType = u.BloodType.ToString(),
                LastDonationDate = u.LastDonationDate,
                Latitude = u.Latitude,
                Longitude = u.Longitude,
                Phone = u.Phone
            })
            .ToListAsync();
    }

    public async Task<List<DonorForValidationDto>> GetAvailableDonorsAsync()
    {
        var cutoffDate = DateTime.UtcNow.AddDays(-90);

        return await _context.Users
            .Where(u =>
                u.Role == UserRole.User &&
                u.IsAvailable == true &&
                u.BloodType.HasValue &&
                (u.LastDonationDate == null || u.LastDonationDate <= cutoffDate))
            .Select(u => new DonorForValidationDto
            {
                Id = u.Id,
                BloodType = u.BloodType.ToString()
            })
            .ToListAsync();
    }
    private static double ToRad(double deg) => deg * Math.PI / 180;
}
