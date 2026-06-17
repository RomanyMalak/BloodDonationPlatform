
using BloodDonation.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace BloodDonation.Infrastructure.Persistence;

public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
       optionsBuilder.UseSqlServer("Server=.;Database=BloodDonationDb;Trusted_Connection=True;TrustServerCertificate=True;");
        //  optionsBuilder.UseSqlServer("Server=DESKTOP-A3J4UMH\\SQLEXPRESS;Database=BloodDonationDb;Trusted_Connection=True;TrustServerCertificate=True;");
       // optionsBuilder.UseSqlServer("Server=db56229.databaseasp.net; Database=db56229; User Id=db56229; Password=Am5_Bo2J; Encrypt=True;TrustServerCertificate=True; MultipleActiveResultSets=True;");
        return new AppDbContext(optionsBuilder.Options);
    }
}
                                                     