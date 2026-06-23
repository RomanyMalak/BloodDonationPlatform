using BloodDonation.Application.DTOs.BloodRequest;
using MediatR;

namespace BloodDonation.Application.Features.BloodRequests.Queries.GetMyBloodRequests;

public sealed record GetMyBloodRequestsQuery : IRequest<List<BloodRequestSummaryDto>>
{
    public Guid UserId { get; init; }
}
