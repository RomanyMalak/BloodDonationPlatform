using BloodDonation.Application.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BloodDonation.Application.Features.Donors.Query
{
    public record GetAvailableBloodTypesQuery(Guid RequestId)
    : IRequest<AvailableBloodTypesResponse>;
}
