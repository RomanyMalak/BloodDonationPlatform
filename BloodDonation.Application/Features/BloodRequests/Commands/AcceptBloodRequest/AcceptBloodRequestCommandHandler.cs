using BloodDonation.Application.Exceptions;
using BloodDonation.Application.Interfaces;
using BloodDonation.Domain.Entities;
using BloodDonation.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BloodDonation.Application.Features.BloodRequests.Commands.AcceptBloodRequest;


public sealed class AcceptBloodRequestCommandHandler
    : IRequestHandler<AcceptBloodRequestCommand, bool>
{
    private const double SearchRadiusKm = 50;

    private readonly IApplicationDbContext _dbContext;
    private readonly INotificationService _notificationService;

    public AcceptBloodRequestCommandHandler(
        IApplicationDbContext dbContext,
        INotificationService notificationService)
    {
        _dbContext = dbContext;
        _notificationService = notificationService;
    }

    public async Task<bool> Handle(
        AcceptBloodRequestCommand request,
        CancellationToken cancellationToken)
    {
        var bloodRequest =
            await _dbContext.BloodRequests
            .FirstOrDefaultAsync(x => x.Id == request.BloodRequestId, cancellationToken);

        if (bloodRequest is null)
            throw new NotFoundException("Blood request not found.");

        if (bloodRequest.Status != RequestStatus.Matching)
        {
            throw new ConflictException(
                $"This request cannot be accepted because its status is '{bloodRequest.Status}'.");
        }

        var donor = await _dbContext.Users
            .FirstOrDefaultAsync(x => x.Id == request.DonorId, cancellationToken);

        if (donor is null)
            throw new NotFoundException("Donor not found.");

        if (donor.Role != UserRole.User)
            throw new ConflictException("Only users with the donor role can accept blood requests.");

        if (donor.Id == bloodRequest.CreatedByUserId)
            throw new ConflictException("You cannot accept a blood request you created yourself.");

        if (!donor.IsAvailable)
            throw new ConflictException("You are currently marked as unavailable. Update your availability before accepting requests.");

        if (!donor.BloodType.HasValue)
            throw new ConflictException("Your blood type is not set. Please update your profile.");

        if (!IsCompatibleDonor(bloodRequest.RequiredBloodType, donor.BloodType.Value))
        {
            throw new ConflictException(
                $"Your blood type ({donor.BloodType.Value}) is not compatible with the requested type ({bloodRequest.RequiredBloodType}).");
        }

        if (!donor.Latitude.HasValue || !donor.Longitude.HasValue)
            throw new ConflictException("Your location is not set. Please update your profile.");

        var distanceKm = CalculateDistance(
            donor.Latitude.Value,
            donor.Longitude.Value,
            bloodRequest.Latitude,
            bloodRequest.Longitude);

        if (distanceKm > SearchRadiusKm)
        {
            throw new ConflictException(
                $"You are {distanceKm:0.0} km away, which is outside the {SearchRadiusKm:0} km matching radius for this request.");
        }

        var alreadyAccepted = await _dbContext.BloodRequestAcceptances
    .AnyAsync(
        x => x.BloodRequestId == bloodRequest.Id &&
             x.DonorId == request.DonorId,
        cancellationToken);

        if (alreadyAccepted)
            throw new ConflictException("You have already accepted this request.");


        var acceptance = new BloodRequestAcceptance
        {
            Id = Guid.NewGuid(),
            BloodRequestId = bloodRequest.Id,
            DonorId = request.DonorId,
            AcceptedAt = DateTime.UtcNow,
            Status = AcceptanceStatus.Accepted
        };

        await _dbContext.BloodRequestAcceptances.AddAsync(
            acceptance,
            cancellationToken);


        var acceptedCount = await _dbContext.BloodRequestAcceptances
            .CountAsync(
                x => x.BloodRequestId == bloodRequest.Id,
                cancellationToken) + 1;


        if (acceptedCount >= bloodRequest.UnitsNeeded)
        {
            bloodRequest.Status = RequestStatus.Accepted;
        }

        await _dbContext.SaveChangesAsync(cancellationToken);

        await _notificationService.CreateAsync(
            bloodRequest.CreatedByUserId,
            "Donor Accepted Your Request",
            "A donor has accepted your blood request.",
            bloodRequest.Id,
            "Acceptance",
            cancellationToken);

        return true;
    }

    private static bool IsCompatibleDonor(BloodType requiredBloodType, BloodType donorBloodType)
    {
        return requiredBloodType switch
        {
            BloodType.ONegative => donorBloodType == BloodType.ONegative,
            BloodType.OPositive => donorBloodType is BloodType.ONegative or BloodType.OPositive,
            BloodType.ANegative => donorBloodType is BloodType.ONegative or BloodType.ANegative,
            BloodType.APositive => donorBloodType is BloodType.ONegative or BloodType.OPositive or BloodType.ANegative or BloodType.APositive,
            BloodType.BNegative => donorBloodType is BloodType.ONegative or BloodType.BNegative,
            BloodType.BPositive => donorBloodType is BloodType.ONegative or BloodType.OPositive or BloodType.BNegative or BloodType.BPositive,
            BloodType.ABNegative => donorBloodType is BloodType.ONegative or BloodType.ANegative or BloodType.BNegative or BloodType.ABNegative,
            BloodType.ABPositive => true,
            _ => false
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