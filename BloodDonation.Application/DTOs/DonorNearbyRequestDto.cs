namespace BloodDonation.Application.DTOs;

public class DonorNearbyRequestDto
{
    public Guid Id { get; set; }
    public string RequiredBloodType { get; set; } = string.Empty;
    public string Urgency { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string HospitalName { get; set; } = string.Empty;
    public double DistanceKm { get; set; }
    public DateTime CreatedAt { get; set; }
}
