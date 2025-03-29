namespace HealthRec.Services.Identity.Contracts;

public interface ICurrentUser
{
    Guid? UserId { get; }
    bool Exists { get; }
}