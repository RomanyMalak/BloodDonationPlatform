using BloodDonation.Application.Interfaces;
using BloodDonation.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BloodDonation.Application.Features.BloodRequests.Commands.CancelBloodRequest;

public sealed class CancelBloodRequestCommandHandler
    : IRequestHandler<CancelBloodRequestCommand, bool>
{
    private readonly IApplicationDbContext _dbContext;

    public CancelBloodRequestCommandHandler(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<bool> Handle(
        CancelBloodRequestCommand request,
        CancellationToken cancellationToken)
    {
        var bloodRequest = await _dbContext.BloodRequests
            .FirstOrDefaultAsync(x => x.Id == request.BloodRequestId, cancellationToken);

        if (bloodRequest is null) return false;


        if (bloodRequest.CreatedByUserId != request.UserId)
            return false;

        if (bloodRequest.Status == RequestStatus.Matching ||
            bloodRequest.Status == RequestStatus.Accepted ||
            bloodRequest.Status == RequestStatus.Completed)
        {
            return false;
        }

        bloodRequest.Status = RequestStatus.Cancelled;
        await _dbContext.SaveChangesAsync(cancellationToken);

        return true;
    }
}