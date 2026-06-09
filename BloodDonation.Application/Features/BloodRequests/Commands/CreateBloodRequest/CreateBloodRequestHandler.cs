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
    public CreateBloodRequestHandler(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
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
        var bloodRequest = new BloodRequest
        {
            Id = Guid.NewGuid(),

            PatientName = request.PatientName,

            PatientAge = request.PatientAge,

            RequiredBloodType = request.RequiredBloodType,

            Urgency = request.Urgency,

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

            CreatedAt = DateTime.UtcNow,

            Status = RequestStatus.PendingVerification
        };

        await _dbContext.BloodRequests.AddAsync(bloodRequest, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        if(!hospitalIsRegistered)
        {
            //ocr verifcation

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