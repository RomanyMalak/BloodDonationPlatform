using BloodDonation.Application.DTOs.Ocr;
using BloodDonation.Application.Interfaces;
using BloodDonation.Domain.Enums;
using Tesseract;

namespace BloodDonation.Infrastructure.Services;

public class OcrService : IOcrService
{
    private static readonly string[] MedicalKeywords =
    {
        // طبي عام
        "تشخيص", "مستشفى", "طبيب", "دكتور", "مريض",
        "وصفة", "علاج", "جراحة", "عملية", "طوارئ",
        "تحليل", "أشعة", "نتيجة", "تقرير طبي",
        "وحدة دم", "نقل دم", "فصيلة", "بنك الدم",

        // تخصصات
        "باطنة", "عظام", "قلب", "أورام",
        "نسا وتوليد", "أطفال", "عيون", "كلى",

        // حالة المريض
        "نزيف", "حادث", "إصابة", "فقر دم", "أنيميا",
        "ضغط", "سكر", "فشل كلوي", "زراعة",

        // طبي عام إنجليزي
        "diagnosis", "hospital", "doctor", "patient",
        "blood", "medical", "report", "prescription",
        "treatment", "surgery", "operation", "emergency",
        "laboratory", "analysis", "result", "clinic",

        // دم تحديداً
        "blood type", "blood group", "blood bank",
        "transfusion", "units", "hemoglobin",
        "anemia", "hemorrhage", "bleeding",

        // تخصصات إنجليزي
        "internal medicine", "orthopedic", "cardiology",
        "oncology", "nephrology", "pediatric",

        // حالة المريض إنجليزي
        "icu", "critical", "urgent", "accident",
        "injury", "trauma", "kidney failure"
    };

    private static readonly Dictionary<string, string> BloodTypeSymbols = new()
    {
        { "APositive",  "A+"  },
        { "ANegative",  "A-"  },
        { "BPositive",  "B+"  },
        { "BNegative",  "B-"  },
        { "ABPositive", "AB+" },
        { "ABNegative", "AB-" },
        { "OPositive",  "O+"  },
        { "ONegative",  "O-"  }
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

        // ٣. تحقق من الكلمات الطبية
        int matchCount = MedicalKeywords.Count(k =>
            extractedText.Contains(k, StringComparison.OrdinalIgnoreCase));

        if (matchCount < 3)
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
            !extractedText.Contains(
                hospitalName, StringComparison.OrdinalIgnoreCase))
        {
            result.IsVerified = false;
            result.FailureReason = "Hospital name not found in document.";
            return result;
        }

        // ٦. تحقق من فصيلة الدم
        var bloodTypeSymbol = BloodTypeSymbols.TryGetValue(
            bloodType, out var symbol) ? symbol : bloodType;

        if (!extractedText.Contains(
                bloodTypeSymbol, StringComparison.OrdinalIgnoreCase))
        {
            result.IsVerified = false;
            result.FailureReason = "Blood type mismatch in document.";
            return result;
        }

        result.IsVerified = true;
        return result;
    }

    private async Task<string> ExtractTextAsync(string? documentUrl)
    {
        if (string.IsNullOrEmpty(documentUrl))
            return string.Empty;

        try
        {
            var tessDataPath = Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                "tessdata");

            byte[] fileBytes;

            if (documentUrl.StartsWith("http",
                StringComparison.OrdinalIgnoreCase))
            {
                using var httpClient = new HttpClient();
                fileBytes = await httpClient
                    .GetByteArrayAsync(documentUrl);
            }
            else
            {
                fileBytes = await File.ReadAllBytesAsync(documentUrl);
            }

            using var engine = new TesseractEngine(
                tessDataPath, "ara+eng", EngineMode.Default);

            using var img = Pix.LoadFromMemory(fileBytes);
            using var page = engine.Process(img);

            return page.GetText();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"OCR Error: {ex.Message}");
            return string.Empty;
        }
    }

    private double CalculateScore(string text)
    {
        if (string.IsNullOrWhiteSpace(text)) return 0;

        var words = text
            .Split(' ', StringSplitOptions.RemoveEmptyEntries)
            .Length;

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
        var match = System.Text.RegularExpressions.Regex.Match(
            text,
            @"(\d+)\s*(units|وحدة|unit)",
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
            text.Contains("نزيف") ||
            text.Contains("حرج"))
            return RequestUrgency.Critical;

        if (text.Contains("urgent") ||
            text.Contains("عاجل"))
            return RequestUrgency.Urgent;

        return RequestUrgency.Normal;
    }
}