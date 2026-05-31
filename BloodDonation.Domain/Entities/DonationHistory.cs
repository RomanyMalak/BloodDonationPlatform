namespace BloodDonation.Domain.Entities;

public class DonationHistory
{
    public Guid Id { get; set; }
    public Guid DonorId { get; set; }
    public Guid PatientId { get; set; }
    public Guid? BloodRequestId { get; set; }
    public string HospitalName { get; set; } = string.Empty;
    public DateTime DonationDate { get; set; }
    public string? Notes { get; set; }

    public User Donor { get; set; } = null!;
    public User Patient { get; set; } = null!;
    public BloodRequest? BloodRequest { get; set; }
}
