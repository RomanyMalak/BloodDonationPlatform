using BloodDonation.Application.DTOs.BloodRequest;
using MediatR;

namespace BloodDonation.Application.Features.BloodRequests.Queries.GetCompletedBloodRequests;

public sealed record GetMatchingBloodRequestsQuery
    : IRequest<List<CompleteBloodRequestDto>>;