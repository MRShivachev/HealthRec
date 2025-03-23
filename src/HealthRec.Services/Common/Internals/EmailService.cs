using System;
using System.Threading.Tasks;
using Essentials.Results;
using HealthRec.Services.Common.Contracts;
using HealthRec.Services.Common.Models;
using Microsoft.Extensions.DependencyInjection;

namespace HealthRec.Services.Common.Internals;

internal class EmailService : IEmailService
{
    private readonly IEmailSender emailSender;

    public EmailService(IEmailSender emailSender)
    {
        this.emailSender = emailSender;
    }

    public async Task<StandardResult> SendEmailAsync(EmailModel model)
    {
        return await this.emailSender.SendEmailAsync(model);
    }
}