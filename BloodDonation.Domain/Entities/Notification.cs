namespace BloodDonation.Domain.Entities;

public class Notification
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string? Type { get; set; }
    public bool IsRead { get; set; }
    public DateTime CreatedAt { get; set; }
    public Guid UserId { get; set; }
    public Guid? BloodRequestId { get; set; }

    public User User { get; set; } = null!;
    public BloodRequest? BloodRequest { get; set; }
}
