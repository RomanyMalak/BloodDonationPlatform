using BloodDonation.Application.Interfaces;
using BloodDonation.Domain.Entities;
using BloodDonation.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BloodDonation.Application.Features.BloodRequests.Commands.CompleteDonation;

public sealed class CompleteDonationHandler : IRequestHandler<CompleteDonationCommand, bool>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IWhatsAppService _whatsAppService;

    public CompleteDonationHandler(
        IApplicationDbContext dbContext,
        IWhatsAppService whatsAppService)
    {
        _dbContext = dbContext;
        _whatsAppService = whatsAppService;
    }

    public async Task<bool> Handle(CompleteDonationCommand request, CancellationToken cancellationToken)
    {
        var bloodRequest =
            await _dbContext.BloodRequests
            .Include(x => x.Hospital)
            .Include(x => x.Acceptances)
            .FirstOrDefaultAsync(x => x.Id == request.BloodRequestId, cancellationToken);

        if (bloodRequest is null)
            return false;

        if (bloodRequest.Status != RequestStatus.Accepted)
            return false;

        // مين المخوّل يقفل الطلب: موظف المستشفى (لو الطلب جاي عن طريق مستشفى مسجلة)
        // أو صاحب الطلب نفسه (لو الطلب جاي عن طريق OCR بدون مستشفى).
        if (bloodRequest.HospitalId.HasValue)
        {
            var hospital = await _dbContext.Hospitals
                .FirstOrDefaultAsync(x => x.Id == bloodRequest.HospitalId, cancellationToken);

            if (hospital is null)
                return false;

            if (hospital.UserId != request.CompletedByUserId)
                throw new UnauthorizedAccessException();
        }
        else
        {
            if (bloodRequest.CreatedByUserId != request.CompletedByUserId)
                throw new UnauthorizedAccessException();
        }

        // كل المتبرعين اللي قبلوا الطلب ده (Accepted) ولسه ماتأكدوش
        var acceptedDonors = bloodRequest.Acceptances
            .Where(x => x.Status == AcceptanceStatus.Accepted)
            .ToList();

        if (acceptedDonors.Count == 0)
            return false;

        var donorIds = acceptedDonors.Select(x => x.DonorId).ToList();

        var donors = await _dbContext.Users
            .Where(x => donorIds.Contains(x.Id))
            .ToListAsync(cancellationToken);

        var donationDate = DateTime.UtcNow;
        var hospitalName = bloodRequest.Hospital?.Name ?? bloodRequest.CustomHospitalName ?? "Unknown";

        foreach (var acceptance in acceptedDonors)
        {
            var donationHistory = new DonationHistory
            {
                Id = Guid.NewGuid(),
                DonorId = acceptance.DonorId,
                PatientId = bloodRequest.CreatedByUserId,
                BloodRequestId = bloodRequest.Id,
                HospitalName = hospitalName,
                DonationDate = donationDate
            };

            await _dbContext.DonationHistories.AddAsync(donationHistory, cancellationToken);

            acceptance.Status = AcceptanceStatus.Completed;

            var donor = donors.FirstOrDefault(x => x.Id == acceptance.DonorId);
            if (donor is not null)
            {
                donor.LastDonationDate = donationDate;
            }
        }

        bloodRequest.Status = RequestStatus.Completed;

        await _dbContext.SaveChangesAsync(cancellationToken);

        const string thankYouMessage = """
            ❤️ Thank you for saving a life.

            Your blood donation has been completed successfully.
            """;

        foreach (var donor in donors)
        {
            await _whatsAppService.SendAsync(
                donor.Phone,
                thankYouMessage,
                cancellationToken);
        }

        await _whatsAppService.SendAsync(
            bloodRequest.Hospital?.Hotline,
            thankYouMessage,
            cancellationToken);

        return true;
    }
}
