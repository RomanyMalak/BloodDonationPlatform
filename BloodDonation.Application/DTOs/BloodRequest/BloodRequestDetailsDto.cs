namespace BloodDonation.Application.DTOs.BloodRequest;

public class BloodRequestDetailsDto
{
    public Guid Id { get; set; }
    public string PatientName { get; set; } = string.Empty;
    public int? PatientAge { get; set; }
    public string RequiredBloodType { get; set; } = string.Empty;
    public string Urgency { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string? HospitalName { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public int UnitsNeeded { get; set; }
    public string? ContactPhone { get; set; }
    public string? Notes { get; set; }
    public DateTime? CreatedAt {  get; set; }
    public DateTime? ExpiresAt { get; set; }
    public DateTime? ApprovedAt { get; set; }
}