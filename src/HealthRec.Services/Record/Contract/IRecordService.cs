using Essentials.Results;
using HealthRec.Services.Record.Models;

namespace HealthRec.Services.Record.Contract;

public interface IRecordService
{
    public Task<IEnumerable<RecordModel>> GetAllAsync();
    public Task<RecordModel?> GetByIdAsync(long id);
    public Task<MutationResult> CreateRecordAsync(RecordModel record);
    public Task<MutationResult> UpdateRecordAsync(long id, RecordModel record);
    public Task<MutationResult> DeleteRecordAsync(long id);

    public Task<List<RecordModel>> GetRecordsByPatientIdAsync(Guid patientId);
}