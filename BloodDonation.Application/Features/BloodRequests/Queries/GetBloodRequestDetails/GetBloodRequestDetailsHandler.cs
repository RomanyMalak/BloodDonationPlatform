// GetBloodRequestDetailsHandler.cs
using BloodDonation.Application.DTOs.BloodRequest;
using BloodDonation.Application.Interfaces.Repositories;
using MediatR;

namespace BloodDonation.Application.Features.BloodRequests.Queries.GetBloodRequestDetails;

public sealed class GetBloodRequestDetailsHandler
    : IRequestHandler<GetBloodRequestDetailsQuery, BloodRequestDetailsDto?>
{
    private readonly IBloodRequestRepository _repo;

    public GetBloodRequestDetailsHandler(IBloodRequestRepository repo)
    {
        _repo = repo;
    }

    public async Task<BloodRequestDetailsDto?> Handle(
        GetBloodRequestDetailsQuery request,
        CancellationToken cancellationToken)
    {
        var br = await _repo.GetWithAcceptancesAsync(request.BloodRequestId);

        if (br is null) return null;

        return new BloodRequestDetailsDto
        {
            Id = br.Id,
            RequiredBloodType = br.RequiredBloodType.ToString(),
            Urgency = br.Urgency.ToString(),
            Status = br.Status.ToString(),
            HospitalName = br.Hospital?.Name ?? br.CustomHospitalName,
            Latitude = br.Latitude,
            Longitude = br.Longitude,
            UnitsNeeded = br.UnitsNeeded,
            ContactPhone = br.ContactPhone,
            Notes = br.Notes,
            CreatedAt = br.CreatedAt,
            ExpiresAt = br.ExpiresAt,
            PatientName = br.CreatedByUser.FullName
        };
    }
}