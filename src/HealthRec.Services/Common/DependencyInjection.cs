using HealthRec.Services.Common.Contracts;
using HealthRec.Services.Common.Internals;
using HealthRec.Services.Common.Internals.EmailSenders;
using HealthRec.Services.Common.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HealthRec.Services.Common;

public static class DependencyInjection
{
    public static IServiceCollection AddCommonServices(
        this IServiceCollection services, IConfiguration configuration)
    {
        // Register SendGrid API key from configuration
        services.Configure<EmailSendGridOptions>(configuration.GetSection("Emails:SendGrid"));

        // Register EmailService
        services.AddScoped<IEmailSender, SendGridSender>();
        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<ISecurityCodeGenerator, SecurityCodeCodeGenerator>();

        return services;
    }
}