using BloodDonation.Domain.Entities;

namespace BloodDonation.Application.DTOs.Hospital;

public class HospitalDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? City { get; set; }
    public string? Address { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public bool IsKnown { get; set; }
    public HospitalStatus Status { get; set; }
    public string StatusLabel => Status.ToString();
    public DateTime CreatedAt { get; set; }
    public DateTime? ReviewedAt { get; set; }
    public string? RejectionReason { get; set; }
    public int TotalBloodRequests { get; set; }
}
