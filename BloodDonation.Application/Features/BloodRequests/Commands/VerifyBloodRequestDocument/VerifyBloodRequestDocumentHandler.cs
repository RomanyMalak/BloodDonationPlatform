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
    private readonly INotificationAgentQueue _notificationAgentQueue;

    public VerifyBloodRequestDocumentHandler(
        IApplicationDbContext dbContext,
        IOcrService ocrService,
        INotificationService notificationService,
        INotificationAgentQueue notificationAgentQueue)
    {
        _dbContext = dbContext;
        _ocrService = ocrService;
        _notificationService = notificationService;
        _notificationAgentQueue = notificationAgentQueue;
    }



    public async Task<OcrResultDto> Handle(VerifyBloodRequestDocumentCommand request,CancellationToken cancellationToken)
    {
        // ١. جيب الطلب مع بيانات المستشفى
        var bloodRequest = await _dbContext.BloodRequests
            .Include(x => x.Hospital)
            .FirstOrDefaultAsync(
                x => x.Id == request.BloodRequestId,
                cancellationToken);

        if (bloodRequest is null)
            throw new Exception("Blood request not found");

        //OCR ٢. جهز البيانات للـ 
        var hospitalName =
            bloodRequest.Hospital?.Name ??
            bloodRequest.CustomHospitalName;

        //مع بيانات الطلبOCR ٣. شغل الـ  
        var result = await _ocrService.VerifyAsync(
            bloodRequest.MedicalDocumentUrl,
            bloodRequest.PatientName,       
            hospitalName,                     
            bloodRequest.RequiredBloodType.ToString(), 
            cancellationToken);

        //OCR ٤. احفظ نتيجة الـ 
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
            .AddAsync(verification, cancellationToken);

        // Status + Urgency ٥. غير 
        if (result.IsVerified)
        {
            bloodRequest.Status = RequestStatus.Matching;

            if (result.ExtractedUrgency.HasValue &&
                (int)result.ExtractedUrgency.Value >
                (int)bloodRequest.Urgency)
            {
                bloodRequest.Urgency = result.ExtractedUrgency.Value;
            }

            await _notificationService.CreateAsync(
                bloodRequest.CreatedByUserId,
                "تم قبول طلبك",
                "تم التحقق من وثيقتك بنجاح.",
                bloodRequest.Id, "OCR", cancellationToken);
        }
        else
        {
            bloodRequest.Status = RequestStatus.Rejected;

            await _notificationService.CreateAsync(
                bloodRequest.CreatedByUserId,
                "تم رفض طلبك",
                result.FailureReason ?? "Document verification failed.",
                bloodRequest.Id, "OCR", cancellationToken);
        }

        await _dbContext.SaveChangesAsync(cancellationToken);

        if (result.IsVerified)
        {
            await _notificationAgentQueue.EnqueueAsync(
                bloodRequest.Id,
                cancellationToken);
        }

        return result;
    }
}
