using BloodDonation.Application.DTOs;
using BloodDonation.Application.Interfaces;
using BloodDonation.Domain.Entities;
using BloodDonation.Domain.Enums;
using BloodDonation.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BloodDonation.Infrastructure.Services;

public sealed class DonorMatchingService : IDonorMatchingService
{
    private readonly AppDbContext _context;

    public DonorMatchingService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<AvailableDonorDto>> GetAvailableDonorsAsync(
        BloodRequest bloodRequest,
        CancellationToken cancellationToken)
    {
        var ninetyDaysAgo = DateTime.UtcNow.AddDays(-90);

        var donors = await _context.Users
            .Where(x =>
                x.Role == UserRole.User &&
                x.IsAvailable &&
                x.LastDonationDate < ninetyDaysAgo)
            .ToListAsync(cancellationToken);

        return donors
            .Select(x => new AvailableDonorDto(
                x.Id,
                x.FullName,
                x.BloodType!.Value))
            .ToList();
    }
   
    private static double ToRad(double degrees) => degrees * Math.PI / 180;
}
