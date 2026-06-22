using BloodDonation.Application.Interfaces;
using BloodDonation.Domain.Entities;
using BloodDonation.Domain.Enums;
using BloodDonation.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Text;

namespace BloodDonation.Infrastructure.Services;

public sealed class NotificationAgentService : IAiMatchingPipelineService
{
    private const string AgentNotificationType = "AgentMatching";

    private readonly AppDbContext _context;
    private readonly IDonorMatchingService _donorMatchingService;
    private readonly INotificationService _notificationService;
    private readonly ILogger<NotificationAgentService> _logger;

    public NotificationAgentService(
        AppDbContext context,
        IDonorMatchingService donorMatchingService,
        INotificationService notificationService,
        ILogger<NotificationAgentService> logger)
    {
        _context = context;
        _donorMatchingService = donorMatchingService;
        _notificationService = notificationService;
        _logger = logger;
    }

    public async Task RunAsync(Guid bloodRequestId, CancellationToken cancellationToken)
    {
        var bloodRequest = await _context.BloodRequests
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == bloodRequestId, cancellationToken);

        if (bloodRequest is null)
        {
            _logger.LogWarning(
                "Notification agent skipped missing blood request {BloodRequestId}",
                bloodRequestId);
            return;
        }

        var matchingResult = await _donorMatchingService.FindMatchesAsync(
            bloodRequest.Id,
            cancellationToken);

        var summary = new StringBuilder();
        summary.AppendLine($"BloodRequestId={bloodRequest.Id}");
        summary.AppendLine($"RequiredBloodType={bloodRequest.RequiredBloodType}");
        summary.AppendLine($"SearchRadiusKm={matchingResult.SearchRadiusKm:0.##}");
        summary.AppendLine($"CompatibleBloodTypes={string.Join(", ", matchingResult.CompatibleBloodTypes)}");

        foreach (var decision in matchingResult.Decisions)
        {
            var outcome = decision.IsMatched ? "MATCHED" : "SKIPPED";
            var distance = decision.DistanceKm.HasValue
                ? $"{decision.DistanceKm.Value:0.00} km"
                : "unknown distance";

            summary.AppendLine(
                $"DonorId={decision.DonorId}; Name={decision.DonorName}; BloodType={decision.BloodType?.ToString() ?? "Unknown"}; Distance={distance}; Outcome={outcome}; Reason={decision.Reason}");
        }

        var notificationsSent = 0;

        foreach (var donor in matchingResult.MatchedDonors)
        {
            try
            {
                var notification = await _notificationService.CreateAsync(
                    donor.DonorId,
                    BuildTitle(bloodRequest.Urgency),
                    BuildMessage(bloodRequest.RequiredBloodType, donor.DistanceKm),
                    bloodRequest.Id,
                    AgentNotificationType,
                    cancellationToken,
                    donor.Reason,
                    MapNotificationPriority(bloodRequest.Urgency));

                if (notification.Status == NotificationStatus.Sent.ToString())
                {
                    notificationsSent++;
                }

                summary.AppendLine(
                    $"NotificationId={notification.Id}; DonorId={donor.DonorId}; SignalRPushStatus={notification.Status}");
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                summary.AppendLine(
                    $"NotificationFailed; DonorId={donor.DonorId}; Error={Trim(ex.Message, 300)}");
                _logger.LogError(
                    ex,
                    "Notification creation or push failed for donor {DonorId} and blood request {BloodRequestId}",
                    donor.DonorId,
                    bloodRequest.Id);
            }
        }

        var log = new AiMatchingLog
        {
            Id = Guid.NewGuid(),
            BloodRequestId = bloodRequest.Id,
            PriorityResult = MapPriorityResult(bloodRequest.Urgency),
            MatchedDonorsCount = matchingResult.MatchedDonors.Count,
            NotificationsSent = notificationsSent,
            SearchRadiusKm = matchingResult.SearchRadiusKm,
            UsedCompatibleBloodTypes = string.Join(", ", matchingResult.CompatibleBloodTypes),
            PipelineSummary = summary.ToString(),
            CreatedAt = DateTime.UtcNow
        };

        await _context.AiMatchingLogs.AddAsync(log, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    private static string BuildTitle(RequestUrgency urgency)
    {
        return urgency == RequestUrgency.Critical
            ? "Critical blood donation match"
            : "Blood donation match nearby";
    }

    private static string BuildMessage(BloodType requiredBloodType, double? distanceKm)
    {
        var distanceText = distanceKm.HasValue
            ? $" about {distanceKm.Value:0.00} km from you"
            : string.Empty;

        return $"A blood request{distanceText} matches your donor profile. Needed blood type: {requiredBloodType}.";
    }

    private static NotificationPriority MapNotificationPriority(RequestUrgency urgency)
    {
        return urgency switch
        {
            RequestUrgency.Critical => NotificationPriority.Critical,
            RequestUrgency.Urgent => NotificationPriority.High,
            _ => NotificationPriority.Normal
        };
    }

    private static PriorityResult MapPriorityResult(RequestUrgency urgency)
    {
        return urgency switch
        {
            RequestUrgency.Critical => PriorityResult.Critical,
            RequestUrgency.Urgent => PriorityResult.High,
            _ => PriorityResult.Medium
        };
    }

    private static string Trim(string value, int maxLength)
    {
        return value.Length <= maxLength ? value : value[..maxLength];
    }
}
