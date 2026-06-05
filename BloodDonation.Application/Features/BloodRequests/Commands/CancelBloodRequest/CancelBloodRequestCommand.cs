using MediatR;

namespace BloodDonation.Application.Features.BloodRequests.Commands.CancelBloodRequest;

public sealed class CancelBloodRequestCommand : IRequest<bool>
{
    public Guid BloodRequestId { get; init; }
    public Guid PatientId { get; init; }
}