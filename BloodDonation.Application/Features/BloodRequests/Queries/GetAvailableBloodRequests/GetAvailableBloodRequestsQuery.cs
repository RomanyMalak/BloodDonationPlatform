using BloodDonation.Application.DTOs.BloodRequest;
using MediatR;
using BloodDonation.Domain.Enums;

namespace BloodDonation.Application.Features.BloodRequests.Queries.GetAvailableBloodRequests;
public sealed record GetAvailableBloodRequestsQuery
    : IRequest<List<BloodRequestSummaryDto>>
{
    public Guid DonorId { get; init; }
    public BloodType? BloodType { get; init; }
    public double? Latitude { get; init; }
    public double? Longitude { get; init; }
    public double? RadiusKm { get; init; }

}