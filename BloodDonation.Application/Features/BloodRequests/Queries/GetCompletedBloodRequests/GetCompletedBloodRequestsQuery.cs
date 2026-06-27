using BloodDonation.Application.DTOs.BloodRequest;
using MediatR;

namespace BloodDonation.Application.Features.BloodRequests.Queries.GetCompletedBloodRequests;

public sealed record GetCompletedBloodRequestsQuery
    : IRequest<List<CompleteBloodRequestDto>>;