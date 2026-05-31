namespace BloodDonation.Domain.Entities;

public class Hospital
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? City { get; set; }
    public string? Address { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public bool IsKnown { get; set; }
    public DateTime CreatedAt { get; set; }

    public ICollection<BloodRequest> BloodRequests { get; set; } = new List<BloodRequest>();
}
