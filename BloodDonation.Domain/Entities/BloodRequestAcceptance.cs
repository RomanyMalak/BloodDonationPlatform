using BloodDonation.Domain.Enums;

namespace BloodDonation.Domain.Entities;

public class BloodRequestAcceptance
{
    public Guid Id { get; set; }
    public Guid BloodRequestId { get; set; }
    public Guid DonorId { get; set; }
    public DateTime AcceptedAt { get; set; }
    public AcceptanceStatus Status { get; set; }
    public string? Notes { get; set; }

    public BloodRequest BloodRequest { get; set; } = null!;
    public User Donor { get; set; } = null!;
}
