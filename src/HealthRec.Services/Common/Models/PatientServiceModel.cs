namespace HealthRec.Services.Patients.Models;

public class PatientProfileViewModel
{
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public DateTime DateOfBirth { get; set; }
    public string MedicalRecordNumber { get; set; }
    public string PhoneNumber { get; set; }
    public string Address { get; set; }
    public string EmergencyContactName { get; set; }
    public string EmergencyContactPhone { get; set; }
}

public class PatientListItemViewModel
{
    public int Id { get; set; }
    public string FullName { get; set; }
    public string MedicalRecordNumber { get; set; }
    public DateTime DateOfBirth { get; set; }
    public DateTime LastAppointment { get; set; }
}