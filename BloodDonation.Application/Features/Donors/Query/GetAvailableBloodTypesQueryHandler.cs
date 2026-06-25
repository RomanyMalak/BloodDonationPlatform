using BloodDonation.Application.DTOs;
using BloodDonation.Application.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BloodDonation.Application.Features.Donors.Query
{
    public class GetAvailableBloodTypesQueryHandler
    : IRequestHandler<GetAvailableBloodTypesQuery, AvailableBloodTypesResponse>
    {
        private readonly IDonorMatchingService _donorMatchingService;

        public GetAvailableBloodTypesQueryHandler(
            IDonorMatchingService donorMatchingService)
        {
            _donorMatchingService = donorMatchingService;
        }

        public async Task<AvailableBloodTypesResponse> Handle(
            GetAvailableBloodTypesQuery request,
            CancellationToken cancellationToken)
        {
            return await _donorMatchingService.GetAvailableBloodTypesAsync(
                request.RequestId,
                cancellationToken);
        }
    }
}
