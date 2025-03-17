using System.Diagnostics;
using System.Security.Claims;
using System.Threading.Tasks;
using HealthRec.Presentation.Models;
using HealthRec.Services.Common.Contracts;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace HealthRec.Presentation.Controllers;

[AllowAnonymous]
[Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
public class HomeController : Controller
{
    private readonly IEmailService emailService;
    private readonly ILogger<HomeController> logger;

    public HomeController(
        IEmailService emailService,
        ILogger<HomeController> logger)
    {
        this.emailService = emailService;
        this.logger = logger;
    }

    [HttpGet("/")]

    public async Task<IActionResult> Index()
    {
        var viewModel = new IndexViewModel
        {
            Title = "Index Title",
            Name = "PEsho",
        };

        this.ViewBag.Message = "Welcome to HealthRec!!";

        return this.View(viewModel);
    }

    [HttpGet("/contact-us")]
    public IActionResult Contacts()
    {
        return this.View();
    }
    
    [HttpGet("/About-us")]
    public IActionResult AboutUs()
    {
        return this.View();
    }
    
   
    
    

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return this.View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? this.HttpContext.TraceIdentifier });
    }
    
    
}