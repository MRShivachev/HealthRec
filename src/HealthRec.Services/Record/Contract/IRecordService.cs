using Essentials.Results;
using HealthRec.Services.Record.Models;

namespace HealthRec.Services.Record.Contract;

public interface IRecordService
{
    public Task<IEnumerable<RecordModel>> GetAllAsync();
    public Task<RecordModel?> GetByIdAsync(Guid id);
    public Task<MutationResult> CreateRecordAsync(RecordModel record);
    public Task<MutationResult> UpdateRecordAsync(Guid id, RecordModel record);
    public Task<MutationResult> DeleteRecordAsync(Guid id);
}