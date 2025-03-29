using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using HealthRec.Data;
using HealthRec.Data.Entities;
using HealthRec.Presentation.Models;
using HealthRec.Services.Common.Contracts;
using HealthRec.Services.Doctor.Contract;
using HealthRec.Services.Doctor.Model;
using HealthRec.Services.Identity.Constants;
using HealthRec.Services.Patient.Contract;
using HealthRec.Services.Patient.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HealthRec.Presentation.Controllers;

[Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
[Route("patients")]
public class PatientController : Controller
{
    private readonly IDoctorService doctorService;
    private readonly IPatientService patientService;
    private readonly UserManager<ApplicationUser> userManager;
    private readonly HealthRecDbContext dbContext;
    private readonly IEmailService emailService;
    private readonly ILogger<PatientController> logger;
    private readonly HealthRecDbContext context;

    public PatientController(
        IDoctorService doctorService,
        IPatientService patientService,
        UserManager<ApplicationUser> userManager,
        HealthRecDbContext dbContext,
        HealthRecDbContext context,
        IEmailService emailService,
        ILogger<PatientController> logger)
    {
        this.doctorService = doctorService;
        this.patientService = patientService;
        this.userManager = userManager;
        this.dbContext = dbContext;
        this.context = context;
        this.emailService = emailService;
        this.logger = logger;
    }

    [HttpGet("index")]
    [Authorize(DefaultPolicies.DoctorPolicy)]
    public async Task<IActionResult> Index()
    {
        var patients = await this.patientService.GetAllAsync();
        return this.View(patients);
    }

    [HttpGet("create")]
    [Authorize(DefaultPolicies.DoctorPolicy)]
    public async Task<IActionResult> Create(Guid id)
    {
        var doctors = await this.doctorService.GetAllAsync();

        var viewModel = new PatientCreateViewModel
        {
            AvailableDoctors = doctors.Select(d => new DoctorDropdownViewModel
            {
                Id = d.Id,
                Name = $"{d.FirstName} {d.LastName}",
                Specialty = d.Specialisation.ToString(),
            }).ToList(),

            AssignedDoctorId = id,
        };

        return this.View(viewModel);
    }

    [HttpPost("create")]
    [Authorize(DefaultPolicies.DoctorPolicy)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(PatientCreateViewModel viewModel)
    {
        if (this.ModelState.IsValid)
        {
            // Get the current user's ID from claims
            var userId = Guid.Parse(this.User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            // Now we know this is a valid doctor ID
            var doctorId = viewModel.AssignedDoctorId;

            // Map view model to service model
            var patientModel = new PatientModel
            {
                FirstName = viewModel.FirstName,
                LastName = viewModel.LastName,
                Email = viewModel.Email,
                Phone = viewModel.PhoneNumber,
            };

            // Use service layer to create patient with the verified doctor ID
            var result = await this.patientService.CreatePatientWithDoctorAsync(
                patientModel,
                viewModel.DateOfBirth,
                doctorId);

            if (result.Succeeded)
            {
                this.TempData["SuccessMessage"] = "Patient created successfully.";
                return this.RedirectToAction("Details", "Doctor", new { id = doctorId });
            }
            else
            {
                this.ModelState.AddModelError(string.Empty, result.Message);
            }
        }

        // If we got this far, something failed. Reload the form with doctors list
        var allDoctors = await this.doctorService.GetAllAsync();
        viewModel.AvailableDoctors = allDoctors.Select(d => new DoctorDropdownViewModel
        {
            Id = d.Id,
            Name = $"{d.FirstName} {d.LastName}",
            Specialty = d.Specialisation.ToString(),
        }).ToList();

        return this.View(viewModel);
    }

    [HttpGet("details/{id}")]
    [Authorize(DefaultPolicies.DoctorPolicy)]
    public async Task<IActionResult> Details(Guid id)
    {
        var patient = await this.patientService.GetByIdAsync(id);
        if (patient == null)
        {
            return this.NotFound();
        }

        return this.View(patient);
    }
}