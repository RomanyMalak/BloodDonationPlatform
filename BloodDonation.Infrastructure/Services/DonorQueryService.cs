using BloodDonation.Application.DTOs;
using BloodDonation.Application.Interfaces;
using BloodDonation.Domain.Entities;
using BloodDonation.Domain.Enums;
using BloodDonation.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;


namespace BloodDonation.Infrastructure.Services
{
    public class DonorQueryService : IDonorQueryService

    {
        private readonly AppDbContext _context;
       

        public DonorQueryService(AppDbContext context)
        {
            _context = context;
           
        }
        private IQueryable<User> GetAvailableDonorsQuery()
        {
            return _context.Users
                .Where(x =>
                    x.Role == UserRole.User &&
                    x.IsAvailable &&
                    x.BloodType != null);
        }
        public async Task<List<AvailableDonorDto>> GetAvailableDonorsAsync(
    CancellationToken cancellationToken)
        {
            return await GetAvailableDonorsQuery()
                .Select(x => new AvailableDonorDto
                {
                    Id = x.Id,
                    FullName = x.FullName,
                    BloodType = x.BloodType!.Value.ToString()
                })
                .ToListAsync(cancellationToken);
        }

        public async Task<List<User>> GetNearbyAvailableDonorsAsync(
            BloodRequest bloodRequest,
            CancellationToken cancellationToken)
        {

            if (bloodRequest.Latitude == 0 || bloodRequest.Longitude == 0)
            {
                return new List<User>();
            }
            var donors = await GetAvailableDonorsQuery()
                .ToListAsync(cancellationToken);

            return donors
                .Where(d =>
                {
                    if (d.Latitude == null || d.Longitude == null)
                        return false;
                    var distance = CalculateDistance(
                        bloodRequest.Latitude,
                        bloodRequest.Longitude,
                        d.Latitude.Value,
                        d.Longitude.Value);
                    return distance <= 50;
                })
                .ToList();
        }
        public static double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
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
}
