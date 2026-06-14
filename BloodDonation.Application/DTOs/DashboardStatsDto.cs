namespace BloodDonation.Application.DTOs;

public class DashboardStatsDto
{
    public int TotalRequests { get; set; }
    public int ActiveRequests { get; set; }
    public int TotalDonors { get; set; }
    public int TotalDonations { get; set; }
    public int TotalNotificationsSent { get; set; }

    // Hospital statistics
    public int TotalHospitals { get; set; }
    public int WaitingHospitals { get; set; }
    public int ActiveHospitals { get; set; }
    public int RejectedHospitals { get; set; }
}
