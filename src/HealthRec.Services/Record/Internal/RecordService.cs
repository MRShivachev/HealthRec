using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Essentials.Results;
using HealthRec.Data;
using HealthRec.Data.Entities;
using HealthRec.Services.Common;
using HealthRec.Services.Common.Contracts;
using HealthRec.Services.Record.Contract;
using HealthRec.Services.Record.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HealthRec.Services.Record.Internal;

internal class RecordService : IRecordService
{
    private readonly HealthRecDbContext context;
    private readonly ILogger<RecordService> logger;

    public RecordService(HealthRecDbContext context, ILogger<RecordService> logger)
    {
        this.context = context;
        this.logger = logger;
    }

    public async Task<IEnumerable<RecordModel>> GetAllAsync() =>
        await this.context.Records
            .Select(r => new RecordModel
            {
                Id = r.Id,
                Description = r.Description,
                Result = r.Result,
                ReferenceRange = r.ReferenceRange,
                MedicationDetails = r.MedicationDetails,
                Type = r.Type,
            })
            .AsNoTracking()
            .ToListAsync();

    public async Task<RecordModel?> GetByIdAsync(Guid id) =>
        await this.context.Records
            .Where(r => r.Id == id)
            .Select(r => new RecordModel
            {
                Id = r.Id,
                Description = r.Description,
                Result = r.Result,
                ReferenceRange = r.ReferenceRange,
                MedicationDetails = r.MedicationDetails,
                Type = r.Type,
            })
            .FirstOrDefaultAsync();

    public async Task<MutationResult> CreateRecordAsync(RecordModel record)
    {
        try
        {
            var recordEntity = new Data.Record
            {
                Id = record.Id,
                Description = record.Description,
                Result = record.Result,
                ReferenceRange = record.ReferenceRange,
                MedicationDetails = record.MedicationDetails,
                Type = record.Type,
                Date = DateTime.UtcNow,
            };

            this.context.Records.Add(recordEntity);
            await this.context.SaveChangesAsync();

            return MutationResult.ResultFrom(recordEntity.Id);
        }
        catch (Exception e)
        {
            this.logger.LogError(e, e.Message);
            return MutationResult.ResultFrom(null, "Error creating record");
        }
    }

    public async Task<MutationResult> UpdateRecordAsync(Guid id, RecordModel record)
    {
        try
        {
            var recordEntity = await this.context.Records.FindAsync(id);
            if (recordEntity == null)
            {
                return MutationResult.ResultFrom("Record not found");
            }

            recordEntity.Description = record.Description;
            recordEntity.Result = record.Result;
            recordEntity.ReferenceRange = record.ReferenceRange;
            recordEntity.MedicationDetails = record.MedicationDetails;
            recordEntity.Type = record.Type;

            this.context.Records.Update(recordEntity);
            await this.context.SaveChangesAsync();

            return MutationResult.ResultFrom(recordEntity.Id);
        }
        catch (Exception e)
        {
            this.logger.LogError(e, e.Message);
            return MutationResult.ResultFrom(null, "Error updating record");
        }
    }

    public async Task<MutationResult> DeleteRecordAsync(Guid id)
    {
        try
        {
            var recordEntity = await this.context.Records.FindAsync(id);
            if (recordEntity == null)
            {
                return MutationResult.ResultFrom("Record not found");
            }

            this.context.Records.Remove(recordEntity);
            await this.context.SaveChangesAsync();

            return MutationResult.ResultFrom(recordEntity.Id);
        }
        catch (Exception e)
        {
            this.logger.LogError(e, e.Message);
            return MutationResult.ResultFrom(null, "Error deleting record");
        }
    }

    public async Task<List<RecordModel>> GetRecordsByPatientIdAsync(Guid patientId)
    {
        return await this.context.Records
            .Where(r => r.PatientId == patientId)
            .Select(r => new RecordModel
            {
                Id = r.Id,
                Description = r.Description,
                Result = r.Result,
                ReferenceRange = r.ReferenceRange,
                MedicationDetails = r.MedicationDetails,
                Type = r.Type,
            })
            .ToListAsync();
    }
}
