using BloodDonation.Application.Interfaces;
using BloodDonation.Application.Interfaces.Repositories;
using BloodDonation.Infrastructure.Persistence;
using BloodDonation.Infrastructure.Repositories;
using BloodDonation.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BloodDonation.Infrastructure.Extensions;

public static class InfrastructureServiceExtensions
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Database
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<IApplicationDbContext>(sp =>
            sp.GetRequiredService<AppDbContext>());

        // Repositories
        services.AddScoped<IBloodRequestRepository, BloodRequestRepository>();

        // Services
        services.AddScoped<IDashboardService, DashboardService>();
        services.AddScoped<IHospitalService, HospitalService>();

        return services;
    }
}
