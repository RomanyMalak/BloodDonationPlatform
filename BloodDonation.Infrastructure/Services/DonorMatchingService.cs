using BloodDonation.Application.DTOs;
using BloodDonation.Application.Interfaces;
using BloodDonation.Domain.Entities;
using BloodDonation.Domain.Enums;
using BloodDonation.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BloodDonation.Infrastructure.Services;

public sealed class DonorMatchingService : IDonorMatchingService
{
    private const double SearchRadiusKm = 50;
    private const string AgentNotificationType = "AgentMatching";
    private readonly AppDbContext _context;

    public DonorMatchingService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<DonorMatchingResultDto> FindMatchesAsync(
        Guid bloodRequestId,
        CancellationToken cancellationToken)
    {
        var bloodRequest = await _context.BloodRequests
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == bloodRequestId, cancellationToken);

        if (bloodRequest is null)
        {
            throw new KeyNotFoundException("Blood request not found.");
        }

        var compatibleBloodTypes = GetCompatibleDonorBloodTypes(bloodRequest.RequiredBloodType);

        var acceptedDonorIds = await _context.BloodRequestAcceptances
            .AsNoTracking()
            .Where(x => x.BloodRequestId == bloodRequest.Id)
            .Select(x => x.DonorId)
            .ToListAsync(cancellationToken);

        var alreadyNotifiedDonorIds = await _context.Notifications
            .AsNoTracking()
            .Where(x =>
                x.BloodRequestId == bloodRequest.Id &&
                x.Type == AgentNotificationType)
            .Select(x => x.UserId)
            .ToListAsync(cancellationToken);

        var donors = await _context.Users
            .AsNoTracking()
            .Where(x => x.Role == UserRole.User)
            .OrderBy(x => x.FullName)
            .ToListAsync(cancellationToken);

        var decisions = donors
            .Select(donor => EvaluateDonor(
                donor.Id,
                donor.FullName,
                donor.BloodType,
                donor.Latitude,
                donor.Longitude,
                donor.IsAvailable,
                bloodRequest,
                compatibleBloodTypes,
                acceptedDonorIds,
                alreadyNotifiedDonorIds))
            .ToList();

        return new DonorMatchingResultDto
        {
            BloodRequestId = bloodRequest.Id,
            SearchRadiusKm = SearchRadiusKm,
            CompatibleBloodTypes = compatibleBloodTypes,
            Decisions = decisions
        };
    }

    private static DonorMatchDecisionDto EvaluateDonor(
        Guid donorId,
        string donorName,
        BloodType? donorBloodType,
        double? donorLatitude,
        double? donorLongitude,
        bool isAvailable,
        BloodRequest bloodRequest,
        IReadOnlyCollection<BloodType> compatibleBloodTypes,
        IReadOnlyCollection<Guid> acceptedDonorIds,
        IReadOnlyCollection<Guid> alreadyNotifiedDonorIds)
    {
        double? distanceKm = null;

        if (donorLatitude.HasValue && donorLongitude.HasValue)
        {
            distanceKm = CalculateDistance(
                donorLatitude.Value,
                donorLongitude.Value,
                bloodRequest.Latitude,
                bloodRequest.Longitude);
        }

        if (donorId == bloodRequest.CreatedByUserId)
        {
            return Skipped(donorId, donorName, donorBloodType, distanceKm, "Skipped: donor created this blood request.");
        }

        if (!isAvailable)
        {
            return Skipped(donorId, donorName, donorBloodType, distanceKm, "Skipped: donor is not available.");
        }

        if (!donorBloodType.HasValue)
        {
            return Skipped(donorId, donorName, donorBloodType, distanceKm, "Skipped: donor blood type is missing.");
        }

        if (!compatibleBloodTypes.Contains(donorBloodType.Value))
        {
            return Skipped(
                donorId,
                donorName,
                donorBloodType,
                distanceKm,
                $"Skipped: donor blood type {donorBloodType.Value} is not compatible with {bloodRequest.RequiredBloodType}.");
        }

        if (!distanceKm.HasValue)
        {
            return Skipped(donorId, donorName, donorBloodType, distanceKm, "Skipped: donor location is missing.");
        }

        if (distanceKm.Value > SearchRadiusKm)
        {
            return Skipped(
                donorId,
                donorName,
                donorBloodType,
                distanceKm,
                $"Skipped: donor is {distanceKm.Value:0.00} km away, outside the {SearchRadiusKm:0} km radius.");
        }

        if (acceptedDonorIds.Contains(donorId))
        {
            return Skipped(donorId, donorName, donorBloodType, distanceKm, "Skipped: donor already accepted this request.");
        }

        if (alreadyNotifiedDonorIds.Contains(donorId))
        {
            return Skipped(donorId, donorName, donorBloodType, distanceKm, "Skipped: donor already has an agent notification for this request.");
        }

        return new DonorMatchDecisionDto
        {
            DonorId = donorId,
            DonorName = donorName,
            BloodType = donorBloodType,
            DistanceKm = Math.Round(distanceKm.Value, 2),
            IsMatched = true,
            Reason = $"Matched: blood type {donorBloodType.Value} is compatible and donor is {distanceKm.Value:0.00} km away."
        };
    }

    private static DonorMatchDecisionDto Skipped(
        Guid donorId,
        string donorName,
        BloodType? bloodType,
        double? distanceKm,
        string reason)
    {
        return new DonorMatchDecisionDto
        {
            DonorId = donorId,
            DonorName = donorName,
            BloodType = bloodType,
            DistanceKm = distanceKm.HasValue ? Math.Round(distanceKm.Value, 2) : null,
            IsMatched = false,
            Reason = reason
        };
    }

    private static IReadOnlyCollection<BloodType> GetCompatibleDonorBloodTypes(BloodType requiredBloodType)
    {
        return requiredBloodType switch
        {
            BloodType.ONegative => new[] { BloodType.ONegative },
            BloodType.OPositive => new[] { BloodType.ONegative, BloodType.OPositive },
            BloodType.ANegative => new[] { BloodType.ONegative, BloodType.ANegative },
            BloodType.APositive => new[] { BloodType.ONegative, BloodType.OPositive, BloodType.ANegative, BloodType.APositive },
            BloodType.BNegative => new[] { BloodType.ONegative, BloodType.BNegative },
            BloodType.BPositive => new[] { BloodType.ONegative, BloodType.OPositive, BloodType.BNegative, BloodType.BPositive },
            BloodType.ABNegative => new[] { BloodType.ONegative, BloodType.ANegative, BloodType.BNegative, BloodType.ABNegative },
            BloodType.ABPositive => Enum.GetValues<BloodType>(),
            _ => Array.Empty<BloodType>()
        };
    }

    private static double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
    {
        const double earthRadiusKm = 6371;
        var dLat = ToRad(lat2 - lat1);
        var dLon = ToRad(lon2 - lon1);
        var a =
            Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
            Math.Cos(ToRad(lat1)) * Math.Cos(ToRad(lat2)) *
            Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        return earthRadiusKm * c;
    }

    private static double ToRad(double degrees) => degrees * Math.PI / 180;
}
