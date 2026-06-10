using BloodDonation.Application.DTOs.BloodRequest;
using MediatR;

namespace BloodDonation.Application.Features.Hospitals.Commands.ApproveBloodRequest
{
    public sealed record ApproveBloodRequestCommand
    : IRequest<BloodRequestDetailsDto?>
    {
        public Guid BloodRequestId { get; init; }
        public Guid HospitalId { get; init; }
    }
}
