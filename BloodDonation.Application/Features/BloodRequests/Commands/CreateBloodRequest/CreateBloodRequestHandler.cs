using BloodDonation.Application.DTOs.BloodRequest;
using BloodDonation.Application.Interfaces;
using BloodDonation.Application.Interfaces.Repositories;
using BloodDonation.Domain.Entities;
using BloodDonation.Domain.Enums;
using MediatR;


namespace BloodDonation.Application.Features.BloodRequests.Commands.CreateBloodRequest
{
    public sealed class CreateBloodRequestHandler
     : IRequestHandler<CreateBloodRequestCommand, CreateBloodRequestResponseDto>
    {
        private readonly  IApplicationDbContext _dbContext;
       

        public CreateBloodRequestHandler(
            IBloodRequestRepository bloodRequestRepository,
            IApplicationDbContext dbContext)
        {
            _dbContext= dbContext;
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

            await _dbContext.BloodRequests.AddAsync(bloodRequest);
            await _dbContext.SaveChangesAsync(cancellationToken);

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

