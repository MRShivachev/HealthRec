using HealthRec.Services.Common.Contracts;
using HealthRec.Services.Common.Internals;
using Microsoft.Extensions.DependencyInjection;

namespace HealthRec.Services;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddHealthRecServices(this IServiceCollection services)
    {
        services.AddScoped<IEmailService, EmailService>();
        
        return services;
    }
}