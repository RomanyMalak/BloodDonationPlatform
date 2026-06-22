using BloodDonation.Application.DTOs.BloodRequest;
using BloodDonation.Application.Interfaces;
using BloodDonation.Domain.Entities;
using BloodDonation.Domain.Enums;
using BloodDonation.Infrastructure.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BloodDonation.Application.Features.BloodRequests.Commands.CreateBloodRequest;

public sealed class CreateBloodRequestHandler: IRequestHandler<CreateBloodRequestCommand, CreateBloodRequestResponseDto>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IOcrVerificationQueue _ocrQueue;
    private readonly INotificationAgentQueue _notificationAgentQueue;

    public CreateBloodRequestHandler(
        IApplicationDbContext dbContext,
        IOcrVerificationQueue ocrVerificationQueue,
        INotificationAgentQueue notificationAgentQueue)
    {
        _dbContext = dbContext;
        _ocrQueue = ocrVerificationQueue;
    }

    public async Task<CreateBloodRequestResponseDto> Handle(CreateBloodRequestCommand request, CancellationToken cancellationToken)
    {
        bool hospitalCanApprove = false;
        if (request.HospitalId.HasValue)
        {
            hospitalCanApprove = await _dbContext.Hospitals
                .AnyAsync(h =>
                    h.Id == request.HospitalId.Value &&
                    h.IsActive,  
                    cancellationToken);
        }

        var user = await _dbContext.Users.FirstAsync(x => x.Id == request.CreatedByUserId, cancellationToken);
        string savedDbPath = await _fileService.UploadFileAsync(request.MedicalDocumentUrl, "MedicalDocuments", cancellationToken);

        var bloodRequest = new BloodRequest
        {
            Id = Guid.NewGuid(),

            PatientName = request.PatientName ?? user.FullName,
            PatientAge =request.PatientAge ?? user.Age,
            RequiredBloodType = request.RequiredBloodType?? user.BloodType ?? throw new Exception("Blood type is required."),
            Status =RequestStatus.PendingVerification,
            HospitalId = request.HospitalId,
            CustomHospitalName = request.CustomHospitalName,
            Latitude = request.Latitude,
            Longitude = request.Longitude,
            MedicalDocumentUrl = savedDbPath,
            Notes = request.Notes,
            ContactPhone = request.ContactPhone,
            UnitsNeeded = request.UnitsNeeded,
            ExpiresAt = request.ExpiresAt,
            CreatedByUserId = request.CreatedByUserId,
            CreatedAt = DateTime.UtcNow
        };

        await _dbContext.BloodRequests.AddAsync(bloodRequest, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
        await _notificationAgentQueue.EnqueueAsync(bloodRequest.Id, cancellationToken);

        if(!hospitalCanApprove)
        {
            await _ocrQueue.EnqueueAsync(bloodRequest.Id, cancellationToken);

        }
        var message = hospitalCanApprove
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
