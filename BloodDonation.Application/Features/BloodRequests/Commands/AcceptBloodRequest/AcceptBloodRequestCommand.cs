// AcceptBloodRequestCommand.cs
using MediatR;

namespace BloodDonation.Application.Features.BloodRequests.Commands.AcceptBloodRequest;

public sealed class AcceptBloodRequestCommand : IRequest<bool>
{
    public Guid BloodRequestId { get; init; }
    public Guid DonorId { get; init; } 
}