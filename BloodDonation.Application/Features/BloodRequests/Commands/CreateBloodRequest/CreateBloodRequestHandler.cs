using BloodDonation.Application.DTOs.BloodRequest;
using BloodDonation.Application.Exceptions;
using BloodDonation.Application.Interfaces;
using BloodDonation.Domain.Entities;
using BloodDonation.Domain.Enums;
using BloodDonation.Infrastructure.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BloodDonation.Application.Features.BloodRequests.Commands.CreateBloodRequest;

public sealed class CreateBloodRequestHandler : IRequestHandler<CreateBloodRequestCommand, CreateBloodRequestResponseDto>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IOcrVerificationQueue _ocrQueue;
    private readonly IFileService _fileService;

    public CreateBloodRequestHandler(
        IApplicationDbContext dbContext,
        IOcrVerificationQueue ocrVerificationQueue,
        IFileService fileService)
    {
        _dbContext = dbContext;
        _ocrQueue = ocrVerificationQueue;
        _fileService = fileService;
    }

    public async Task<CreateBloodRequestResponseDto> Handle(CreateBloodRequestCommand request, CancellationToken cancellationToken)
    {
        var user = await _dbContext.Users
            .FirstOrDefaultAsync(x => x.Id == request.CreatedByUserId, cancellationToken);

        if (user is null)
            throw new InvalidOperationException("Authenticated user not found.");

        bool hospitalCanApprove = false;
        if (request.HospitalId.HasValue)
        {
            hospitalCanApprove = await _dbContext.Hospitals
                .AnyAsync(h =>
                    h.Id == request.HospitalId.Value &&
                    h.IsActive,
                    cancellationToken);
        }

        string savedDbPath = await _fileService.UploadFileAsync(
            request.MedicalDocumentUrl, "MedicalDocuments", cancellationToken);

        var bloodRequest = new BloodRequest
        {
            Id = Guid.NewGuid(),

            PatientName = request.PatientName ?? user.FullName,
            PatientAge = request.PatientAge ?? user.Age,
            RequiredBloodType = request.RequiredBloodType ?? user.BloodType
                ?? throw new InvalidOperationException("Blood type is required."),
            Status = RequestStatus.PendingVerification,
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

        if (request.HospitalId.HasValue)
        {
            var hospitalExists = await _dbContext.Hospitals
                .AnyAsync(h => h.Id == request.HospitalId.Value, cancellationToken);

            if (!hospitalExists)
            {
                throw new NotFoundException("Hospital not found.");
            }
        }

        await _dbContext.BloodRequests.AddAsync(bloodRequest, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        if (!hospitalCanApprove)
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