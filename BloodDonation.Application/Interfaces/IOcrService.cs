// Application/Interfaces/IOcrService.cs
using BloodDonation.Application.DTOs.Ocr;

namespace BloodDonation.Application.Interfaces;

public interface IOcrService
{
    Task<OcrResultDto> VerifyAsync(
        string? documentUrl,
        string patientName,
        string? hospitalName,
        string bloodType,
        CancellationToken cancellationToken);
}