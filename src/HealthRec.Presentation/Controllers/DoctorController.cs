using HealthRec.Data;
using HealthRec.Data.Entities;
using HealthRec.Presentation.Models;
using HealthRec.Services.Doctor.Contract;
using HealthRec.Services.Doctor.Model;
using HealthRec.Services.Identity.Constants;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace HealthRec.Presentation.Controllers;

[Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
public class DoctorController : Controller
{
    private readonly ILogger<DoctorController> logger;
    private readonly IDoctorService doctorService;
    private readonly UserManager<ApplicationUser> userManager;

    public DoctorController(
        ILogger<DoctorController> logger,
        IDoctorService doctorService,
        UserManager<ApplicationUser> userManager)
    {
        this.logger = logger;
        this.doctorService = doctorService;
        this.userManager = userManager;
    }

    // GET: /Doctor/Index
    [HttpGet]
    [Route("doctors")]
    public async Task<IActionResult> Index(string q)
    {
        try
        {
            var doctorModels = await this.doctorService.GetAllAsync();
            var viewModel = new DoctorIndexViewModel
            {
                SearchQuery = q,
            };

            var doctorViewModels = new List<DoctorViewModel>();
            foreach (var doctor in doctorModels)
            {
                doctorViewModels.Add(new DoctorViewModel
                {
                    Id = doctor.Id,
                    Name = doctor.FirstName + " " + doctor.LastName,
                    Specialisation = doctor.Specialisation,
                    Email = doctor.Email,
                    Phone = doctor.Phone,
                });
            }

            viewModel.Doctors = doctorViewModels;

            return this.View(viewModel);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    [HttpGet("doctors/{id}")]
    public async Task<IActionResult> Details(Guid id)
    {
        var doctor = await this.doctorService.GetByIdAsync(id);
        if (doctor == null)
        {
            return this.NotFound();
        }

        var viewModel = new DoctorDetailsViewModel
        {
            Doctor = doctor,
        };

        return this.View(viewModel);
    }

    [HttpGet("doctors/create")]
    [Authorize(DefaultPolicies.AdminPolicy)]
    public IActionResult Create()
    {
       return this.View(new DoctorCreateViewModel());
    }

    [HttpPost("doctors/create")]
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
}