using HealthRec.Services.Patient.Contract;
using HealthRec.Services.Patient.Internal;
using Microsoft.Extensions.DependencyInjection;

namespace HealthRec.Services.Patient;

public static class DependencyInjection
{
    public static IServiceCollection AddPatientServices(this IServiceCollection services)
    {
        services.AddScoped<IPatientService, PatientService>();
        return services;
    }
}