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
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection"),
                sql =>
                {
                    sql.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName);
                }));

        services.AddScoped<IApplicationDbContext>(provider =>
            provider.GetRequiredService<AppDbContext>());

        // Repositories
        services.AddScoped<IBloodRequestRepository, BloodRequestRepository>();
        //services 
        services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
        // تسجيل خدمة المتبرعين في السيرفر
        services.AddScoped<IDonorService, DonorService>();
        services.AddScoped<IDashboardService, DashboardService>();
        services.AddScoped<INotificationService, NotificationService>();


        // Services
        services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();

        return services;
    }
}
