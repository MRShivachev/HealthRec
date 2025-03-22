using Essentials.Results;
using HealthRec.Services.Doctor.Model;

namespace HealthRec.Services.Doctor.Contract;

public interface IDoctorService
{
    public Task<IEnumerable<DoctorModel>> GetAllAsync();
    public Task<DoctorModel?> GetByIdAsync(Guid id);
    public Task<MutationResult?> UpdateDoctorAsync(Guid id, DoctorModel doctor);
    public Task<MutationResult?> CreateDoctorAsync(DoctorModel doctor);
    public Task<MutationResult?> DeleteDoctorAsync(Guid id);

}