using BloodDonation.Application.DTOs.BloodRequest;
using BloodDonation.Application.Interfaces;
using BloodDonation.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BloodDonation.Application.Features.BloodRequests.Queries.GetAvailableBloodRequests;

public sealed class GetAvailableBloodRequestsHandler
    : IRequestHandler<GetAvailableBloodRequestsQuery, List<BloodRequestSummaryDto>>
{
    private readonly IApplicationDbContext _dbContext;

    public GetAvailableBloodRequestsHandler(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<BloodRequestSummaryDto>> Handle(
    GetAvailableBloodRequestsQuery request,
    CancellationToken cancellationToken)
    {
        // ١. جيب البيانات من الـ DB أولاً
        var bloodRequests = await _dbContext.BloodRequests
            .Include(br => br.Hospital)
            .Where(br =>
                br.Status == RequestStatus.Approved &&
                (br.ExpiresAt == null || br.ExpiresAt > DateTime.UtcNow))
            .OrderByDescending(br => br.Urgency)
            .ThenBy(br => br.CreatedAt)
            .ToListAsync(cancellationToken);


        return bloodRequests.Select(br => new BloodRequestSummaryDto
        {
            Id = br.Id,
            RequiredBloodType = br.RequiredBloodType.ToString(),
            Urgency = br.Urgency.ToString(),
            Status = br.Status.ToString(),
            HospitalName = br.Hospital?.Name ?? br.CustomHospitalName,
            UnitsNeeded = br.UnitsNeeded,
            ContactPhone = br.ContactPhone,
            CreatedAt = br.CreatedAt,
            ExpiresAt = br.ExpiresAt
        }).ToList();
    }
}