using BloodDonation.Domain.Enums;

namespace BloodDonation.Domain.Entities;

public class BloodRequest
{
    public Guid Id { get; set; }

    public string PatientName { get; set; } = string.Empty;

    public int? PatientAge { get; set; }

    public BloodType RequiredBloodType { get; set; }

    public RequestUrgency Urgency { get; set; }

    public RequestStatus Status { get; set; }

    public Guid? HospitalId { get; set; }

    public string? CustomHospitalName { get; set; }

    public double Latitude { get; set; }

    public double Longitude { get; set; }

    public string? MedicalDocumentUrl { get; set; }

    public string? Notes { get; set; }

    public string? ContactPhone { get; set; }

    public int UnitsNeeded { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? ExpiresAt { get; set; }

    public Guid CreatedByUserId { get; set; }

    public Guid? ApprovedByHospitalId { get; set; }

    public DateTime? ApprovedAt { get; set; }

    public string? RejectionReason { get; set; }

    public User CreatedByUser { get; set; } = null!;

    public Hospital? Hospital { get; set; }
    public OcrVerification? OcrVerification { get; set; }

    public ICollection<BloodRequestAcceptance> Acceptances { get; set; } = new List<BloodRequestAcceptance>();

    public ICollection<Notification> Notifications { get; set; } = new List<Notification>();

    public ICollection<AiMatchingLog> AiMatchingLogs { get; set; } = new List<AiMatchingLog>();

    public ICollection<DonationHistory> DonationHistories { get; set; } = new List<DonationHistory>();
}
