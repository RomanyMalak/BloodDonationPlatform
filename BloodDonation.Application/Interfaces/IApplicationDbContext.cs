using BloodDonation.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BloodDonation.Application.Interfaces;

public interface IApplicationDbContext
{
    DbSet<User> Users { get; }
    DbSet<BloodRequest> BloodRequests { get; }
    DbSet<BloodRequestAcceptance> BloodRequestAcceptances { get; }
    DbSet<Hospital> Hospitals { get; }
    DbSet<Notification> Notifications { get; }
    DbSet<DonationHistory> DonationHistories { get; }
    DbSet<OcrVerification>  OcrVerifications { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
