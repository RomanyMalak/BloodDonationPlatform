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
    private readonly IDonorMatchingService _donorMatchingService;
    private readonly IWhatsAppService _whatsAppService;

    public VerifyBloodRequestDocumentHandler(
        IApplicationDbContext dbContext,
        IOcrService ocrService,
        INotificationService notificationService,
        IDonorMatchingService donorMatchingService,
        IWhatsAppService whatsAppService)
    {
        _dbContext = dbContext;
        _ocrService = ocrService;
        _notificationService = notificationService;
        _donorMatchingService = donorMatchingService;
        _whatsAppService = whatsAppService;
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

            var matchedDonors =
            await _donorMatchingService.GetMatchedDonorsAsync(
            bloodRequest.Id,
            cancellationToken);

            foreach (var donor in matchedDonors)
            {
                await _notificationService.CreateAsync(
                    donor.Id,
                    "Urgent Blood Donation Request",
                    $"A nearby patient needs {bloodRequest.RequiredBloodType} blood.",
                    bloodRequest.Id,
                    "BloodRequest",
                    cancellationToken);

                await _whatsAppService.SendAsync(
                    donor.Phone,
                    BuildBloodDonationAlertMessage(
                        bloodRequest.RequiredBloodType.ToString(),
                        bloodRequest.Hospital?.Name ?? bloodRequest.CustomHospitalName ?? "Unknown"),
                    cancellationToken);
            }

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

        return result;
    }

    private static string BuildBloodDonationAlertMessage(string bloodType, string hospitalName)
    {
        return $"""
            🩸 Blood Donation Alert

            A nearby patient urgently needs blood.

            Blood Type: {bloodType}
            Hospital: {hospitalName}

            Please open the application to respond.

            Thank you for helping save lives ❤️
            """;
    }
}
