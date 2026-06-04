using BloodDonation.Application.DTOs.BloodRequest;
using MediatR;

namespace BloodDonation.Application.Features.BloodRequests.Queries.GetBloodRequestDetails;

public sealed class GetBloodRequestDetailsQuery : IRequest<BloodRequestDetailsDto?>
{
    public Guid BloodRequestId { get; init; }
}