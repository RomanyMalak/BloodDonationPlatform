
using BloodDonation.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BloodDonation.Application.Interfaces;

public interface IApplicationDbContext
{
    DbSet<User> Users { get; }
    public DbSet<BloodRequest> BloodRequests { get; }

    Task<int> SaveChangesAsync(
        CancellationToken cancellationToken);
}