namespace BloodDonation.Application.DTOs.BloodRequest;

public class BloodRequestSummaryDto
{
    public Guid Id { get; init; }
    public string PatientName { get; init; }
    public string RequiredBloodType { get; init; } = null!;
    public string Urgency { get; init; } = null!;
    public string Status { get; init; } = null!;
    public string? HospitalName { get; init; }
    public int UnitsNeeded { get; init; }
    public string? ContactPhone { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? ExpiresAt { get; init; }
}