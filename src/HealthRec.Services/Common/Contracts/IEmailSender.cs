using Essentials.Results;
using HealthRec.Services.Common.Models;

namespace HealthRec.Services.Common.Contracts;

public interface IEmailSender
{
    Task<StandardResult> SendEmailAsync(EmailModel model);
}