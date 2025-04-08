using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using HealthRec.Data;
using HealthRec.Data.Entities;
using HealthRec.Presentation.Extensions;
using HealthRec.Presentation.Models;
using HealthRec.Services.Doctor.Contract;
using HealthRec.Services.Identity.Constants;
using HealthRec.Services.Identity.Contracts;
using HealthRec.Services.Patient.Contract;
using HealthRec.Services.Record.Contract;
using HealthRec.Services.Record.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HealthRec.Presentation.Controllers;

[Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
[Route("records")]
public class RecordController : Controller
{
    private readonly HealthRecDbContext context;
    private readonly ILogger<RecordController> logger;
    private readonly IPatientService patientService;
    private readonly IDoctorService doctorService;
    private readonly ICurrentUser currentUser;
    private readonly IRecordService recordService;

    public RecordController(
        HealthRecDbContext context,
        ILogger<RecordController> logger,
        IPatientService patientService,
        IDoctorService doctorService,
        ICurrentUser currentUser,
        IRecordService recordService)
    {
        this.context = context;
        this.logger = logger;
        this.patientService = patientService;
        this.doctorService = doctorService;
        this.currentUser = currentUser;
        this.recordService = recordService;
    }

    // GET: Records/MyRecords
    [HttpGet("my-records")]
    public async Task<IActionResult> MyRecords(Guid patientId)
    {
        var patient = await this.patientService.GetByIdAsync(patientId);
        if (patient == null)
        {
            return this.NotFound();
        }

        var records = await this.recordService.GetRecordsByPatientIdAsync(patientId);
        var viewModel = new PatientRecordsViewModel
        {
            PatientName = $"{patient.FirstName} {patient.LastName}",
            PatientId = patientId,
            DateOfBirth = patient.DateOfBirth,
            MedicalRecordNumber = $"MRN-{patient.Id.ToString().Substring(0, 8)}",
            Records = records.Select(r => new MedicalRecordViewModel
            {
                Id = r.Id, // Set if needed
                Title = r.Type.ToString(),
                Description = r.Description,
                RecordDate = r.RecordDate,
                RecordType = r.Type,
                TestResults = r.Result,
                NormalRange = r.ReferenceRange,
                MedicationDetails = r.MedicationDetails,
                MedicationName = r.MedicationName,
                DiagnosisCode = r.DiagnosisCode,
                Department = r.Department,
                Dosage = r.Dosage,
                DoctorName = r.DoctorName,
                DoctorSpecialty = r.DoctorSpecialty,
                VaccineDose = r.VaccineDose,
                PatientId = r.PatientId,
                Notes = r.Notes,
                Instructions = r.Instructions,
                Severity = r.Severity,
                TestType = r.TestType,
                Prescription = r.Prescription,
                TreatmentPlan = r.TreatmentPlan,
                CreatedDate = r.CreatedDate,
                VaccineName = r.VaccineName,
                Duration = r.Duration,
            }).ToList(),
        };

        return this.View(viewModel);
    }

    // GET: Records/Create
    [HttpGet("create")]
    [Authorize(Roles = "Doctor,Admin")]
    public async Task<IActionResult> Create(Guid patientId)
    {
        var patient = await this.patientService.GetByIdAsync(patientId);
        if (patient == null)
        {
            return this.NotFound();
        }

        var viewModel = new MedicalRecordViewModel
        {
            PatientId = patientId,
            CreatedDate = DateTime.Now,
            RecordDate = DateTime.Now,
        };

        return this.View(viewModel);
    }

    // POST: Records/Create
    [HttpPost("create")]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Doctor,Admin")]
    public async Task<IActionResult> Create(MedicalRecordViewModel model, IFormFile testResultFile = null!)
    {
        if (this.ModelState.IsValid)
        {
            // Get current doctor info
            var doctorId = this.currentUser.UserId!.Value;
            {
                var doctor = await this.doctorService.GetByIdAsync(doctorId);
                // Handle file upload if present
                if (testResultFile != null && testResultFile.Length > 0)
                {
                }

                // Add doctor information
                if (doctor != null)
                {
                    model.DoctorName = $"{doctor.FirstName} {doctor.LastName}";
                    model.DoctorSpecialty = doctor.Specialisation.ToString();
                }

                var record = new RecordModel
                {
                    Description = model.Description,
                    PatientId = model.PatientId,
                    Result = model.TestResults,
                    MedicationDetails = model.MedicationDetails,
                    NormalRange = model.NormalRange,
                    RecordDate = DateTime.Now,
                    Type = model.RecordType,
                    DoctorSpecialty = model.DoctorSpecialty,
                    Department = model.Department,
                    CreatedDate = DateTime.Now,
                    DiagnosisCode = model.DiagnosisCode,
                    TreatmentPlan = model.TreatmentPlan,
                    TestResults = model.TestResults,
                    Dosage = model.Dosage,
                    Instructions = model.Instructions,
                    Notes = model.Notes,
                    Severity = model.Severity,
                    TestType = model.TestType,
                    MedicationName = model.MedicationName,
                    Duration = model.Duration,
                    Prescription = model.Prescription,
                    VaccineName = model.VaccineName,
                    VaccineDose = model.VaccineDose,
                };

                var result = await this.recordService.CreateRecordAsync(record);
                if (result.Succeeded)
                {
                    return this.RedirectToAction(nameof(this.MyRecords), new { patientId = model.PatientId });
                }
            }
        }

        return this.View(model);
    }

    // GET: Records/Details/{id}
    [HttpGet("details")]
    [Authorize(Roles = "Doctor,Admin,Patient")]
    public async Task<IActionResult> Details(long id)
    {
        // In a real application, you would fetch the record from your database
        // For now, we'll use our sample data
        var record = this.GetSampleMedicalRecords(Guid.Empty).FirstOrDefault(r => r.Id == id);
        if (record == null)
        {
            return this.NotFound();
        }

        return this.View(record);
    }

    // Helper methods to generate sample data
    private List<MedicalRecordViewModel> GetSampleMedicalRecords(Guid patientId)
    {
        return new List<MedicalRecordViewModel>
        {
            new MedicalRecordViewModel
            {
                Id = 1,
                Title = "Annual Physical Examination",
                Description = "Routine annual physical examination with standard lab work.",
                RecordDate = DateTime.Now.AddDays(-30),
                DoctorName = "Sarah Johnson",
                DoctorSpecialty = "General Medicine",
                Department = "Primary Care",
                CreatedDate = DateTime.Now.AddDays(-30),
                Notes = "Patient is in good health overall. Recommended to maintain current exercise routine and diet.",
                PatientId = patientId,
            },
            new MedicalRecordViewModel
            {
                Id = 2,
                Title = "Blood Test Results",
                Description = "Complete blood count and metabolic panel.",
                RecordDate = DateTime.Now.AddDays(-28),
                RecordType = RecordType.Резултат,
                DoctorName = "Sarah Johnson",
                DoctorSpecialty = "General Medicine",
                Department = "Laboratory",
                CreatedDate = DateTime.Now.AddDays(-28),
                TestResults =
                    "All values within normal range. Cholesterol slightly elevated but not of immediate concern.",
                NormalRange = "HDL: 40-60 mg/dL, LDL: <100 mg/dL, Total Cholesterol: <200 mg/dL",
                Notes = "Follow-up in 6 months recommended for cholesterol monitoring.",
                PatientId = patientId,
            },
            new MedicalRecordViewModel
            {
                Id = 3,
                Title = "Hypertension Diagnosis",
                Description = "Initial diagnosis of stage 1 hypertension.",
                RecordDate = DateTime.Now.AddDays(-14),
                RecordType = RecordType.Диагноза,
                DoctorName = "Michael Chen",
                DoctorSpecialty = "Cardiology",
                Department = "Cardiovascular Health",
                CreatedDate = DateTime.Now.AddDays(-14),
                DiagnosisCode = "I10",
                TreatmentPlan =
                    "Lifestyle modifications including reduced sodium intake, increased physical activity. Recheck blood pressure in 4 weeks.",
                Notes = "Patient advised on home blood pressure monitoring techniques.",
                PatientId = patientId,
            },
            new MedicalRecordViewModel
            {
                Id = 4,
                Title = "Medication: Lisinopril",
                Description = "Prescription for hypertension.",
                RecordDate = DateTime.Now.AddDays(-14),
                RecordType = RecordType.Лекарства,
                DoctorName = "Michael Chen",
                DoctorSpecialty = "Cardiology",
                Department = "Cardiovascular Health",
                CreatedDate = DateTime.Now.AddDays(-14),
                MedicationName = "Lisinopril",
                Dosage = "10mg daily",
                Duration = "30 days with 2 refills",
                Instructions = "Take one tablet by mouth daily with or without food.",
                Notes = "Monitor for side effects such as dizziness or cough.",
                PatientId = patientId,
            },
        };
    }

    private List<DoctorViewModel> GetSampleDoctors()
    {
        return new List<DoctorViewModel>
        {
            new DoctorViewModel
                { Id = Guid.Empty, Name = "Sarah Johnson", Specialisation = Specialisation.Dermatologist },
            new DoctorViewModel
                { Id = Guid.Empty, Name = "Michael Chen", Specialisation = Specialisation.Cardiologist },
            new DoctorViewModel
                { Id = Guid.Empty, Name = "Emily Rodriguez", Specialisation = Specialisation.Gastroenterologist },
        };
    }
}