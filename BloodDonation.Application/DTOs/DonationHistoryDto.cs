namespace BloodDonation.Application.DTOs;

public class DonationHistoryDto
{
    public Guid Id { get; set; }
    public string HospitalName { get; set; } = string.Empty;
    public DateTime DonationDate { get; set; }
    public string? Notes { get; set; }
}
