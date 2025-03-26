using System.Security.Claims;
using HealthRec.Data;
using HealthRec.Presentation.Extensions;
using HealthRec.Presentation.Models;
using HealthRec.Services.Doctor.Models;
using HealthRec.Services.Identity.Constants;
using HealthRec.Services.Patient.Contract;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HealthRec.Presentation.Controllers;

[Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
[AllowAnonymous]
public class MedicalRecordsController : Controller
{
    private readonly ILogger<MedicalRecordsController> logger;
    private readonly HealthRecDbContext context;
    private readonly IPatientService patientService;

    public MedicalRecordsController(
        ILogger<MedicalRecordsController> logger,
        HealthRecDbContext context,
        IPatientService patientService)
    {
        this.logger = logger;
        this.context = context;
        this.patientService = patientService;
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

                // Create claims for this patient
                if (patient.Email != null)
                {
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier, patient.Id.ToString()),
                        new Claim(ClaimTypes.Name, $"{patient.FirstName} {patient.LastName}"),
                        new Claim(ClaimTypes.Email, patient.Email),
                        new Claim(ClaimTypes.Role, DefaultRoles.Patient),
                        new Claim("SecurityCodeAccess", "true"),
                        new Claim("PatientId", patient.Id.ToString()),
                    };

                    // Create the identity and principal
                    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var principal = new ClaimsPrincipal(identity);

                    // Set authentication properties
                    var authProperties = new AuthenticationProperties
                    {
                        IsPersistent = true, // Make the cookie persistent
                        ExpiresUtc = DateTimeOffset.UtcNow.AddHours(24), // Expire after 24 hours
                    };

                    // Sign in the user
                    await this.HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        principal,
                        authProperties);
                }

                this.logger.LogInformation(
                    "Patient {PatientId} logged in with security code at {Time}",
                    patient.Id,
                    DateTime.UtcNow);
            }

            // Redirect to the home page or patient records
            return this.RedirectToAction("Index", "Home");
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
    [Authorize(Roles = "Patient")] // Ensure only authenticated patients can access
    public async Task<IActionResult> PatientRecords()
    {
        // Get current patient ID from claims
        var patientIdClaim = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(patientIdClaim) || !Guid.TryParse(patientIdClaim, out var patientId))
        {
            return this.RedirectToAction("Login", "Authentication");
        }

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

        return this.View(viewModel);
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
}