using MediatR;

namespace BloodDonation.Application.Features.BloodRequests.Commands.CompleteDonation;


public sealed record CompleteDonationCommand: IRequest<bool>
{
    public Guid BloodRequestId { get; init; }
    public Guid DonorId { get; init; }
    public Guid CompletedByUserId { get; init; }
    public string? Notes { get; init; }
}