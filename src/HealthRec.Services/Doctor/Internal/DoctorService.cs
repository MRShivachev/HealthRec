using Essentials.Results;
using HealthRec.Data;
using HealthRec.Data.Entities;
using HealthRec.Services.Doctor.Contract;
using HealthRec.Services.Doctor.Extensions;
using HealthRec.Services.Doctor.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HealthRec.Services.Doctor.Internal;

internal class DoctorService : IDoctorService
{
    private readonly HealthRecDbContext context;
    private readonly ILogger<DoctorService> logger;

    public DoctorService(
        HealthRecDbContext context,
        ILogger<DoctorService> logger)
    {
        this.context = context;
        this.logger = logger;
    }

    public async Task<IEnumerable<DoctorModel>> GetAllAsync() =>
        await this.context
            .Doctors
            .Select(x => x.ToModel())
            .AsNoTracking()
            .ToListAsync();

    public async Task<DoctorModel?> GetByIdAsync(Guid id) =>
        await this.context.Doctors
            .Where(x => x.Id == id)
            .Select(x => x.ToModel())
            .FirstOrDefaultAsync();

    public async Task<MutationResult?> CreateDoctorAsync(DoctorModel doctor)
    {
        try
        {
            var doctorEntity = new Data.Entities.Doctor
            {
                Id = doctor.Id,
                FirstName = doctor.FirstName,
                LastName = doctor.LastName,
                Email = doctor.Email,
                Specialisation = (Specialisation)doctor.Specialisation,
            };

            this.context.Doctors.Add(doctorEntity);
            await this.context.SaveChangesAsync();

            return MutationResult.ResultFrom(doctorEntity.Id);
        }
        catch (Exception e)
        {
            this.logger.LogError(e, e.Message);
            return MutationResult.ResultFrom(null, "Error creating doctor");
        }
    }

    public async Task<MutationResult?> UpdateDoctorAsync(Guid id, DoctorModel doctor)
    {
        try
        {
            var doctorEntity = await this.context.Doctors.FindAsync(id);
            if (doctorEntity == null)
            {
                return MutationResult.ResultFrom(null, "Doctor not found");
            }

            doctorEntity.FirstName = doctor.FirstName;
            doctorEntity.LastName = doctor.LastName;
            doctorEntity.Email = doctor.Email;
            doctorEntity.Specialisation = (Specialisation)doctor.Specialisation;

            this.context.Doctors.Update(doctorEntity);
            await this.context.SaveChangesAsync();

            return MutationResult.ResultFrom(doctorEntity.Id);
        }
        catch (Exception e)
        {
            this.logger.LogError(e, e.Message);
            return MutationResult.ResultFrom(null, "Error updating doctor");
        }
    }

    public async Task<MutationResult?> DeleteDoctorAsync(Guid id)
    {
        try
        {
            var doctorEntity = await this.context.Doctors.FindAsync(id);
            if (doctorEntity == null)
            {
                return MutationResult.ResultFrom(null, "Doctor not found");
            }

            this.context.Doctors.Remove(doctorEntity);
            await this.context.SaveChangesAsync();

            return MutationResult.ResultFrom(doctorEntity.Id);
        }
        catch (Exception e)
        {
            this.logger.LogError(e, e.Message);
            return MutationResult.ResultFrom(null, "Error deleting doctor");
        }
    }
}