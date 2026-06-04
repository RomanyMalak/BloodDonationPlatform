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

    public AcceptBloodRequestCommandHandler(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<bool> Handle(
        AcceptBloodRequestCommand request,
        CancellationToken cancellationToken)
    {
        var bloodRequest = await _dbContext.BloodRequests
            .FirstOrDefaultAsync(x => x.Id == request.BloodRequestId, cancellationToken);

        if (bloodRequest is null) return false;

        // بس الطلبات الـ Approved تنقبل دلوقتي
        // لما يجي الـ AI Pipeline هيبقى Matching كمان
        if (bloodRequest.Status != RequestStatus.Approved)
            return false;

        // تأكد إن المتبرع مش عمل accept قبل كده
        var alreadyAccepted = await _dbContext.BloodRequestAcceptances
            .AnyAsync(x =>
                x.BloodRequestId == request.BloodRequestId &&
                x.DonorId == request.DonorId,
                cancellationToken);

        if (alreadyAccepted) return false;

        var acceptance = new BloodRequestAcceptance
        {
            Id = Guid.NewGuid(),
            BloodRequestId = request.BloodRequestId,
            DonorId = request.DonorId,
            AcceptedAt = DateTime.UtcNow,
            Status = AcceptanceStatus.Accepted
        };

        bloodRequest.Status = RequestStatus.Accepted;

        await _dbContext.BloodRequestAcceptances.AddAsync(acceptance, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return true;
    }
}