using BloodDonation.Domain.Enums;

namespace BloodDonation.Application.DTOs;

public sealed class DonorMatchingResultDto
{
    public Guid BloodRequestId { get; init; }
    public double SearchRadiusKm { get; init; }
    public IReadOnlyCollection<BloodType> CompatibleBloodTypes { get; init; } = Array.Empty<BloodType>();
    public List<DonorMatchDecisionDto> Decisions { get; init; } = new();

    public IReadOnlyCollection<DonorMatchDecisionDto> MatchedDonors =>
        Decisions.Where(x => x.IsMatched).ToList();
}

public sealed class DonorMatchDecisionDto
{
    public Guid DonorId { get; init; }
    public string DonorName { get; init; } = string.Empty;
    public BloodType? BloodType { get; init; }
    public double? DistanceKm { get; init; }
    public bool IsMatched { get; init; }
    public string Reason { get; init; } = string.Empty;
}
