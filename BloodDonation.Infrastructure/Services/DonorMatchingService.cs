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
    private readonly IDonorQueryService _donorQueryService;
    private readonly IMedicalValidatorAgent _medicalValidatorAgent;

    public DonorMatchingService(AppDbContext context, IDonorQueryService donorQueryService ,IMedicalValidatorAgent medicalValidatorAgent)
    {
        _context = context;
        _donorQueryService = donorQueryService;
        _medicalValidatorAgent = medicalValidatorAgent;
    }

    public async Task<AvailableBloodTypesResponse> GetAvailableBloodTypesAsync(
         Guid bloodRequestId,
         CancellationToken cancellationToken)
    {
        var bloodRequest = await _context.BloodRequests
            .FirstOrDefaultAsync(
                x => x.Id == bloodRequestId,
                cancellationToken);
        if (bloodRequest is null)
            throw new Exception("Blood request not found");

        var nearbyDonors =
          await _donorQueryService.GetNearbyAvailableDonorsAsync(
        bloodRequest,
        cancellationToken);
        

        var ninetyDaysAgo = DateTime.UtcNow.AddDays(-90);
        nearbyDonors = nearbyDonors
            .Where(donor => 
            donor.LastDonationDate == null ||
            donor.LastDonationDate < ninetyDaysAgo)
            .ToList();

        return new AvailableBloodTypesResponse
        {
            PatientBloodType = bloodRequest.RequiredBloodType.ToString(),

            AvailableBloodTypes = nearbyDonors
        .Select(x => x.BloodType!.Value.ToString())
        .Distinct()
        .ToList()
        };
    }

    public async Task<List<AvailableDonorDto>> GetMatchedDonorsAsync(Guid bloodRequestId, CancellationToken cancellationToken)
    {
        var bloodRequest = await _context.BloodRequests
            .FirstOrDefaultAsync(
                x => x.Id == bloodRequestId,
                cancellationToken);
        if (bloodRequest is null)
            throw new Exception("Blood request not found");
        var nearbyDonors = await _donorQueryService.GetNearbyAvailableDonorsAsync(
            bloodRequest,
            cancellationToken);
        var ninetyDaysAgo = DateTime.UtcNow.AddDays(-90);
        nearbyDonors = nearbyDonors
            .Where(donor =>
            donor.LastDonationDate == null ||
            donor.LastDonationDate < ninetyDaysAgo)
            .ToList();
        var availableBloodTypes = nearbyDonors
            .Select(x => x.BloodType!.Value.ToString())
            .Distinct()
            .ToList();
        var compatibleBloodTypes =
    await _medicalValidatorAgent.GetCompatibleBloodTypesAsync(
        bloodRequest.RequiredBloodType.ToString(),
        availableBloodTypes,
        cancellationToken);

        var matchedDonors = nearbyDonors
    .Where(x =>
        compatibleBloodTypes.Contains(
            x.BloodType!.Value.ToString()))
    .Select(x => new AvailableDonorDto
    {
        Id = x.Id,
        FullName = x.FullName,
        BloodType = x.BloodType!.Value.ToString()
    })
    .ToList();
        return matchedDonors;

    }
}
