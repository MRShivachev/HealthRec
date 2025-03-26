using System.Security.Claims;
using HealthRec.Data;
using HealthRec.Data.Entities;
using HealthRec.Presentation.Models;
using HealthRec.Services.Doctor.Contract;
using HealthRec.Services.Doctor.Model;
using HealthRec.Services.Identity.Constants;
using HealthRec.Services.Patient.Contract;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HealthRec.Presentation.Controllers;

[Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
[Route("doctors")]
public class DoctorController : Controller
{
    private readonly IDoctorService doctorService;
    private readonly IPatientService patientService;
    private readonly HealthRecDbContext dbContext;
    private readonly UserManager<ApplicationUser> userManager;
    private readonly ILogger<DoctorController> logger;

    public DoctorController(
        IDoctorService doctorService,
        IPatientService patientService,
        HealthRecDbContext dbContext,
        UserManager<ApplicationUser> userManager,
        ILogger<DoctorController> logger)
    {
        this.doctorService = doctorService;
        this.patientService = patientService;
        this.dbContext = dbContext;
        this.userManager = userManager;
        this.logger = logger;
    }

    [HttpGet("index")]
    [Authorize(Roles = DefaultRoles.Doctor + "," + DefaultRoles.Admin)]
    public async Task<IActionResult> Index()
    {
        var doctors = await this.doctorService.GetAllAsync();

        var viewModel = new DoctorIndexViewModel
        {
            Doctors = doctors.Select(d => new DoctorListItemViewModel
            {
                Id = d.Id,
                Name = $"{d.FirstName} {d.LastName}",
                Specialisation = d.Specialisation.ToString(),
            }).ToList(),
        };

        return this.View(viewModel);
    }

    [HttpGet("details/{id}")]
    [Authorize(Roles = DefaultRoles.Doctor + "," + DefaultRoles.Admin)]
    public async Task<IActionResult> Details(Guid id)
    {
        // Check if current user is the doctor being viewed or an admin
        var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
        var isAdmin = this.User.IsInRole(DefaultRoles.Admin);

        if (!isAdmin && userId != id.ToString())
        {
            // If not the same doctor and not admin, redirect to their own profile
            return this.RedirectToAction(nameof(this.Details), new { id = userId });
        }

        var doctor = await this.doctorService.GetByIdAsync(id);
        if (doctor == null)
        {
            return this.NotFound();
        }

        // Get all patients linked to this doctor - Updated to use plural table name
        var patientDoctorRelations = await this.dbContext.DoctorPatients
            .Where(dp => dp.DoctorId == id)
            .Select(dp => dp.PatientId)
            .ToListAsync();

        var patients = await this.dbContext.Patients
            .Where(p => patientDoctorRelations.Contains(p.Id))
            .Select(p => new PatientViewModel
            {
                Id = p.Id,
                Name = $"{p.FirstName} {p.LastName}",
                Email = p.Email,
                PhoneNumber = p.PhoneNumber,
                SecurityCode = p.Code,
            })
            .ToListAsync();

        var viewModel = new DoctorDetailsViewModel
        {
            Doctor = doctor,
            Patients = patients,
        };

        // Pass success message from TempData if it exists
        if (this.TempData.ContainsKey("SuccessMessage"))
        {
            this.ViewBag.SuccessMessage = this.TempData["SuccessMessage"];
        }

        return this.View(viewModel);
    }

    [HttpGet("create")]
    [Authorize(DefaultPolicies.AdminPolicy)]
    public IActionResult Create()
    {
        return this.View(new DoctorCreateViewModel());
    }

    [HttpPost("create")]
    [ValidateAntiForgeryToken]
    [Authorize(DefaultPolicies.AdminPolicy)]
    public async Task<IActionResult> Create(DoctorCreateViewModel model)
    {
        try
        {
            var doctorModel = new DoctorModel
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Specialisation = model.Specialisation,
                Email = model.Email,
                Phone = model.PhoneNumber,
                Password = model.Password,
            };

            var result = await this.doctorService.CreateDoctorAsync(doctorModel);
            if (result!.Succeeded)
            {
                return this.RedirectToAction("Index");
            }

            this.ModelState.AddModelError(string.Empty, result.Message);
        }
        catch (Exception e)
        {
            this.logger.LogError(e, e.Message);
            return this.View(model);
        }

        return this.View(model);
    }

    [HttpGet("my-patients")]
    [Authorize(DefaultPolicies.DoctorPolicy)]
    public async Task<IActionResult> MyPatients()
    {
        var doctorId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!Guid.TryParse(doctorId, out var parsedDoctorId))
        {
            return this.BadRequest("Invalid doctor ID.");
        }

        // Fetch patients associated with the doctor through the DoctorPatient table
        var patients = await this.dbContext.Patients
            .Where(p => p.Doctors != null && p.Doctors.Any(dp => dp.DoctorId == parsedDoctorId))
            .ToListAsync();

        // Map the patients to PatientViewModel
        var patientViewModels = patients.Select(p => new PatientViewModel
        {
            Id = p.Id,
            Name = p.FirstName + " " + p.LastName,
            Email = p.Email,
            SecurityCode = p.Code,
            DateOfBirth = p.DateOfBirth,
            PhoneNumber = p.PhoneNumber,
        }).ToList();

        var viewModel = new PatientTableViewModel
        {
            Patients = patientViewModels,
        };

        return this.View("PatientTable", viewModel);
    }
}