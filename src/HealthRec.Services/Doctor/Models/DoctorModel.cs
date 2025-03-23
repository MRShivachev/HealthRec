using HealthRec.Data.Entities;

namespace HealthRec.Services.Doctor.Model;

public class DoctorModel
{
    public Guid Id { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public SpecialisationModel Specialisation { get; set; }
}