using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using HealthRec.Common;
using HealthRec.Data;
using HealthRec.Data.Entities;
using HealthRec.Presentation.Extensions;
using HealthRec.Presentation.Models;
using HealthRec.Services.Common.Contracts;
using HealthRec.Services.Doctor.Contract;
using HealthRec.Services.Doctor.Model;
using HealthRec.Services.Doctor.Models;
using HealthRec.Services.Identity.Constants;
using HealthRec.Services.Patient.Contract;
using HealthRec.Services.Patient.Models;
using Microsoft.AspNetCore.Authentication;
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
    private readonly SignInManager<ApplicationUser> signInManager;

    public PatientController(
        IDoctorService doctorService,
        IPatientService patientService,
        UserManager<ApplicationUser> userManager,
        HealthRecDbContext dbContext,
        HealthRecDbContext context,
        IEmailService emailService,
        ILogger<PatientController> logger,
        SignInManager<ApplicationUser> signInManager)
    {
        this.doctorService = doctorService;
        this.patientService = patientService;
        this.userManager = userManager;
        this.dbContext = dbContext;
        this.context = context;
        this.emailService = emailService;
        this.logger = logger;
        this.signInManager = signInManager;
    }

    [HttpGet("index")]
    [Authorize(DefaultPolicies.DoctorPolicy)]
    public async Task<IActionResult> Index()
    {
        return this.RedirectToAction("MyPatients", "Doctor");
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

    [HttpPost("delete/{id}")]
    [Authorize(DefaultPolicies.DoctorPolicy)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            var patient = await this.patientService.GetByIdAsync(id);
            if (patient == null)
            {
                return this.NotFound();
            }

            var result = await this.patientService.DeletePatientsAsync(id);

            if (result.Succeeded)
            {
                this.TempData["SuccessMessage"] = $"Patient {patient.FirstName} {patient.LastName} was successfully deleted.";
            }

            // Redirect to the correct view name or action
            // Option 1: If you have another action that shows the patient list
            return this.RedirectToAction("MyPatients", "Doctor"); // Change this to the correct action name

            // Option 2: If you want to go back to the doctor's details page that shows patients
            // return this.RedirectToAction("Details", "Doctor", new { id = doctorId });
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "Error occurred while deleting patient with ID: {PatientId}", id);
            this.TempData["ErrorMessage"] = "An error occurred while deleting the patient.";

            // Also redirect to the correct view here
            return this.RedirectToAction("Index"); // Change this to the correct action name
        }
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

    // For authenticated doctors to view a specific patient's records

    // Display patient records after security code verification

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
                Specialisation = (Specialisation?)doctor.Specialisation,
            },
        };
    }

    // Sample data generation methods - in a real app these would be database queries

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