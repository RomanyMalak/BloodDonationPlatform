using BloodDonation.Application.Interfaces;
using BloodDonation.Domain.Entities;
using BloodDonation.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BloodDonation.Application.Features.BloodRequests.Commands.AcceptBloodRequest;


public sealed class AcceptBloodRequestCommandHandler
    : IRequestHandler<AcceptBloodRequestCommand, bool>
{

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
            .Include(x => x.Acceptances)
            .FirstOrDefaultAsync(x => x.Id == request.BloodRequestId,cancellationToken);


        if (bloodRequest is null)
            return false;

        if (bloodRequest.Status != RequestStatus.Approved &&
            bloodRequest.Status != RequestStatus.Matching)
            return false;

        var donor = await _dbContext.Users
            .FirstOrDefaultAsync(x => x.Id == request.DonorId, cancellationToken);

        if (donor is null ||
            donor.Role != UserRole.User ||
            !donor.IsAvailable ||
            donor.Id == bloodRequest.CreatedByUserId ||
            !donor.BloodType.HasValue ||
            !IsCompatibleDonor(bloodRequest.RequiredBloodType, donor.BloodType.Value) ||
            !donor.Latitude.HasValue ||
            !donor.Longitude.HasValue ||
            CalculateDistance(
                donor.Latitude.Value,
                donor.Longitude.Value,
                bloodRequest.Latitude,
                bloodRequest.Longitude) > 50)
        {
            return false;
        }

        var alreadyAccepted =bloodRequest.Acceptances.Any(x =>x.DonorId == request.DonorId);

        if (alreadyAccepted)
            return false;

        var acceptance = new BloodRequestAcceptance
        {
            Id = Guid.NewGuid(),
            BloodRequestId = bloodRequest.Id,
            DonorId = request.DonorId,
            AcceptedAt = DateTime.UtcNow,
            Status = AcceptanceStatus.Accepted
        };

        await _dbContext.BloodRequestAcceptances.AddAsync(acceptance, cancellationToken);

        var acceptedCount =bloodRequest.Acceptances.Count + 1;

        if (acceptedCount >= bloodRequest.UnitsNeeded)
        {
            bloodRequest.Status =RequestStatus.Accepted;
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
