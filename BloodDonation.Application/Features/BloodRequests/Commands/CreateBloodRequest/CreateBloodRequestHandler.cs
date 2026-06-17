using BloodDonation.Application.DTOs.BloodRequest;
using BloodDonation.Application.Interfaces;
using BloodDonation.Domain.Entities;
using BloodDonation.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BloodDonation.Application.Features.BloodRequests.Commands.CreateBloodRequest;

public sealed class CreateBloodRequestHandler
    : IRequestHandler<CreateBloodRequestCommand, CreateBloodRequestResponseDto>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IOcrVerificationQueue _ocrQueue;
    public CreateBloodRequestHandler(IApplicationDbContext dbContext,IOcrVerificationQueue ocrVerificationQueue)
    {
        _dbContext = dbContext;
        _ocrQueue = ocrVerificationQueue;
    }

    public async Task<CreateBloodRequestResponseDto> Handle(
        CreateBloodRequestCommand request,
        CancellationToken cancellationToken)
    {
        bool hospitalIsRegistered = false;
        if(request.HospitalId.HasValue)
        {
            hospitalIsRegistered = await _dbContext.Hospitals.AnyAsync(h => h.Id == request.HospitalId.Value, cancellationToken);

        }
        var user = await _dbContext.Users.FirstAsync( x => x.Id == request.CreatedByUserId,cancellationToken);

        bool hospitalIsActive = false;
        if (request.HospitalId.HasValue)
        {
            hospitalIsActive = await _dbContext.Hospitals.AnyAsync(
                    h => h.Id == request.HospitalId.Value && h.IsActive,cancellationToken);
        }


        var bloodRequest = new BloodRequest
        {
            Id = Guid.NewGuid(),

            PatientName = request.PatientName ?? user.FullName,
            PatientAge =request.PatientAge ?? user.Age,
            RequiredBloodType = request.RequiredBloodType ?? user.BloodType!.Value,
            Status =RequestStatus.PendingVerification,
            HospitalId = request.HospitalId,
            CustomHospitalName = request.CustomHospitalName,
            Latitude = request.Latitude,
            Longitude = request.Longitude,
            MedicalDocumentUrl = request.MedicalDocumentUrl,
            Notes = request.Notes,
            ContactPhone = request.ContactPhone,
            UnitsNeeded = request.UnitsNeeded,
            ExpiresAt = request.ExpiresAt,
            CreatedByUserId = request.CreatedByUserId,
            CreatedAt = DateTime.UtcNow
        };

        await _dbContext.BloodRequests.AddAsync(bloodRequest, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        if(!hospitalIsActive)
        {
            await _ocrQueue.EnqueueAsync(bloodRequest.Id, cancellationToken);

        }
        var message = hospitalIsRegistered
        ? "Request submitted. Waiting for hospital approval."
        : "Request submitted. Document verification in progress.";

        return new CreateBloodRequestResponseDto
        {
            Id = bloodRequest.Id,
            Status = bloodRequest.Status.ToString(),
            CreatedAt = bloodRequest.CreatedAt,
            Message = message
        };
    }
}