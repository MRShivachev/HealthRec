using System.Text.Encodings.Web;
using Azure.Core;
using HealthRec.Common;
using Essentials.Extensions;
using HealthRec.Data.Entities;
using HealthRec.Presentation.Extensions;
using HealthRec.Presentation.Models;
using HealthRec.Services.Authentication.Models;
using HealthRec.Services.Common.Contracts;
using HealthRec.Services.Common.Models;
using HealthRec.Services.Identity.Constants;
using HealthRec.Services.Identity.Extensions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

[Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
public class AuthenticationController : Controller
{
    private readonly UserManager<ApplicationUser> userManager;
    private readonly SignInManager<ApplicationUser> signInManager;
    private readonly IEmailService emailService;
    private readonly UrlEncoder urlEncoder;
    private readonly ILogger<AuthenticationController> logger;

    public AuthenticationController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IEmailService emailService,
        UrlEncoder urlEncoder,
        ILogger<AuthenticationController> logger)
    {
        this.userManager = userManager;
        this.signInManager = signInManager;
        this.emailService = emailService;
        this.urlEncoder = urlEncoder;
        this.logger = logger;
    }

    [HttpGet("/login")]
    [AllowAnonymous]
    public IActionResult Login()
    {
        if (this.IsUserAuthenticated())
        {
            return this.RedirectToDefault();
        }

        var model = new PatientLoginViewModel();
        return this.View(model);
    }

    [HttpPost("/login")]
    [ValidateAntiForgeryToken]
    [AllowAnonymous]
    public async Task<IActionResult> Login(PatientLoginViewModel model)
    {
        if (this.IsUserAuthenticated())
        {
            return this.RedirectToDefault();
        }

        if (this.ModelState.IsValid)
        {
            /*var user = await this.userManager.FindByEmailAsync(model.Email!);
            if (user == null || !(await this.userManager.CheckPasswordAsync(user, model.Password!)))
            {
                this.ModelState.AddModelError(string.Empty, T.InvalidLoginErrorMessage);
                return this.View(model);
            }

            if (await this.userManager.IsLockedOutAsync(user))
            {
                this.ModelState.AddModelError(string.Empty, T.UserLockedOutErrorMessage);
                return this.View(model);
            }

            await this.SignInAsync(user, model.RememberMe);

            return this.RedirectToDefault();*/
        }

        return this.View(model);
    }

    [HttpGet("/doctor-login")]
    [AllowAnonymous]
    public IActionResult DoctorLogin()
    {
        if (this.IsUserAuthenticated())
        {
            return this.RedirectToDefault();
        }

        var model = new DoctorLoginViewModel();
        return this.View(model);
    }

    [HttpPost("/doctor-login")]
    [ValidateAntiForgeryToken]
    [AllowAnonymous]
    public async Task<IActionResult> DoctorLogin(DoctorLoginViewModel model)
    {
        if (this.IsUserAuthenticated())
        {
            return this.RedirectToDefault();
        }

        if (this.ModelState.IsValid)
        {
            var user = await this.userManager.FindByEmailAsync(model.Email!);
            if (user == null || !(await this.userManager.CheckPasswordAsync(user, model.Password!)))
            {
                this.ModelState.AddModelError(string.Empty, T.InvalidLoginErrorMessage);
                return this.View(model);
            }

            if (await this.userManager.IsLockedOutAsync(user))
            {
                this.ModelState.AddModelError(string.Empty, T.UserLockedOutErrorMessage);
                return this.View(model);
            }

            await this.SignInAsync(user, model.RememberMe);

            return this.RedirectToAction("Index", "Doctor");
        }

        // If we got this far, something failed, redisplay form
        return this.View(model);
    }

    [HttpGet("/patient-login")]
    [AllowAnonymous]
    public IActionResult PatientLogin()
    {
        if (this.IsUserAuthenticated())
        {
            return this.RedirectToDefault();
        }

        var model = new PatientLoginViewModel();
        return this.View(model);
    }

    [HttpPost("/patient-login")]
    [ValidateAntiForgeryToken]
    [AllowAnonymous]
    public async Task<IActionResult> PatientLogin(PatientLoginViewModel model)
    {
        if (this.IsUserAuthenticated())
        {
            return this.RedirectToDefault();
        }

        if (this.ModelState.IsValid)
        {
            var user = await this.userManager.FindByEmailAsync(model.Email!);
            if (user == null || !(await this.userManager.CheckPasswordAsync(user, model.Password!)))
            {
                this.ModelState.AddModelError(string.Empty, T.InvalidLoginErrorMessage);
                return this.View(model);
            }

            if (await this.userManager.IsLockedOutAsync(user))
            {
                this.ModelState.AddModelError(string.Empty, T.UserLockedOutErrorMessage);
                return this.View(model);
            }

            // Verify the user is a patient
            bool isPatient = await this.userManager.IsInRoleAsync(user, "Patient");


            await this.SignInAsync(user, model.RememberMe);
            this.logger.LogInformation("Patient {Email} logged in at {Time}.", model.Email, DateTime.UtcNow);

            return this.RedirectToDefault();
        }

        return this.View(model);
    }

    [HttpGet("/register-patient")]
    [AllowAnonymous]
    public IActionResult RegisterPatient()
    {
        if (this.IsUserAuthenticated())
        {
            return this.RedirectToDefault();
        }

        var model = new RegisterPatientViewModel();
        return this.View(model);
    }

    [HttpPost("/register-patient")]
    [ValidateAntiForgeryToken]
    [AllowAnonymous]
    public async Task<IActionResult> RegisterPatient(RegisterPatientViewModel model)
    {
        if (this.IsUserAuthenticated())
        {
            return this.RedirectToDefault();
        }

        if (this.ModelState.IsValid)
        {
            // Check if user already exists
            var existingUser = await this.userManager.FindByEmailAsync(model.Email);
            if (existingUser != null)
            {
                this.ModelState.AddModelError(string.Empty, "Email is already registered.");
                return this.View(model);
            }

            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
            };

            var result = await this.userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                await this.userManager.AddToRoleAsync(user, "Patient");

                if (model.DateOfBirth != default)
                {
                    await this.userManager.AddClaimAsync(user,
                        new System.Security.Claims.Claim("DateOfBirth", model.DateOfBirth.ToString("yyyy-MM-dd")));
                }

                await this.SignInAsync(user, false);

                await this.emailService.SendEmailAsync(new EmailModel
                {
                    To = user.Email,
                    Subject = "Welcome to HealthRec",
                    Body =
                        $"Dear {user.FirstName},<br/><br/>Welcome to HealthRec! Your patient account has been successfully created.",
                    Email = null,
                    Message = null
                });

                this.logger.LogInformation("New patient account created for {Email}", model.Email);

                return this.RedirectToDefault();
            }

            this.ModelState.AssignIdentityErrors(result.Errors);
        }

        return this.View(model);
    }


    [HttpGet("/forgot-password")]
    [AllowAnonymous]
    public IActionResult ForgotPassword()
    {
        if (this.IsUserAuthenticated())
        {
            return this.RedirectToDefault();
        }

        var model = new ForgotPasswordViewModel();
        return this.View(model);
    }

    [HttpPost("/forgot-password")]
    [ValidateAntiForgeryToken]
    [AllowAnonymous]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
    {
        if (this.IsUserAuthenticated())
        {
            return this.RedirectToDefault();
        }

        if (this.ModelState.IsValid)
        {
            var user = await this.userManager.FindByEmailAsync(model.Email!);
            if (user != null)
            {
                var resetPasswordToken = await this.userManager.GeneratePasswordResetTokenAsync(user);
                var resetPasswordEncodedToken = UrlEncoder.Default.Encode(resetPasswordToken);
                var resetPasswordUrl = this.HttpContext
                    .GetAbsoluteRoute($"/reset-password?email={user.Email}&token={resetPasswordEncodedToken}");

                var result = await this.emailService.SendResetPasswordEmailAsync(
                    user.Email!,
                    resetPasswordUrl);

                this.logger.LogInformation("Reset password email send result {Result}", result);
            }

            this.TempData["MessageText"] = T.ForgotPasswordSuccessMessage;
            this.TempData["MessageVariant"] = "success";

            return this.RedirectToAction(nameof(this.Login));
        }

        return this.View(model);
    }

    [AllowAnonymous]
    [HttpGet("/reset-password")]
    public async Task<IActionResult> ResetPassword(string email, string token)
    {
        if (this.IsUserAuthenticated())
        {
            return this.RedirectToDefault();
        }

        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(token))
        {
            return this.NotFound();
        }

        var user = await this.userManager.FindByEmailAsync(email);
        if (user == null)
        {
            return this.NotFound();
        }

        var model = new ResetPasswordViewModel
        {
            Token = token,
            Email = email,
        };

        return this.View(model);
    }

    [HttpPost("/reset-password")]
    [AllowAnonymous]
    public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
    {
        if (this.IsUserAuthenticated())
        {
            return this.RedirectToDefault();
        }

        if (this.ModelState.IsValid)
        {
            var user = await this.userManager.FindByEmailAsync(model.Email!);
            if (user == null)
            {
                return this.NotFound();
            }

            var result = await this.userManager.ResetPasswordAsync(user, model.Token!, model.Password!);
            if (result.Succeeded)
            {
                return this.RedirectToAction(nameof(this.Login));
            }

            this.ModelState.AssignIdentityErrors(result.Errors);
        }

        return this.View(model);
    }

    [HttpGet("/access-denied")]
    public IActionResult AccessDenied()
    {
        return this.View();
    }

    [HttpGet("/patient-register")]
    [AllowAnonymous]
    public IActionResult PatientRegister()
    {
        if (this.IsUserAuthenticated())
        {
            return this.RedirectToDefault();
        }

        var model = new RegisterPatientViewModel();
        return this.View(model);
    }

    [HttpPost("/patient-register")]
    [ValidateAntiForgeryToken]
    [AllowAnonymous]
    public async Task<IActionResult> PatientRegister(RegisterPatientViewModel model)
    {
        if (this.IsUserAuthenticated())
        {
            return this.RedirectToDefault();
        }

        if (this.ModelState.IsValid)
        {
            var result = await this.userManager.CreateAsync(
                new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                },
                model.Password!);

            if (result.Succeeded)
            {
                this.TempData["MessageText"] = T.RegisterSuccessMessage;
                this.TempData["MessageVariant"] = "success";
                return this.RedirectToAction(nameof(this.Login));
            }

            this.ModelState.AssignIdentityErrors(result.Errors);
        }

        return this.View(model);
    }

    [HttpPost("/logout")]
    public async Task<IActionResult> Logout()
    {
        await this.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return this.RedirectToAction(nameof(this.Login));
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