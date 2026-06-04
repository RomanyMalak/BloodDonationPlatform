using BloodDonation.Application.DTOs.BloodRequest;
using BloodDonation.Application.Interfaces;
using BloodDonation.Domain.Entities;
using BloodDonation.Domain.Enums;
using MediatR;

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
        BloodRequest bloodRequest = new BloodRequest
        {
            Id = Guid.NewGuid(),
            RequiredBloodType = request.RequiredBloodType,
            Urgency = request.Urgency,
            Status = RequestStatus.PendingVerification, 
            HospitalId = request.HospitalId,
            CustomHospitalName = request.CustomHospitalName,
            Latitude = request.Latitude,
            Longitude = request.Longitude,
            MedicalDocumentUrl = request.MedicalDocumentUrl,
            Notes = request.Notes,
            ContactPhone = request.ContactPhone,
            UnitsNeeded = request.UnitsNeeded,
            ExpiresAt = request.ExpiresAt,
            PatientId = request.PatientId,
            CreatedAt = DateTime.UtcNow
        };

        await _dbContext.BloodRequests.AddAsync(bloodRequest, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return new CreateBloodRequestResponseDto
        {
            Id = bloodRequest.Id,
            Status = bloodRequest.Status.ToString(),
            CreatedAt = bloodRequest.CreatedAt,
            Message = "Request submitted successfully. Pending document verification."
        };
    }
}