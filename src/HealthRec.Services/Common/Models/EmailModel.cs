namespace HealthRec.Services.Common.Models;

public class EmailModel
{
    public required string Email { get; set; }

    public required string Subject { get; set; }

    public required string? To { get; set; }
    public required string Message { get; set; }

    public required string? Body { get; set; }
}
