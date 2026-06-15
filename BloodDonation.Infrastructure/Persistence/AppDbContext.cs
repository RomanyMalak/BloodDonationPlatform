using BloodDonation.Application.Interfaces;
using BloodDonation.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BloodDonation.Infrastructure.Persistence;

public class AppDbContext : DbContext, IApplicationDbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Hospital> Hospitals => Set<Hospital>();
    public DbSet<BloodRequest> BloodRequests => Set<BloodRequest>();
    public DbSet<BloodRequestAcceptance> BloodRequestAcceptances => Set<BloodRequestAcceptance>();
    public DbSet<Notification> Notifications => Set<Notification>();
    public DbSet<AiMatchingLog> AiMatchingLogs => Set<AiMatchingLog>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<UserReport> UserReports => Set<UserReport>();
    public DbSet<DonationHistory> DonationHistories => Set<DonationHistory>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.FullName).HasMaxLength(150).IsRequired();
            entity.Property(x => x.Email).HasMaxLength(200).IsRequired();
            entity.HasIndex(x => x.Email).IsUnique();
        });

        modelBuilder.Entity<Hospital>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Name).HasMaxLength(200).IsRequired();
            entity.Property(x => x.City).HasMaxLength(100);
            entity.Property(x => x.AddressDetail).HasMaxLength(300);
            entity.Property(x => x.IsActive).HasDefaultValue(false);
            entity.Property(x => x.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            entity.HasIndex(x => x.UserId).IsUnique();

            entity.HasOne(x => x.User)
                .WithOne(x => x.Hospital)
                .HasForeignKey<Hospital>(x => x.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<BloodRequest>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.CustomHospitalName).HasMaxLength(200);
            entity.Property(x => x.MedicalDocumentUrl).HasMaxLength(500);
            entity.Property(x => x.Notes).HasMaxLength(500);
            entity.Property(x => x.ContactPhone).HasMaxLength(30);

            entity.HasOne(x => x.User)
                .WithMany(x => x.BloodRequests)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(x => x.Hospital)
                .WithMany(x => x.BloodRequests)
                .HasForeignKey(x => x.HospitalId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<BloodRequestAcceptance>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.HasIndex(x => new { x.BloodRequestId, x.DonorId }).IsUnique();

            entity.HasOne(x => x.BloodRequest)
                .WithMany(x => x.Acceptances)
                .HasForeignKey(x => x.BloodRequestId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(x => x.Donor)
                .WithMany(x => x.AcceptedRequests)
                .HasForeignKey(x => x.DonorId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Title).HasMaxLength(150).IsRequired();
            entity.Property(x => x.Message).HasMaxLength(500).IsRequired();
            entity.Property(x => x.Type).HasMaxLength(50);

            entity.HasOne(x => x.User)
                .WithMany(x => x.Notifications)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(x => x.BloodRequest)
                .WithMany(x => x.Notifications)
                .HasForeignKey(x => x.BloodRequestId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<AiMatchingLog>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.HasOne(x => x.BloodRequest)
                .WithMany(x => x.AiMatchingLogs)
                .HasForeignKey(x => x.BloodRequestId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Token).HasMaxLength(500).IsRequired();
            entity.HasOne(x => x.User)
                .WithMany(x => x.RefreshTokens)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<UserReport>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Reason).HasMaxLength(500).IsRequired();
            entity.HasOne(x => x.Reporter)
                .WithMany(x => x.ReportsMade)
                .HasForeignKey(x => x.ReporterId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(x => x.ReportedUser)
                .WithMany(x => x.ReportsReceived)
                .HasForeignKey(x => x.ReportedUserId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<DonationHistory>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.HospitalName).HasMaxLength(200).IsRequired();
            entity.Property(x => x.Notes).HasMaxLength(500);
            entity.HasOne(x => x.Donor)
                .WithMany(x => x.DonationsMade)
                .HasForeignKey(x => x.DonorId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(x => x.Patient)
                .WithMany(x => x.DonationsReceived)
                .HasForeignKey(x => x.PatientId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(x => x.BloodRequest)
                .WithMany(x => x.DonationHistories)
                .HasForeignKey(x => x.BloodRequestId)
                .OnDelete(DeleteBehavior.SetNull);
        });
    }
}
