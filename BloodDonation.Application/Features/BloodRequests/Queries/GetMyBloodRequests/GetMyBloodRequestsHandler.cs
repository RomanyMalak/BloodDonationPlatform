using BloodDonation.Application.DTOs.BloodRequest;
using BloodDonation.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BloodDonation.Application.Features.BloodRequests.Queries.GetMyBloodRequests;

public sealed class GetMyBloodRequestsHandler
    : IRequestHandler<GetMyBloodRequestsQuery, List<BloodRequestSummaryDto>>
{
    private readonly IApplicationDbContext _dbContext;

    public GetMyBloodRequestsHandler(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<BloodRequestSummaryDto>> Handle(
        GetMyBloodRequestsQuery request,
        CancellationToken cancellationToken)
    {
        var bloodRequests = await _dbContext.BloodRequests
            .Include(x => x.Hospital)
            .Where(x => x.CreatedByUserId == request.UserId)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(cancellationToken);

        return bloodRequests.Select(x => new BloodRequestSummaryDto
        {
            Id = x.Id,
            PatientName = x.PatientName,
            RequiredBloodType = x.RequiredBloodType.ToString(),
            Urgency = x.Urgency.ToString(),
            Status = x.Status.ToString(),
            HospitalName = x.Hospital?.Name ?? x.CustomHospitalName,
            UnitsNeeded = x.UnitsNeeded,
            ContactPhone = x.ContactPhone,
            CreatedAt = x.CreatedAt,
            ExpiresAt = x.ExpiresAt
        }).ToList();
    }
}
