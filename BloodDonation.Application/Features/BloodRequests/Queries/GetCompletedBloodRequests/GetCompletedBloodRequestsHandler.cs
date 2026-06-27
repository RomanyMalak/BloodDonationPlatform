using BloodDonation.Application.DTOs.BloodRequest;
using BloodDonation.Application.Interfaces;
using BloodDonation.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BloodDonation.Application.Features.BloodRequests.Queries.GetCompletedBloodRequests;
public sealed class GetCompletedBloodRequestsHandler
    : IRequestHandler<GetCompletedBloodRequestsQuery, List<CompleteBloodRequestDto>>
{

    private readonly IApplicationDbContext _context;
    public GetCompletedBloodRequestsHandler(
        IApplicationDbContext context)
    {
        _context = context;
    }
    public async Task<List<CompleteBloodRequestDto>> Handle(
        GetCompletedBloodRequestsQuery request,
        CancellationToken cancellationToken)
    {

        return await _context.BloodRequests
            .Include(x => x.Hospital)
            .Where(x =>
                x.Status == RequestStatus.Completed)
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