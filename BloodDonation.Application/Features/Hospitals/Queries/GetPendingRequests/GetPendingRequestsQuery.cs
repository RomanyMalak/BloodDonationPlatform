using BloodDonation.Application.DTOs.BloodRequest;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BloodDonation.Application.Features.Hospitals.Queries.GetPendingRequests
{
    public sealed record GetPendingRequestsQuery
    : IRequest<List<BloodRequestSummaryDto>>
    {
        public Guid HospitalId { get; init; }
    }

}
