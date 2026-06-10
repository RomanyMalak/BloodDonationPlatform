using BloodDonation.Application.DTOs.BloodRequest;
using BloodDonation.Application.Interfaces;
using BloodDonation.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BloodDonation.Application.Features.Hospitals.Queries.GetPendingRequests;

public sealed class GetPendingRequestsHandler
    : IRequestHandler<GetPendingRequestsQuery, List<BloodRequestSummaryDto>>
{
    private readonly IApplicationDbContext _dbContext;
    public GetPendingRequestsHandler(
        IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<BloodRequestSummaryDto>> Handle(
        GetPendingRequestsQuery request,
        CancellationToken cancellationToken)
    {
        var hospital = await _dbContext.Hospitals
            .FirstOrDefaultAsync( x => x.Id == request.HospitalId,cancellationToken);

        if (hospital is null || !hospital.IsActive)
            return new List<BloodRequestSummaryDto>();

        return await _dbContext.BloodRequests.Where(x =>x.HospitalId == request.HospitalId &&
             x.Status == RequestStatus.PendingVerification)
            .OrderByDescending(x => x.CreatedAt)
            .Select(x => new BloodRequestSummaryDto
            {
                Id = x.Id,
                PatientName = x.PatientName,
                RequiredBloodType =x.RequiredBloodType.ToString(),
                Urgency =x.Urgency.ToString(),
                Status =x.Status.ToString(),
                HospitalName = x.Hospital != null ? x.Hospital.Name: x.CustomHospitalName,
                UnitsNeeded =x.UnitsNeeded,
                CreatedAt =x.CreatedAt,
                ExpiresAt =x.ExpiresAt

            }).ToListAsync(cancellationToken);
    }
}