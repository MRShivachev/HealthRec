using HealthRec.Data.Entities;
using HealthRec.Services.Doctor.Models;
using HealthRec.Services.Patient.Models;

namespace HealthRec.Services.Doctor.Model;

public class DoctorModel
{
    public Guid Id { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public SpecialisationModel Specialisation { get; set; }
    public string? Password { get; set; }
    public List<PatientModel?>? Patients { get; set; }
}