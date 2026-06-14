using BloodDonation.Application.Interfaces;
using BloodDonation.Domain.Entities;
using BloodDonation.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BloodDonation.Application.Features.BloodRequests.Commands.CompleteDonation;
public sealed class CompleteDonationHandler: IRequestHandler<CompleteDonationCommand, bool>
{

    private readonly IApplicationDbContext _dbContext;

    public CompleteDonationHandler(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<bool> Handle( CompleteDonationCommand request,CancellationToken cancellationToken)
    {

        var bloodRequest =
            await _dbContext.BloodRequests
            .Include(x => x.Hospital)
            .FirstOrDefaultAsync(x => x.Id == request.BloodRequestId, cancellationToken);

        if (bloodRequest is null)
            return false;

        if (bloodRequest.Status != RequestStatus.Accepted)
            return false;

        var acceptance =
            await _dbContext.BloodRequestAcceptances
            .FirstOrDefaultAsync(x =>x.BloodRequestId == request.BloodRequestId &&
                x.DonorId == request.DonorId &&
                x.Status == AcceptanceStatus.Accepted,
                cancellationToken);

        if (acceptance is null)
            return false;

        if (bloodRequest.HospitalId.HasValue)
        {

            var hospital =await _dbContext.Hospitals
                .FirstOrDefaultAsync( x => x.Id == bloodRequest.HospitalId,
                    cancellationToken);

            if (hospital is null)
                return false;


            if (hospital.UserId != request.CompletedByUserId)
                throw new UnauthorizedAccessException();
        }

        else
        {
            if (bloodRequest.CreatedByUserId
                != request.CompletedByUserId)
                throw new UnauthorizedAccessException();
        }


        var donationHistory = new DonationHistory
        {
            Id = Guid.NewGuid(),
            DonorId = request.DonorId,
            PatientId = bloodRequest.CreatedByUserId,
            BloodRequestId = bloodRequest.Id,
            HospitalName =bloodRequest.Hospital?.Name?? bloodRequest.CustomHospitalName?? "Unknown",
            DonationDate = DateTime.UtcNow,
            Notes = request.Notes
        };



        await _dbContext.DonationHistories.AddAsync( donationHistory, cancellationToken);

        bloodRequest.Status =RequestStatus.Completed;

        await _dbContext.SaveChangesAsync(cancellationToken);

        return true;
    }
}