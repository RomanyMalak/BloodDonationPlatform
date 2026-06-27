using BloodDonation.Application.DTOs.BloodRequest;
using BloodDonation.Application.Interfaces;
using BloodDonation.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BloodDonation.Application.Features.BloodRequests.Queries.GetCompletedBloodRequests;
public sealed class GetMatchingBloodRequestsHandler
    : IRequestHandler<GetMatchingBloodRequestsQuery, List<CompleteBloodRequestDto>>
{

    private readonly IApplicationDbContext _context;
    public GetMatchingBloodRequestsHandler(
        IApplicationDbContext context)
    {
        _context = context;
    }
    public async Task<List<CompleteBloodRequestDto>> Handle(
        GetMatchingBloodRequestsQuery request,
        CancellationToken cancellationToken)
    {

        return await _context.BloodRequests
            .Include(x => x.Hospital)
            .Where(x =>
                x.Status == RequestStatus.Matching)
            .OrderByDescending(x => x.CreatedAt)
            .Select(x => new CompleteBloodRequestDto
            {
                Id = x.Id,
                PatientName =x.PatientName,
                BloodType =x.RequiredBloodType.ToString(),
                UnitsNeeded =x.UnitsNeeded,
                Urgency =x.Urgency.ToString(),
                Status =x.Status.ToString(),
                HospitalName =x.Hospital != null? x.Hospital.Name : x.CustomHospitalName!,
                CreatedAt =x.CreatedAt,
            })
            .ToListAsync(cancellationToken);
    }
}