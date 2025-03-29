using System.Security.Claims;
using HealthRec.Common;
using HealthRec.Data;
using HealthRec.Data.Entities;
using HealthRec.Presentation.Extensions;
using HealthRec.Presentation.Models;
using HealthRec.Services.Doctor.Models;
using HealthRec.Services.Identity.Constants;
using HealthRec.Services.Patient.Contract;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HealthRec.Presentation.Controllers;

[Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
[Route("medical-records")]
[AllowAnonymous]
public class MedicalRecordsController : Controller
{
    private readonly ILogger<MedicalRecordsController> logger;
    private readonly HealthRecDbContext context;
    private readonly IPatientService patientService;
    private readonly UserManager<ApplicationUser> userManager;
    private readonly SignInManager<ApplicationUser> signInManager;

    public MedicalRecordsController(
        ILogger<MedicalRecordsController> logger,
        HealthRecDbContext context,
        IPatientService patientService,
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager)
    {
        this.logger = logger;
        this.context = context;
        this.patientService = patientService;
        this.userManager = userManager;
        this.signInManager = signInManager;
    }

    [HttpGet("/security-code")]
    [AllowAnonymous]
    public IActionResult VerifySecurityCode()
    {
        if (this.IsUserAuthenticated())
        {
            return this.RedirectToDefault();
        }

        var model = new SecurityCodeViewModel();
        return this.View(model);
    }

    [HttpPost("/security-code")]
    [ValidateAntiForgeryToken]
    [AllowAnonymous]
    public async Task<IActionResult> VerifySecurityCode(SecurityCodeViewModel model)
    {
        if (this.IsUserAuthenticated())
        {
            return this.RedirectToDefault();
        }

        if (this.ModelState.IsValid)
        {
            // Authenticate using the patient service
            if (model.SecurityCode != null)
            {
                var patient = await this.patientService.AuthenticateBySecurityCodeAsync(model.SecurityCode);

                if (patient == null)
                {
                    this.ViewBag.ErrorMessage = "Invalid security code. Please check and try again.";
                    return this.View(model);
                }

                var user = await this.userManager.FindByEmailAsync(patient.Email!);
                if (user == null)
                {
                    this.ModelState.AddModelError(string.Empty, T.InvalidLoginErrorMessage);
                    return this.View(model);
                }

                // Create claims for this patient
                if (await this.userManager.IsLockedOutAsync(user))
                {
                    this.ModelState.AddModelError(string.Empty, T.UserLockedOutErrorMessage);
                    return this.View(model);
                }

                await this.SignInAsync(user, model.RememberMe);
                return this.RedirectToDefault();
            }

            // Redirect to the home page or patient records
            return this.RedirectToDefault();
        }

        return this.View(model);
    }

    // For authenticated patients to view their own records
    [HttpGet]
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme, Roles = "Patient")]
    public IActionResult MyRecords()
    {
        // In a real application, you would fetch this data from your database
        // based on the current user's identity

        var viewModel = new PatientRecordsViewModel
        {
            PatientName = "John Smith",
            DateOfBirth = new DateTime(1985, 5, 15),
            MedicalRecordNumber = "MRN12345",
            Records = this.GetSampleMedicalRecords(),
        };

        return this.View(viewModel);
    }

    // For authenticated doctors to view a specific patient's records
    [HttpGet]
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme, Roles = "Doctor")]
    public IActionResult DoctorViewPatientRecords(int patientId)
    {
        // In a real application, you would fetch this data from your database
        // based on the provided patientId

        var viewModel = new PatientRecordsViewModel
        {
            PatientName = "John Smith",
            DateOfBirth = new DateTime(1985, 5, 15),
            MedicalRecordNumber = "MRN12345",
            Records = this.GetSampleMedicalRecords(),
        };

        return this.View("PatientRecords", viewModel); // Reuse the same view
    }

    // Display patient records after security code verification
    [HttpGet("patient-records")]
    [Authorize(DefaultPolicies.PatientPolicy)] // Ensure only authenticated patients can access
    public async Task<IActionResult> PatientRecords(Guid patientId)
    {
        // Get the patient details
        var patient = await this.patientService.GetByIdAsync(patientId);
        if (patient == null)
        {
            return this.NotFound();
        }

        // Get the patient's single doctor
        var doctorPatient = await this.context.DoctorPatients
            .FirstOrDefaultAsync(dp => dp.PatientId == patientId);

        DoctorViewModel? doctorViewModel = null;

        if (doctorPatient != null)
        {
            var doctor = await this.context.Doctors
                .FirstOrDefaultAsync(d => d.Id == doctorPatient.DoctorId);

            if (doctor != null)
            {
                doctorViewModel = new DoctorViewModel
                {
                    Id = doctor.Id,
                    Name = $"{doctor.FirstName} {doctor.LastName}",
                    Specialisation = (SpecialisationModel?)doctor.Specialisation,
                };
            }
        }

        // Prepare view model with a single doctor or empty list
        var viewModel = new PatientRecordsViewModel
        {
            PatientName = $"{patient.FirstName} {patient.LastName}",
            DateOfBirth = (DateTime)patient.DateOfBirth!,
            MedicalRecordNumber = $"MRN-{patientId.ToString().Substring(0, 8)}",
            Records = this.GetSampleMedicalRecords(), // In a real app, get actual records
            Doctors = doctorViewModel != null
                ? new List<DoctorViewModel> { doctorViewModel }
                : new List<DoctorViewModel>(),
        };

        return this.View("PatientRecords", new PatientRecordsViewModel());
    }

    private async Task<List<DoctorViewModel>> GetPatientDoctor(Guid patientId)
    {
        // Get the doctor associated with this patient
        var doctorPatient = await this.context.DoctorPatients
            .FirstOrDefaultAsync(dp => dp.PatientId == patientId);

        if (doctorPatient == null)
        {
            return new List<DoctorViewModel>();
        }

        var doctor = await this.context.Doctors
            .FirstOrDefaultAsync(d => d.Id == doctorPatient.DoctorId);

        if (doctor == null)
        {
            return new List<DoctorViewModel>();
        }

        return new List<DoctorViewModel>
        {
            new DoctorViewModel
            {
                Id = doctor.Id,
                Name = $"{doctor.FirstName} {doctor.LastName}",
                Specialisation = (SpecialisationModel?)doctor.Specialisation,
            },
        };
    }

    // Sample data generation methods - in a real app these would be database queries
    private List<MedicalRecordViewModel> GetSampleMedicalRecords()
    {
        return new List<MedicalRecordViewModel>
        {
            new MedicalRecordViewModel
            {
                Id = 1,
                Title = "Annual Physical Examination",
                Description = "Routine annual physical examination with standard lab work.",
                RecordDate = DateTime.Now.AddDays(-30),
                RecordType = "Appointment",
                DoctorName = "Sarah Johnson",
                DoctorSpecialty = "General Medicine",
                Department = "Primary Care",
                CreatedDate = DateTime.Now.AddDays(-30),
                Notes = "Patient is in good health overall. Recommended to maintain current exercise routine and diet.",
            },
            new MedicalRecordViewModel
            {
                Id = 2,
                Title = "Blood Test Results",
                Description = "Complete blood count and metabolic panel.",
                RecordDate = DateTime.Now.AddDays(-28),
                RecordType = "Test",
                DoctorName = "Sarah Johnson",
                DoctorSpecialty = "General Medicine",
                Department = "Laboratory",
                CreatedDate = DateTime.Now.AddDays(-28),
                TestResults =
                    "All values within normal range. Cholesterol slightly elevated but not of immediate concern.",
                NormalRange = "HDL: 40-60 mg/dL, LDL: <100 mg/dL, Total Cholesterol: <200 mg/dL",
                Notes = "Follow-up in 6 months recommended for cholesterol monitoring.",
            },
            new MedicalRecordViewModel
            {
                Id = 3,
                Title = "Hypertension Diagnosis",
                Description = "Initial diagnosis of stage 1 hypertension.",
                RecordDate = DateTime.Now.AddDays(-14),
                RecordType = "Diagnosis",
                DoctorName = "Michael Chen",
                DoctorSpecialty = "Cardiology",
                Department = "Cardiovascular Health",
                CreatedDate = DateTime.Now.AddDays(-14),
                DiagnosisCode = "I10",
                TreatmentPlan =
                    "Lifestyle modifications including reduced sodium intake, increased physical activity. Recheck blood pressure in 4 weeks.",
                Notes = "Patient advised on home blood pressure monitoring techniques.",
            },
        };
    }

    private async Task SignInAsync(
        ApplicationUser user,
        bool rememberMe)
    {
        var claimsPrinciple = await this.signInManager
            .ClaimsFactory
            .CreateAsync(user);

        await this.HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            claimsPrinciple,
            new AuthenticationProperties
            {
                ExpiresUtc = DateTimeOffset.UtcNow.AddDays(30),
                IsPersistent = rememberMe,
            });
    }
}