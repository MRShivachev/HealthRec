using HealthRec.Services.Common;
using HealthRec.Services.Doctor;
using HealthRec.Services.Identity;
using HealthRec.Services.Patient;
using HealthRec.Services.Record;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HealthRec.Services;

public static class DependencyInjection
{
    public static IServiceCollection AddServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddCommonServices(configuration);
        services.AddIdentityServices();
        services.AddDoctorServices();
        services.AddPatientServices();
        services.AddRecordServices();
        return services;
    }
}