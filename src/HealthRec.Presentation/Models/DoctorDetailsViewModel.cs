namespace HealthRec.Presentation.Models;

public class DoctorDetailsViewModel
{
    public DoctorViewModel Doctor { get; set; }
    public List<PatientViewModel> Patients { get; set; } = new List<PatientViewModel>();
}