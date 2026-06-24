using BloodDonation.Application.DTOs.BloodRequest;
using BloodDonation.Application.Interfaces;
using BloodDonation.Domain.Entities;
using BloodDonation.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BloodDonation.Application.Features.Hospitals.Commands.ApproveBloodRequest
{
    public sealed class ApproveBloodRequestHandler
    : IRequestHandler<ApproveBloodRequestCommand, BloodRequestDetailsDto?>
    {

        private readonly IApplicationDbContext _dbContext;
        private readonly INotificationService _notificationService;

        public ApproveBloodRequestHandler(
            IApplicationDbContext dbContext,
            INotificationService notificationService)
        {
            _dbContext = dbContext;
            _notificationService = notificationService;
        }

        public async Task<BloodRequestDetailsDto?> Handle(
            ApproveBloodRequestCommand request,
            CancellationToken cancellationToken)
        {
            var bloodRequest =await _dbContext.BloodRequests
                .Include(x => x.Hospital)
                .FirstOrDefaultAsync(
                    x => x.Id == request.BloodRequestId,
                    cancellationToken
                );


            if (bloodRequest is null)
                return null;

            var hospital = await _dbContext.Hospitals.FirstOrDefaultAsync(
                x => x.Id == request.HospitalId,cancellationToken);


            if (hospital is null)
                throw new Exception("Hospital not found");


            if (!hospital.IsActive)
                throw new UnauthorizedAccessException(
                    "Hospital is waiting for admin approval");


            if (bloodRequest.HospitalId != hospital.Id)
                throw new UnauthorizedAccessException();

            if (bloodRequest.Status != RequestStatus.PendingVerification)
                throw new Exception("Request already processed");

            bloodRequest.Status = RequestStatus.Matching;
            bloodRequest.ApprovedByHospitalId = request.HospitalId;
            bloodRequest.ApprovedAt = DateTime.UtcNow;

            await _dbContext.SaveChangesAsync(cancellationToken);

            await _notificationService.CreateAsync(
                   bloodRequest.CreatedByUserId,
                   "Blood Request Approved",
                   "Your blood request has been approved by the hospital.",
                   bloodRequest.Id,
                   "BloodRequest",
                   cancellationToken);

            return new BloodRequestDetailsDto
            {
                Id = bloodRequest.Id,
                PatientName = bloodRequest.PatientName,
                RequiredBloodType = bloodRequest.RequiredBloodType.ToString(),
                Urgency = bloodRequest.Urgency.ToString(),
                Status = bloodRequest.Status.ToString(),
                HospitalName =bloodRequest.Hospital?.Name,
                UnitsNeeded = bloodRequest.UnitsNeeded,
                CreatedAt =bloodRequest.CreatedAt
            };
        }
    }
}
