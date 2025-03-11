namespace HealthRec.Services.Identity.Constants;

public class DefaultRoles
{
    public const string Admin = "Admin";
    public const string Patient = "Patient";
    public const string Doctor = "Doctor";
    public static readonly string[] List = [ Admin, Doctor, Patient ];
}