using HealthRec.Data;
using HealthRec.Data.Entities;
using HealthRec.Presentation.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HealthRec.Presentation.Controllers;

[Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
public class DoctorController : Controller
{
    private readonly ILogger<DoctorController> _logger;
    private readonly HealthRecDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public DoctorController(
        ILogger<DoctorController> logger,
        HealthRecDbContext context,
        UserManager<ApplicationUser> userManager)
    {
        _logger = logger;
        _context = context;
        _userManager = userManager;
    }

    // GET: /Doctor/Index
    [HttpGet]
    [Route("doctors")]
    public async Task<IActionResult> Index()
    {

        return this.View();
    }

    // GET: /Doctor/Details/5
    [HttpGet]
    [Route("doctor/{id}")]
    public async Task<IActionResult> Details(int id)
    {
        // Since we're using the hash code as ID in the view, we need to find the doctor by iterating
        var allDoctors = await _context.Doctors.ToListAsync();
        var doctor = allDoctors.FirstOrDefault(d => Math.Abs(d.Id.GetHashCode()) == id);

        if (doctor == null)
        {
            return NotFound();
        }

        // Get patients for this doctor (this is a simplified approach - in a real app,
        // you would likely have a Patient-Doctor relationship table)
        // For demonstration, we're retrieving all patients
        var patients = await _context.Patients
            .ToListAsync();

        var viewModel = new DoctorDetailsViewModel
        {
            Doctor = new DoctorViewModel
            {
                Id = Math.Abs(doctor.Id.GetHashCode()),
                Name = $"{doctor.FirstName} {doctor.LastName}",
                Specialty = doctor.Specialisation.ToString()
            },
            Patients = patients.Select(p => new PatientViewModel
            {
                Id = Math.Abs(p.Id.GetHashCode()),
                Name = $"{p.FirstName} {p.LastName}",
                Email = p.Email,
                SecurityCode = p.Code
            }).ToList()
        };

        return View(viewModel);
    }
}