using BloodDonation.Application.DTOs.Ocr;
using BloodDonation.Application.Interfaces;
using BloodDonation.Domain.Enums;

namespace BloodDonation.Infrastructure.Services;
public class OcrService : IOcrService
{
    public async Task<OcrResultDto> VerifyAsync(Guid bloodRequestId,CancellationToken cancellationToken)
    {

        // هنا بعدين هنجيب الملف من MedicalDocumentUrl
        // ونبعته لـ OCR Provider


        var extractedText =
            "Patient Blood Group: A+ \n" +
            "Units Required: 2 \n" +
            "Emergency surgery required";

        var result = new OcrResultDto
        {
            RawText = extractedText,

            ConfidenceScore = 0.90,

            ExtractedBloodType = ExtractBloodType(extractedText),

            ExtractedUnits = ExtractUnits(extractedText),

            ExtractedUrgency = DetermineUrgency(extractedText)
        };



        // validation

        if (result.ConfidenceScore < 0.75)
        {
            result.IsVerified = false;
            result.FailureReason =
                "Low OCR confidence";

            return result;
        }

        result.IsVerified = true;

        return result;
    }

    private BloodType? ExtractBloodType(string text)
    {
        text = text.ToUpper();

        if (text.Contains("AB+"))
            return BloodType.ABPositive;

        if (text.Contains("AB-"))
            return BloodType.ABNegative;

        if (text.Contains("A+"))
            return BloodType.APositive;

        if (text.Contains("A-"))
            return BloodType.ANegative;

        if (text.Contains("B+"))
            return BloodType.BPositive;

        if (text.Contains("B-"))
            return BloodType.BNegative;

        if (text.Contains("O+"))
            return BloodType.OPositive;

        if (text.Contains("O-"))
            return BloodType.ONegative;


        return null;
    }

    private int? ExtractUnits(string text)
    {

        var number = new string( text.Where(char.IsDigit) .ToArray());

        if (int.TryParse(number, out int units))
            return units;

        return null;
    }
    private RequestUrgency DetermineUrgency(string text)
    {
        text = text.ToLower();

        if (
            text.Contains("emergency") ||
            text.Contains("bleeding") ||
            text.Contains("surgery") ||
            text.Contains("icu")
           )
        {
            return RequestUrgency.Critical;
        }


        if (
            text.Contains("urgent") ||
            text.Contains("as soon as possible")
           )
        {
            return RequestUrgency.Urgent;
        }


        return RequestUrgency.Normal;
    }
}