using Essentials.Results;
using HealthRec.Data;
using HealthRec.Services.Doctor.Contract;
using HealthRec.Services.Doctor.Model;
using HealthRec.Services.Patient.Contract;
using HealthRec.Services.Patient.Extensions;
using HealthRec.Services.Patient.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HealthRec.Services.Patient.Internal;

internal class PatientService : IPatientService
{
    private readonly HealthRecDbContext context;
    private readonly ILogger<PatientService> logger;

    public PatientService(HealthRecDbContext context, ILogger<PatientService> logger)
    {
        this.context = context;
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
            logger.LogError(e, e.Message);
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
            logger.LogError(e, e.Message);
            return MutationResult.ResultFrom(null, "Error deleting patient");
        }
    }
}