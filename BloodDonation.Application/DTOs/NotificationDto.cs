using System.ComponentModel.DataAnnotations;

namespace BloodDonation.Application.DTOs;

public class NotificationDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string? Reason { get; set; }
    public string? Type { get; set; }
    public string Priority { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public bool IsRead { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? SentAt { get; set; }
    public DateTime? ReadAt { get; set; }
    public Guid? BloodRequestId { get; set; }
}

public class SendNotificationRequestDto
{
    [Required]
    public Guid UserId { get; set; }

    public Guid? BloodRequestId { get; set; }

    [Required]
    [MaxLength(150)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [MaxLength(500)]
    public string Message { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Reason { get; set; }

    [Required]
    public string Priority { get; set; } = string.Empty;
}

public class AdminNotificationDto : NotificationDto
{
    public Guid UserId { get; set; }
    public string UserFullName { get; set; } = string.Empty;
    public string UserEmail { get; set; } = string.Empty;
    public int RetryCount { get; set; }
    public string? ErrorMessage { get; set; }
}

public class RetryFailedNotificationsResultDto
{
    public int Attempted { get; set; }
    public int Sent { get; set; }
    public int Failed { get; set; }
}
