using BloodDonation.Application.Interfaces;
using BloodDonation.Application.Interfaces.Repositories;
using BloodDonation.Infrastructure.Hubs;
using BloodDonation.Infrastructure.Persistence;
using BloodDonation.Infrastructure.Repositories;
using BloodDonation.Infrastructure.Services;
using Microsoft.AspNetCore.SignalR;
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
                sql => { sql.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName); }));

        services.AddScoped<IApplicationDbContext>(provider =>
            provider.GetRequiredService<AppDbContext>());

        // Repositories
        services.AddScoped<IBloodRequestRepository, BloodRequestRepository>();

        // Services
        services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>(); 
        services.AddScoped<IFileService, FileService>();
        services.AddScoped<IDonorService, DonorService>();
        services.AddScoped<IDonorMatchingService, DonorMatchingService>();
        services.AddScoped<IAiMatchingPipelineService, NotificationAgentService>();
        services.AddScoped<IDashboardService, DashboardService>();
        services.AddScoped<IHospitalService, HospitalService>();
        services.AddScoped<INotificationService, NotificationService>();
        services.AddScoped<INotificationSender, SignalRNotificationSender>();
        services.AddSingleton<IUserIdProvider, NotificationUserIdProvider>();
        services.AddSingleton<INotificationAgentQueue, NotificationAgentQueue>();
        services.AddHostedService<NotificationAgentBackgroundService>();
        services.AddHostedService<BloodRequestAgentWorker>();
        services.AddScoped<IOcrService, OcrService>();
        services.AddSingleton<IOcrVerificationQueue, OcrVerificationQueue>();
        services.AddHostedService<OcrBackgroundService>();

        return services;
    }
}
