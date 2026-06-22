namespace BloodDonation.Application.DTOs.BloodRequest;

public record CompleteDonationRequest(Guid DonorId, string? Notes);