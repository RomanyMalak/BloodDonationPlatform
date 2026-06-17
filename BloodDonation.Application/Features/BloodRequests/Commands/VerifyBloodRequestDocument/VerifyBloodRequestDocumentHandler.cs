using BloodDonation.Application.DTOs.Ocr;
using BloodDonation.Application.Interfaces;
using BloodDonation.Domain.Entities;
using BloodDonation.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;


namespace BloodDonation.Application.Features.BloodRequests.Commands.VerifyBloodRequestDocument;


public sealed class VerifyBloodRequestDocumentHandler
    : IRequestHandler<VerifyBloodRequestDocumentCommand, OcrResultDto>
{

    private readonly IApplicationDbContext _dbContext;
    private readonly IOcrService _ocrService;
    private readonly INotificationService _notificationService;


    public VerifyBloodRequestDocumentHandler(
        IApplicationDbContext dbContext,
        IOcrService ocrService,
        INotificationService notificationService)
    {
        _dbContext = dbContext;
        _ocrService = ocrService;
        _notificationService = notificationService;
    }



    public async Task<OcrResultDto> Handle(
        VerifyBloodRequestDocumentCommand request,
        CancellationToken cancellationToken)
    {

        var bloodRequest =
            await _dbContext.BloodRequests
            .FirstOrDefaultAsync(
                x => x.Id == request.BloodRequestId,
                cancellationToken);


        if (bloodRequest is null)
            throw new Exception("Blood request not found");



        var result =
            await _ocrService.VerifyAsync(
                bloodRequest.Id,
                cancellationToken);



        var verification = new OcrVerification
        {
            Id = Guid.NewGuid(),

            BloodRequestId = bloodRequest.Id,

            IsVerified = result.IsVerified,

            ConfidenceScore = result.ConfidenceScore,

            RawExtractedText = result.RawText,

            FailureReason = result.FailureReason,

            ExtractedBloodType = result.ExtractedBloodType,

            ExtractedUnits = result.ExtractedUnits,

            ExtractedUrgency = result.ExtractedUrgency,

            VerifiedAt = DateTime.UtcNow
        };


        await _dbContext.OcrVerifications
            .AddAsync(
                verification,
                cancellationToken);



        if (result.IsVerified)
        {
            bloodRequest.Status = RequestStatus.Approved;


            // رفع مستوى الخطورة فقط لو الـ OCR اكتشف حالة أخطر
            if (result.ExtractedUrgency.HasValue &&
                (int)result.ExtractedUrgency.Value >
                (int)bloodRequest.Urgency)
            {
                bloodRequest.Urgency =
                    result.ExtractedUrgency.Value;
            }


            await _notificationService.CreateAsync(
                bloodRequest.CreatedByUserId,
                "Request Verified",
                "Your blood request document has been approved.",
                bloodRequest.Id,
                "OCR",
                cancellationToken);
        }
        else
        {
            bloodRequest.Status =
                RequestStatus.Rejected;


            await _notificationService.CreateAsync(
                bloodRequest.CreatedByUserId,
                "Request Rejected",
                result.FailureReason ??
                "Document verification failed.",
                bloodRequest.Id,
                "OCR",
                cancellationToken);
        }



        await _dbContext.SaveChangesAsync(
            cancellationToken);



        return result;
    }
}