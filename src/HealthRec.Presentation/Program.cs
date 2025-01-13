using HealthRec.Data;
using HealthRec.Services;
using HealthRec.Services.Identity.Constants;
using Microsoft.AspNetCore.Authentication.Cookies;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services
            .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(
                CookieAuthenticationDefaults.AuthenticationScheme,
                options =>
                {
                    options.LoginPath = "/login";
                    options.LogoutPath = "/logout";
                    options.AccessDeniedPath = "/access-denied";
                });

        builder.Services.AddAuthorization(options =>
        {
            options.AddPolicy(DefaultPolicies.AdminPolicy, policyBuilder =>
            {
                policyBuilder.RequireAuthenticatedUser();
                policyBuilder.AddAuthenticationSchemes(CookieAuthenticationDefaults.AuthenticationScheme);
                policyBuilder.RequireRole(DefaultRoles.Admin);
            });

            options.AddPolicy(DefaultPolicies.UserPolicy, policyBuilder =>
            {
                policyBuilder.RequireAuthenticatedUser();
                policyBuilder.AddAuthenticationSchemes(CookieAuthenticationDefaults.AuthenticationScheme);
                policyBuilder.RequireRole(DefaultRoles.User);
            });
        });

        builder.Services.AddHttpContextAccessor();
        builder.Services.AddData(builder.Configuration);
        builder.Services.AddServices(builder.Configuration);

        builder.Services.AddMvc();

        var app = builder.Build();

        await app.PrepareAsync();

        if (app.Environment.IsDevelopment())
        {
            // do nothing
        }
        else
        {
            app.UseExceptionHandler("/Home/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
            app.UseHttpsRedirection();
        }

        app.UseStaticFiles();
        app.UseAuthentication();
        app.UseRouting();
        app.UseAuthorization();
        app.MapControllers();

        app.Run();
    }
}