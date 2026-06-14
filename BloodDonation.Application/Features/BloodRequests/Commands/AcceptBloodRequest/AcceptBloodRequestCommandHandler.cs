using BloodDonation.Application.Interfaces;
using BloodDonation.Domain.Entities;
using BloodDonation.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BloodDonation.Application.Features.BloodRequests.Commands.AcceptBloodRequest;


public sealed class AcceptBloodRequestCommandHandler
    : IRequestHandler<AcceptBloodRequestCommand, bool>
{

    private readonly IApplicationDbContext _dbContext;


    public AcceptBloodRequestCommandHandler(
        IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<bool> Handle(
        AcceptBloodRequestCommand request,
        CancellationToken cancellationToken)
    {
        var bloodRequest =
            await _dbContext.BloodRequests
            .Include(x => x.Acceptances)
            .FirstOrDefaultAsync(x => x.Id == request.BloodRequestId,cancellationToken);


        if (bloodRequest is null)
            return false;

        if (bloodRequest.Status != RequestStatus.Matching)
            return false;

        var alreadyAccepted =bloodRequest.Acceptances.Any(x =>x.DonorId == request.DonorId);

        if (alreadyAccepted)
            return false;

        var acceptance = new BloodRequestAcceptance
        {
            Id = Guid.NewGuid(),
            BloodRequestId = bloodRequest.Id,
            DonorId = request.DonorId,
            AcceptedAt = DateTime.UtcNow,
            Status = AcceptanceStatus.Accepted
        };

        await _dbContext.BloodRequestAcceptances.AddAsync(acceptance, cancellationToken);

        var acceptedCount =bloodRequest.Acceptances.Count + 1;

        if (acceptedCount >= bloodRequest.UnitsNeeded)
        {
            bloodRequest.Status =RequestStatus.Accepted;
        }

        await _dbContext.SaveChangesAsync(cancellationToken);

        return true;
    }
}