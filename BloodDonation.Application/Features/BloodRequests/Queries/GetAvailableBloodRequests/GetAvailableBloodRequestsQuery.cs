using BloodDonation.Application.DTOs.BloodRequest;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BloodDonation.Application.Features.BloodRequests.Queries.GetAvailableBloodRequests
{
    public class GetAvailableBloodRequestsQuery : IRequest<List<BloodRequestSummaryDto>>
    {
        //هنضيف فيها الداتا اللى هنعمل بيها فيلتر بعدين
    }
}
