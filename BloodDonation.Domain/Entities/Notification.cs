using BloodDonation.Domain.Enums;

namespace BloodDonation.Domain.Entities;

public class Notification
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string? Reason { get; set; }
    public string? Type { get; set; }
    public NotificationPriority Priority { get; set; } = NotificationPriority.Normal;
    public NotificationStatus Status { get; set; } = NotificationStatus.Pending;
    public bool IsRead { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? SentAt { get; set; }
    public DateTime? ReadAt { get; set; }
    public int RetryCount { get; set; }
    public string? ErrorMessage { get; set; }
    public Guid UserId { get; set; }
    public Guid? BloodRequestId { get; set; }

    public User User { get; set; } = null!;
    public BloodRequest? BloodRequest { get; set; }
}
