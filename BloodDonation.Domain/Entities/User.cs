using BloodDonation.Domain.Enums;

namespace BloodDonation.Domain.Entities;

public class User
{
    public Guid Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public int? Age { get; set; }
    public BloodType? BloodType { get; set; }
    public UserRole Role { get; set; }
    public Hospital? Hospital { get; set; }
    public double ? Latitude { get; set; }
    public double ? Longitude { get; set; }
    public bool IsAvailable { get; set; }
    public DateTime? LastDonationDate { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;


    public ICollection<BloodRequest> BloodRequests { get; set; } = new List<BloodRequest>();

    public ICollection<BloodRequestAcceptance> AcceptedRequests { get; set; } = new List<BloodRequestAcceptance>();

    public ICollection<Notification> Notifications { get; set; } = new List<Notification>();

    public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();

    public ICollection<DonationHistory> DonationsMade { get; set; } = new List<DonationHistory>();

    public ICollection<DonationHistory> DonationsReceived { get; set; } = new List<DonationHistory>();

    public ICollection<UserReport> ReportsMade { get; set; } = new List<UserReport>();

    public ICollection<UserReport> ReportsReceived { get; set; } = new List<UserReport>();
}
