using BloodDonation.Application.DTOs.BloodRequest;
using BloodDonation.Application.Interfaces;
using BloodDonation.Application.Interfaces.Repositories;
using BloodDonation.Domain.Entities;
using BloodDonation.Domain.Enums;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BloodDonation.Application.Features.BloodRequests.Commands.CreateBloodRequest
{
    public sealed class CreateBloodRequestHandler
     : IRequestHandler<CreateBloodRequestCommand, CreateBloodRequestResponseDto>
    {
        private readonly IBloodRequestRepository _bloodRequestRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CreateBloodRequestHandler(
            IBloodRequestRepository bloodRequestRepository,
            IUnitOfWork unitOfWork)
        {
            _bloodRequestRepository = bloodRequestRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<CreateBloodRequestResponseDto> Handle(
            CreateBloodRequestCommand request,
            CancellationToken cancellationToken)
        {
            var bloodRequest = new BloodRequest
            {
                Id = Guid.NewGuid(),
                RequiredBloodType = request.RequiredBloodType,
                Urgency = request.Urgency,
                Status = RequestStatus.Pending,
                HospitalId = request.HospitalId,
                CustomHospitalName = request.CustomHospitalName,
                Latitude = request.Latitude,
                Longitude = request.Longitude,
                MedicalDocumentUrl = request.MedicalDocumentUrl,
                Notes = request.Notes,
                ContactPhone = request.ContactPhone,
                UnitsNeeded = request.UnitsNeeded,
                ExpiresAt = request.ExpiresAt,
                PatientId = request.PatientId,
                CreatedAt = DateTime.UtcNow
            };

            await _bloodRequestRepository.AddAsync(bloodRequest);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // --- هنا لاحقاً هنضيف: await _aiPipeline.RunAsync(bloodRequest); ---

            return new CreateBloodRequestResponseDto
            {
                Id = bloodRequest.Id,
                Status = bloodRequest.Status.ToString(),
                CreatedAt = bloodRequest.CreatedAt,
                Message = "Blood request created successfully. Matching donors will be notified shortly."
            };
        }
    }
}

