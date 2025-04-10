namespace HealthRec.Services.Patient.Models;

public class PatientModel
{
    public Guid Id { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Code { get; set; }
    public string? Password { get; set; } // Only used during creation
    public DateTime DateOfBirth { get; set; }
    public Guid AssignedDoctorId { get; set; }
    public string? PhoneNumber { get; set; }
    public string? SecurityCode { get; set; }
}