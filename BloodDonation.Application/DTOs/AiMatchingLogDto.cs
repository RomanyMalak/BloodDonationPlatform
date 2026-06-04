namespace BloodDonation.Application.DTOs;

public class AiMatchingLogDto
{
    public Guid Id { get; set; }
    public Guid BloodRequestId { get; set; }
    public string PriorityResult { get; set; } = string.Empty;
    public int MatchedDonorsCount { get; set; }
    public int NotificationsSent { get; set; }
    public double SearchRadiusKm { get; set; }
    public string UsedCompatibleBloodTypes { get; set; } = string.Empty;
    public string PipelineSummary { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
