using BloodDonation.Application.DTOs.BloodRequest;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BloodDonation.Application.Features.Hospitals.Commands.RejectBloodRequest
{
    public sealed record RejectBloodRequestCommand
    : IRequest<BloodRequestDetailsDto?>
    {
        public Guid BloodRequestId { get; init; }

        public Guid HospitalId { get; init; }

        public string Reason { get; init; } = string.Empty;
    }
}
