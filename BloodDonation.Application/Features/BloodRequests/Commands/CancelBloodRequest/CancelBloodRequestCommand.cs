using MediatR;

namespace BloodDonation.Application.Features.BloodRequests.Commands.CancelBloodRequest;

public sealed record CancelBloodRequestCommand : IRequest<bool>
{
    public Guid BloodRequestId { get; init; }
    public Guid UserId { get; init; }
}