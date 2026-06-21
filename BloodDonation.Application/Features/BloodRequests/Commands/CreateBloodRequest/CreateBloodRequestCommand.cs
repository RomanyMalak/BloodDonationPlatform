using BloodDonation.Application.DTOs.BloodRequest;
using BloodDonation.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BloodDonation.Application.Features.BloodRequests.Commands.CreateBloodRequest
{
    public sealed record CreateBloodRequestCommand
     : IRequest<CreateBloodRequestResponseDto>
    {
        public string PatientName { get; init; }

        public int? PatientAge { get; init; }

        public BloodType? RequiredBloodType { get; init; }

        public RequestUrgency Urgency { get; init; }

        public Guid? HospitalId { get; init; }

        public string? CustomHospitalName { get; init; }

        public double Latitude { get; init; }

        public double Longitude { get; init; }

        public IFormFile MedicalDocumentUrl { get; init; }

        public string? Notes { get; init; }

        public string? ContactPhone { get; init; }

        public int UnitsNeeded { get; init; }

        public DateTime? ExpiresAt { get; init; }

        public Guid CreatedByUserId { get; init; }
    }
}
