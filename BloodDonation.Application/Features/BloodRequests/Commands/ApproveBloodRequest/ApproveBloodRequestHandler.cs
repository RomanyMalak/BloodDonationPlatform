using BloodDonation.Application.DTOs.BloodRequest;
using BloodDonation.Application.Interfaces;
using BloodDonation.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BloodDonation.Application.Features.BloodRequests.Commands.ApproveBloodRequest
{
    public sealed class ApproveBloodRequestHandler
    : IRequestHandler<ApproveBloodRequestCommand, BloodRequestDetailsDto?>
    {

        private readonly IApplicationDbContext _dbContext;

        public ApproveBloodRequestHandler(
            IApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<BloodRequestDetailsDto?> Handle(
            ApproveBloodRequestCommand request,
            CancellationToken cancellationToken)
        {
            var bloodRequest =await _dbContext.BloodRequests
                .Include(x => x.Hospital)
                .Include(x => x.CreatedByUser)
                .FirstOrDefaultAsync(
                    x => x.Id == request.BloodRequestId,
                    cancellationToken
                );


            if (bloodRequest is null)
                return null;

            if (bloodRequest.HospitalId != request.HospitalId)
                throw new UnauthorizedAccessException();

            bloodRequest.Status = RequestStatus.Matching;
            bloodRequest.ApprovedByHospitalId = request.HospitalId;
            bloodRequest.ApprovedAt = DateTime.UtcNow;

            await _dbContext.SaveChangesAsync(cancellationToken);

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
