using BloodDonation.Application.DTOs.Ocr;
using BloodDonation.Application.Interfaces;
using BloodDonation.Domain.Enums;
using static System.Net.Mime.MediaTypeNames;

public class OcrService : IOcrService
{
    private static readonly string[] MedicalKeywords =
{
    // طبي عام
    "تشخيص", "مستشفى", "طبيب", "دكتور", "مريض",
    "وصفة", "علاج", "جراحة", "عملية", "طوارئ",
    "تحليل", "أشعة", "نتيجة", "تقرير طبي",
    "وحدة دم", "نقل دم", "فصيلة", "بنك الدم",

    //  تخصصات
    "باطنة", "جراحة", "عظام", "قلب", "أورام",
    "نسا وتوليد", "أطفال", "عيون", "كلى",

    // حالة المريض
    "نزيف", "حادث", "إصابة", "فقر دم", "أنيميا",
    "ضغط", "سكر", "فشل كلوي", "زراعة",

    //  طبي عام
    "diagnosis", "hospital", "doctor", "patient",
    "blood", "medical", "report", "prescription",
    "treatment", "surgery", "operation", "emergency",
    "laboratory", "analysis", "result", "clinic",

    //  دم تحديداً
    "blood type", "blood group", "blood bank",
    "transfusion", "units", "hemoglobin",
    "anemia", "hemorrhage", "bleeding",

    //  تخصصات
    "internal medicine", "orthopedic", "cardiology",
    "oncology", "nephrology", "pediatric",

    //  حالة المريض
    "icu", "critical", "urgent", "accident",
    "injury", "trauma", "kidney failure"
};

    public async Task<OcrResultDto> VerifyAsync(
        string? documentUrl,
        string patientName,
        string? hospitalName,
        string bloodType,
        CancellationToken cancellationToken)
    {
        // ١. استخرج النص
        var extractedText = await ExtractTextAsync(documentUrl);

        var result = new OcrResultDto
        {
            RawText = extractedText,
            ConfidenceScore = CalculateScore(extractedText),
            ExtractedBloodType = ExtractBloodType(extractedText),
            ExtractedUnits = ExtractUnits(extractedText),
            ExtractedUrgency = DetermineUrgency(extractedText)
        };

        // ٢. تحقق من الـ Score
        if (result.ConfidenceScore < 0.75)
        {
            result.IsVerified = false;
            result.FailureReason = "Low OCR confidence score.";
            return result;
        }


        int matchCount = MedicalKeywords.Count(k =>
            extractedText.Contains(k, StringComparison.OrdinalIgnoreCase));

        bool hasMedical = matchCount >= 3;
        if (!hasMedical)
        {
            result.IsVerified = false;
            result.FailureReason = "No medical content detected.";
            return result;
        }

        // ٤. تحقق من اسم المريض
        var nameParts = patientName
            .Split(' ', StringSplitOptions.RemoveEmptyEntries)
            .Take(2);

        bool nameFound = nameParts.All(p =>
            extractedText.Contains(p, StringComparison.OrdinalIgnoreCase));

        if (!nameFound)
        {
            result.IsVerified = false;
            result.FailureReason = "Patient name not found in document.";
            return result;
        }

        // ٥. تحقق من المستشفى
        if (hospitalName is not null &&
            !extractedText.Contains(hospitalName, StringComparison.OrdinalIgnoreCase))
        {
            result.IsVerified = false;
            result.FailureReason = "Hospital name not found in document.";
            return result;
        }

        // ٦. تحقق من فصيلة الدم
        if (!extractedText.Contains(bloodType, StringComparison.OrdinalIgnoreCase))
        {
            result.IsVerified = false;
            result.FailureReason = "Blood type mismatch in document.";
            return result;
        }

        result.IsVerified = true;
        return result;
    }

    private Task<string> ExtractTextAsync(string? documentUrl)
    {
        // دلوقتي: نص تجريبي للتست
        // لاحقاً: Tesseract أو Azure Vision
        var fakeText =
            "Patient: محمد علي\n" +
            "Blood Group: A+\n" +
            "Hospital: مستشفى القاهرة\n" +
            "Diagnosis: Emergency surgery required\n" +
            "Units Required: 2";

        return Task.FromResult(fakeText);
    }

    private double CalculateScore(string text)
    {
        if (string.IsNullOrWhiteSpace(text)) return 0;
        var words = text.Split(' ',
            StringSplitOptions.RemoveEmptyEntries).Length;
        return words switch
        {
            > 50 => 0.95,
            > 20 => 0.85,
            > 10 => 0.75,
            _ => 0.40
        };
    }

    private BloodType? ExtractBloodType(string text)
    {
        text = text.ToUpper();
        if (text.Contains("AB+")) return BloodType.ABPositive;
        if (text.Contains("AB-")) return BloodType.ABNegative;
        if (text.Contains("A+")) return BloodType.APositive;
        if (text.Contains("A-")) return BloodType.ANegative;
        if (text.Contains("B+")) return BloodType.BPositive;
        if (text.Contains("B-")) return BloodType.BNegative;
        if (text.Contains("O+")) return BloodType.OPositive;
        if (text.Contains("O-")) return BloodType.ONegative;
        return null;
    }

    private int? ExtractUnits(string text)
    {
        // ✅ بندور على pattern "X units" أو "X وحدة"
        var match = System.Text.RegularExpressions
            .Regex.Match(text, @"(\d+)\s*(units|وحدة|unit)",
                System.Text.RegularExpressions.RegexOptions.IgnoreCase);

        if (match.Success &&
            int.TryParse(match.Groups[1].Value, out int units))
            return units;

        return null;
    }

    private RequestUrgency DetermineUrgency(string text)
    {
        text = text.ToLower();

        if (text.Contains("emergency") ||
            text.Contains("bleeding") ||
            text.Contains("surgery") ||
            text.Contains("icu") ||
            text.Contains("طارئ") ||
            text.Contains("حرج"))
            return RequestUrgency.Critical;

        if (text.Contains("urgent") ||
            text.Contains("عاجل"))
            return RequestUrgency.Urgent;

        return RequestUrgency.Normal;
    }
}