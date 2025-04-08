using HealthRec.Services.Record.Contract;
using HealthRec.Services.Record.Internal;
using Microsoft.Extensions.DependencyInjection;

namespace HealthRec.Services.Record;

public static class DependencyInjection
{
    public static IServiceCollection AddRecordServices(this IServiceCollection services)
    {
        services.AddScoped<IRecordService, RecordService>();
        return services;
    }
}