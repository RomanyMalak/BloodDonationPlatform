using BloodDonation.Domain.Enums;

namespace BloodDonation.Domain.Entities;

public class AiMatchingLog
{
    public Guid Id { get; set; }
    public Guid BloodRequestId { get; set; }
    public PriorityResult PriorityResult { get; set; }
    public int MatchedDonorsCount { get; set; }
    public int NotificationsSent { get; set; }
    public double SearchRadiusKm { get; set; }
    public string UsedCompatibleBloodTypes { get; set; } = string.Empty;
    public string PipelineSummary { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }

    public BloodRequest BloodRequest { get; set; } = null!;
}
