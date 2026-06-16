using BloodDonation.Domain.Enums;

namespace BloodDonation.Application.DTOs.Ocr
{
    public class OcrResultDto
    {
        public bool IsVerified { get; set; }
        public double ConfidenceScore { get; set; }
        public string? RawText { get; set; }
        public BloodType? ExtractedBloodType { get; set; }
        public int? ExtractedUnits { get; set; }
        public RequestUrgency? ExtractedUrgency { get; set; }
        public string? FailureReason { get; set; }
    }
}
