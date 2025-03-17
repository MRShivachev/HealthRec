using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using HealthRec.Data;
using HealthRec.Presentation.Extensions;
using HealthRec.Presentation.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

[Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
[AllowAnonymous]
public class MedicalRecordsController : Controller
{
    private readonly ILogger<MedicalRecordsController> _logger;
    private readonly HealthRecDbContext context;

    public MedicalRecordsController(ILogger<MedicalRecordsController> logger,
        HealthRecDbContext context)
    {
        _logger = logger;
        this.context = context;
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
            // In a real application, you would:
            // 1. Look up the security code in your database
            // 2. Check which patient it belongs to
            // 3. Create a temporary session or token for that patient
            var patient = this.context.Patients.FirstOrDefault(x => x.Code == model.SecurityCode);
            if (patient != null)
            {
                var claims = new List<System.Security.Claims.Claim>
                {
                    new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.NameIdentifier, patient.Id.ToString()),
                    new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Name, "John Smith"),
                    new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Role, "Patient"),
                    new System.Security.Claims.Claim("SecurityCodeAccess", "true"),
                    new System.Security.Claims.Claim("PatientId", patient.Id.ToString()),
                };
            }
            
            
            // This is a simplified example - in a real app, don't hardcode security codes
            if (model.SecurityCode == "12345678")
            {
                // For demonstration, we're creating a claims identity for the patient
                // In a real app, you'd look up the patient details based on the security code

                // Create claims for this patient
                var patientId = "P12345"; // This would come from your database lookup
                
/*
                // Create claimsPrincipal - in your app, you might use a different approach aligned with SignInAsync
                var identity = new System.Security.Claims.ClaimsIdentity(claims, "SecurityCode");
                var claimsPrincipal = new System.Security.Claims.ClaimsPrincipal(identity);
                */

                // Sign in the user with a temporary authentication
                

                // Redirect to the medical records
                return this.RedirectToAction("PatientRecords");
            }

            this.ModelState.AddModelError(string.Empty, "Invalid security code. Please check and try again.");
        }

        return this.View(model);
    }

    // Display patient records after security code verification
    [HttpGet("patient-records")]
    [AllowAnonymous] // No login required, but security code verified
    public IActionResult PatientRecords(string securityCode)
    {
        // Verify the security code again as a security measure
        // In a real app, you might use TempData or a more secure method
        if (string.IsNullOrEmpty(securityCode) || securityCode != "12345678")
        {
            return this.RedirectToAction("VerifySecurityCode");
        }

        // Here you would look up the patient associated with this security code
        // and retrieve their records

        var viewModel = new PatientRecordsViewModel
        {
            PatientName = "John Smith",
            DateOfBirth = new DateTime(1985, 5, 15),
            MedicalRecordNumber = "MRN12345",
            Records = GetSampleMedicalRecords(),
            Doctors = GetSampleDoctors()
        };

        return View(viewModel);
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
            Records = GetSampleMedicalRecords(),
            Doctors = GetSampleDoctors()
        };

        return View(viewModel);
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
            Records = GetSampleMedicalRecords(),
            Doctors = GetSampleDoctors()
        };

        return View("PatientRecords", viewModel); // Reuse the same view
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
                Notes = "Patient is in good health overall. Recommended to maintain current exercise routine and diet."
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
                Notes = "Follow-up in 6 months recommended for cholesterol monitoring."
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
                Notes = "Patient advised on home blood pressure monitoring techniques."
            }
        };
    }

    private List<DoctorViewModel> GetSampleDoctors()
    {
        return new List<DoctorViewModel>
        {
            new DoctorViewModel { Id = 1, Name = "Sarah Johnson", Specialty = "General Medicine" },
            new DoctorViewModel { Id = 2, Name = "Michael Chen", Specialty = "Cardiology" },
            new DoctorViewModel { Id = 3, Name = "Emily Rodriguez", Specialty = "Family Medicine" }
        };
    }
}