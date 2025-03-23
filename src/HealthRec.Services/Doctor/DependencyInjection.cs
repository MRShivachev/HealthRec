using HealthRec.Services.Doctor.Contract;
using HealthRec.Services.Doctor.Internal;
using Microsoft.Extensions.DependencyInjection;

namespace HealthRec.Services.Doctor;

public static class DependencyInjection
{
    public static IServiceCollection AddDoctorServices(this IServiceCollection services)
    {
        services.AddScoped<IDoctorService, DoctorService>();
        return services;
    }
}