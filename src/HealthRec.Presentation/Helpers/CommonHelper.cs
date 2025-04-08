using System.Security.Claims;
using HealthRec.Services.Doctor.Contract;

namespace HealthRec.Presentation.Helpers;

public class CommonHelper
{
    private readonly ClaimsPrincipal user;
    private readonly IServiceProvider? serviceProvider;

    public CommonHelper(ClaimsPrincipal user, IServiceProvider? serviceProvider = null)
    {
        this.user = user;
        this.serviceProvider = serviceProvider;
    }

    public string GetUserRole()
    {
        var roleClaim = this.user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role);
        return roleClaim?.Value ?? "Patient";
    }

    public Guid? GetUserId()
    {
        var userIdClaim = this.user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
        if (userIdClaim != null && Guid.TryParse(userIdClaim.Value, out var userId))
        {
            return userId;
        }

        return null;
    }

    public bool IsAdmin()
    {
        return this.GetUserRole() == "Admin";
    }

    public bool IsDoctor()
    {
        return this.GetUserRole() == "Doctor";
    }

    public bool IsPatient()
    {
        return this.GetUserRole() == "Patient";
    }

    public async System.Threading.Tasks.Task<bool> IsDoctorForPatientAsync(Guid patientId)
    {
        if (this.IsAdmin())
        {
            return true;
        }

        if (!this.IsDoctor() || this.serviceProvider == null)
        {
            return false;
        }

        var userId = this.GetUserId();
        if (!userId.HasValue)
        {
            return false;
        }

        var doctorService = this.serviceProvider.GetRequiredService<IDoctorService>();
        return await doctorService.IsDoctorForPatientAsync(patientId, userId.Value);
    }

    public async System.Threading.Tasks.Task<bool> CanManagePatientsAsync(Guid patientId)
    {
        if (this.IsAdmin())
        {
            return true;
        }

        return await this.IsDoctorForPatientAsync(patientId);
    }

    public string GetFullName()
    {
        var givenName = this.user.FindFirstValue(ClaimTypes.GivenName);
        var surname = this.user.FindFirstValue(ClaimTypes.Surname);

        if (!string.IsNullOrEmpty(givenName) && !string.IsNullOrEmpty(surname))
        {
            return $"{givenName} {surname}";
        }

        return this.user.FindFirstValue(ClaimTypes.Name) ?? "Unknown User";
    }
}