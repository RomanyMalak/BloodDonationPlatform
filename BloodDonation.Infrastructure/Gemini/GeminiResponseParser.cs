using System.Text.Json;
using BloodDonation.Application.DTOs.Ocr;
using BloodDonation.Domain.Enums;

namespace BloodDonation.Infrastructure.Gemini;


public class GeminiResponseParser
{

    private const double MinimumConfidence = 0.7;



    public OcrResultDto Parse(
        string response,
        string expectedBloodType)
    {

        var text =
        ExtractText(response);


        text = text
        .Replace("```json", "")
        .Replace("```", "")
        .Trim();


        using var doc =
        JsonDocument.Parse(text);


        var root = doc.RootElement;



        var confidence =
        root.GetProperty("confidence")
        .GetDouble();



        var valid =
        root.GetProperty("isValidMedicalDocument")
        .GetBoolean();


        var blood =
        root.TryGetProperty("bloodType", out var b)
        ?
        b.GetString()
        :
        null;



        if (!valid || confidence < MinimumConfidence)
        {
            return Failed(
            "Invalid medical document",
            text,
            confidence);
        }



        if (blood != null &&
        NormalizeBlood(blood)
        != NormalizeBlood(expectedBloodType))
        {

            return Failed(
            "Blood type mismatch",
            text,
            confidence);

        }



        return new OcrResultDto
        {
            IsVerified = true,
            RawText = text,
            ConfidenceScore = confidence,
            ExtractedBloodType =
        ParseBlood(blood)
        };

    }




    private string ExtractText(string response)
    {
        using var doc =
        JsonDocument.Parse(response);


        return doc.RootElement
        .GetProperty("candidates")[0]
        .GetProperty("content")
        .GetProperty("parts")[0]
        .GetProperty("text")
        .GetString()
        ?? "";
    }




    private static string NormalizeBlood(string value)
    {
        return value
        .ToUpper()
        .Replace(" ", "")
        .Replace("POSITIVE", "+")
        .Replace("NEGATIVE", "-");
    }



    private static BloodType? ParseBlood(string? value)
    {
        return value?.ToUpper() switch
        {
            "A+" => BloodType.APositive,
            "A-" => BloodType.ANegative,
            "B+" => BloodType.BPositive,
            "B-" => BloodType.BNegative,
            "AB+" => BloodType.ABPositive,
            "AB-" => BloodType.ABNegative,
            "O+" => BloodType.OPositive,
            "O-" => BloodType.ONegative,
            _ => null
        };
    }



    private OcrResultDto Failed(
    string reason,
    string text,
    double confidence)
    {
        return new OcrResultDto
        {
            IsVerified = false,
            FailureReason = reason,
            RawText = text,
            ConfidenceScore = confidence
        };
    }


}