using System.Threading.Tasks;
using HealthRec.Services.Common.Contracts;
using HealthRec.Services.Common.Models;
using Essentials.Results;

namespace HealthRec.Services.Identity.Extensions;

public static class EmailServiceExtensions
{
    public static async Task<StandardResult> SendResetPasswordEmailAsync(
        this IEmailService emailService,
        string email,
        string token)
    {
        return await emailService.SendEmailAsync(
            new EmailModel
            {
                Email = email,
                Subject = HealthRec.Common.T.ResetPasswordTitle,
                Message = string.Format(HealthRec.Common.Emails.ResetPassword,
                    token),
                To = null,
                Body = null,
            });
    }
}