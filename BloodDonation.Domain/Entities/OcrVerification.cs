using BloodDonation.Domain.Enums;

namespace BloodDonation.Domain.Entities;

public class OcrVerification
{
    public Guid Id { get; set; }
    public Guid BloodRequestId { get; set; }
    public bool IsVerified { get; set; }
    public double ConfidenceScore { get; set; }
    public string? RawExtractedText { get; set; }
    public string? FailureReason { get; set; }
    public BloodType? ExtractedBloodType { get; set; }
    public int? ExtractedUnits { get; set; }
    public RequestUrgency? ExtractedUrgency { get; set; }
    public DateTime VerifiedAt { get; set; }
    public BloodRequest BloodRequest { get; set; } = null!;
}  


