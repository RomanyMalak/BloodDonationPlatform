namespace BloodDonation.Domain.Enums;

public enum RequestStatus
{
    PendingVerification,
    Approved,
    Matching,
    Accepted,
    Completed,
    Rejected,
    Cancelled
}