using Essentials.Results;
using HealthRec.Services.Doctor.Model;
using HealthRec.Services.Patient.Models;

namespace HealthRec.Services.Doctor.Contract;

public interface IDoctorService
{
    public Task<IEnumerable<DoctorModel>> GetAllAsync();
    public Task<DoctorModel?> GetByIdAsync(Guid id);
    public Task<MutationResult?> UpdateDoctorAsync(Guid id, DoctorModel doctor);
    public Task<MutationResult?> CreateDoctorAsync(DoctorModel doctor);
    public Task<MutationResult?> DeleteDoctorAsync(Guid id);
    public Task<bool> IsDoctorForPatientAsync(Guid patientId, Guid userId);
    public Task<IEnumerable<PatientModel>> GetPatientsByDoctorIdAsync(Guid? doctorId);
}