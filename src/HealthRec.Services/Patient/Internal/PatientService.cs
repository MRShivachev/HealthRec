using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Essentials.Results;
using HealthRec.Data;
using HealthRec.Data.Entities;
using HealthRec.Services.Common;
using HealthRec.Services.Common.Contracts;
using HealthRec.Services.Common.Internals;
using HealthRec.Services.Identity.Constants;
using HealthRec.Services.Patient.Contract;
using HealthRec.Services.Patient.Extensions;
using HealthRec.Services.Patient.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HealthRec.Services.Patient.Internal;

internal class PatientService : IPatientService
{
    private readonly HealthRecDbContext context;
    private readonly UserManager<ApplicationUser> userManager;
    private readonly IEmailService emailService;
    private readonly ISecurityCodeGenerator securityCodeGenerator;
    private readonly ILogger<PatientService> logger;

    public PatientService(
        HealthRecDbContext context,
        UserManager<ApplicationUser> userManager,
        IEmailService emailService,
        ISecurityCodeGenerator securityCodeGenerator,
        ILogger<PatientService> logger)
    {
        this.context = context;
        this.userManager = userManager;
        this.emailService = emailService;
        this.securityCodeGenerator = securityCodeGenerator;
        this.logger = logger;
    }

    public async Task<IEnumerable<PatientModel>> GetAllAsync() =>
        await this.context
            .Patients.Select(x => x.ToModel())
            .AsNoTracking()
            .ToListAsync();

    public async Task<PatientModel?> GetByIdAsync(Guid id) =>
        await this.context.Patients
            .Where(x => x.Id == id)
            .Select(x => x.ToModel())
            .FirstOrDefaultAsync();

    public async Task<MutationResult> CreatePatientsAsync(PatientModel patient)
    {
        try
        {
            var patientEntity = new Data.Entities.Patient
            {
                Id = patient.Id,
                FirstName = patient.FirstName,
                LastName = patient.LastName,
                Email = patient.Email,
                Code = patient.Code,
                PhoneNumber = patient.Phone,
            };

            this.context.Patients.Add(patientEntity);
            await this.context.SaveChangesAsync();

            return MutationResult.ResultFrom(patientEntity.Id);
        }
        catch (Exception e)
        {
            this.logger.LogError(e, e.Message);
            return MutationResult.ResultFrom(null, "Error creating patient");
        }
    }

    public async Task<MutationResult> UpdatePatientsAsync(Guid id, PatientModel patient)
    {
        try
        {
            var patientEntity = await this.context.Patients.FindAsync(id);
            if (patientEntity == null)
            {
                return MutationResult.ResultFrom("Patient not found");
            }

            patientEntity.FirstName = patient.FirstName;
            patientEntity.LastName = patient.LastName;
            patientEntity.Email = patient.Email;
            patientEntity.Code = patient.Code;
            patientEntity.PhoneNumber = patient.Phone;

            this.context.Patients.Update(patientEntity);
            await this.context.SaveChangesAsync();

            return MutationResult.ResultFrom(patientEntity.Id);
        }
        catch (Exception e)
        {
            this.logger.LogError(e, e.Message);
            return MutationResult.ResultFrom(null, "Error updating patient");
        }
    }

    public async Task<MutationResult> DeletePatientsAsync(Guid id)
    {
        try
        {
            var patientEntity = await this.context.Patients.FindAsync(id);
            if (patientEntity == null)
            {
                return MutationResult.ResultFrom("Patient not found");
            }

            this.context.Patients.Remove(patientEntity);
            await this.context.SaveChangesAsync();

            return MutationResult.ResultFrom(patientEntity.Id);
        }
        catch (Exception e)
        {
            this.logger.LogError(e, e.Message);
            return MutationResult.ResultFrom(null, "Error deleting patient");
        }
    }

    public async Task<List<PatientModel>> GetPatientsByDoctorIdAsync(Guid doctorId)
    {
        // Get IDs of patients assigned to this doctor
        var patientIds = await this.context.DoctorPatients
            .Where(dp => dp.DoctorId == doctorId)
            .Select(dp => dp.PatientId)
            .ToListAsync();

        // Get patient details
        var patients = await this.context.Patients
            .Where(p => patientIds.Contains(p.Id))
            .Select(p => p.ToModel())
            .ToListAsync();

        return patients;
    }

    public async Task<MutationResult> CreatePatientWithDoctorAsync(
        PatientModel patient,
        string password,
        DateTime dateOfBirth,
        Guid assignedDoctorId,
        Guid currentDoctorId)
    {
        // Check if email already exists
        var existingUser = await this.userManager.FindByEmailAsync(patient.Email!);
        if (existingUser != null)
        {
            return MutationResult.ResultFrom("Email is already registered.");
        }

        // Generate a unique 8-digit security code using the existing service
        var securityCode = this.securityCodeGenerator.GenerateUniqueSecurityCode();

        // Set the security code in the patient model
        patient.Code = securityCode;

        // Create ApplicationUser
        /*var user = new ApplicationUser
        {
            UserName = patient.Email,
            Email = patient.Email,
            FirstName = patient.FirstName,
            LastName = patient.LastName,
            PhoneNumber = patient.Phone,
        };*/

        var patientEntity = new Data.Entities.Patient
        {
            Id = Guid.NewGuid(),
            FirstName = patient.FirstName,
            LastName = patient.LastName,
            Email = patient.Email,
            Code = patient.Code,
            PhoneNumber = patient.Phone,
            UserName = patient.Email,
        };

        /*
        // Create user with password
        var result = await this.userManager.CreateAsync(user, password);
        */
        var result = await this.userManager.CreateAsync(patientEntity);
        await this.userManager.AddToRoleAsync(patientEntity, "Doctor");

        if (!result.Succeeded)
        {
            string errorMessage = string.Join(", ", result.Errors.Select(e => e.Description));
            return MutationResult.ResultFrom(errorMessage);
        }

        // Add to Patient role
        await this.userManager.AddToRoleAsync(patientEntity, DefaultRoles.Patient);

        // Add DateOfBirth claim
        await this.userManager.AddClaimAsync(
            patientEntity,
            new System.Security.Claims.Claim("DateOfBirth", dateOfBirth.ToString("yyyy-MM-dd")));

        // Create link to doctor
        var doctorPatient = new DoctorPatient
        {
            DoctorId = assignedDoctorId,
            PatientId = patientEntity.Id,
        };

        // Save changes
        await this.context.DoctorPatients.AddAsync(doctorPatient);
        await this.context.SaveChangesAsync();

        // Send welcome email
        await this.emailService.SendEmailAsync(new Services.Common.Models.EmailModel
        {
            To = patientEntity.Email,
            Subject = "Welcome to HealthRec",
            Body = $"Dear {patientEntity.FirstName},<br/><br/>Welcome to HealthRec! Your patient account has been successfully created.<br/><br/>Your security code is: <strong>{securityCode}</strong><br/><br/>Keep this code secure - you can use it to access your medical records.",
            Email = patient.Email!,
            Message = "Your patient has been successfully created.",
        });

        this.logger.LogInformation(
            "Patient {Email} created with security code {Code} and assigned to doctor {DoctorId}",
            patient.Email,
            securityCode,
            doctorPatient.DoctorId);

        return MutationResult.ResultFrom(patient, securityCode);
    }

    public async Task<string> GenerateUniqueSecurityCodeAsync()
    {
        // Use the existing security code generator service
        return this.securityCodeGenerator.GenerateUniqueSecurityCode();
    }
}