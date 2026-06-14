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


    public GetAvailableBloodRequestsHandler(
        IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }



    public async Task<List<BloodRequestSummaryDto>> Handle(
        GetAvailableBloodRequestsQuery request,
        CancellationToken cancellationToken)
    {
        var query = _dbContext.BloodRequests
            .Include(x => x.Hospital)
            .Where(x => x.Status == RequestStatus.Matching &&
                (x.ExpiresAt == null || x.ExpiresAt > DateTime.UtcNow));

        if (request.BloodType.HasValue)
        {
            query = query.Where(x => x.RequiredBloodType == request.BloodType.Value);
        }



        var bloodRequests = await query
            .OrderByDescending(x => x.Urgency)
            .ThenBy(x => x.CreatedAt)
            .ToListAsync(cancellationToken);



        return bloodRequests.Select(x => new BloodRequestSummaryDto
        {
            Id = x.Id,
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