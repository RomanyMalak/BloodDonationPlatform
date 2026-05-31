namespace BloodDonation.Domain.Entities;

public class UserReport
{
    public Guid Id { get; set; }
    public Guid ReporterId { get; set; }
    public Guid ReportedUserId { get; set; }
    public string Reason { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }

    public User Reporter { get; set; } = null!;
    public User ReportedUser { get; set; } = null!;
}
