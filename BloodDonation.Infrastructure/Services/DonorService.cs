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

    private static double ToRad(double deg) => deg * Math.PI / 180;
}
