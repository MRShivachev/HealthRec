namespace HealthRec.Services.Doctors.Models;

public class DoctorProfileViewModel
{
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string LicenseNumber { get; set; }
    public string Specialty { get; set; }
    public string PhoneNumber { get; set; }
    public string Biography { get; set; }
    public bool IsAvailableForNewPatients { get; set; }
}

public class ScheduleSlotViewModel
{
    public int Id { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public bool IsAvailable { get; set; }
    public string PatientName { get; set; }
    public int? AppointmentId { get; set; }
}