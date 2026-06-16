namespace BloodDonation.Application.DTOs.BloodRequest;

public sealed class CreateBloodRequestResponseDto
{
    public Guid Id { get; init; }
    public string Status { get; init; } = null!;
    public DateTime CreatedAt { get; init; }
    public string Message { get; init; } = null!;
}
