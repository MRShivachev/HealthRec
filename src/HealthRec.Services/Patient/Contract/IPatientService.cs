using Essentials.Results;
using HealthRec.Services.Patient.Models;

namespace HealthRec.Services.Patient.Contract;

public interface IPatientService
{
    public Task<IEnumerable<PatientModel>> GetAllAsync();

    public Task<PatientModel?> GetByIdAsync(Guid id);

    public Task<MutationResult> CreatePatientsAsync(PatientModel patient);

    public Task<MutationResult> UpdatePatientsAsync(Guid id, PatientModel patient);

    public Task<MutationResult> DeletePatientsAsync(Guid id);
    Task<List<PatientModel>> GetPatientsByDoctorIdAsync(Guid doctorId);
    Task<MutationResult> CreatePatientWithDoctorAsync(
        PatientModel patient,
        string password,
        DateTime dateOfBirth,
        Guid assignedDoctorId,
        Guid currentDoctorId);
    Task<string> GenerateUniqueSecurityCodeAsync();
}