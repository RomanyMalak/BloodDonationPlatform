using BloodDonation.Application.Features.BloodRequests.Commands.CreateBloodRequest;
using Microsoft.Extensions.DependencyInjection;

namespace BloodDonation.Application.Extensions;

public static class ApplicationServiceExtensions
{
    public static IServiceCollection AddApplication(
        this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(
                typeof(CreateBloodRequestHandler).Assembly));

        return services;
    }
}