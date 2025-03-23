using HealthRec.Services.Doctor.Model;

namespace HealthRec.Presentation.Models;

public class DoctorDetailsViewModel
{
    public DoctorModel? Doctor { get; set; }
    public List<PatientViewModel> Patients { get; set; } = new List<PatientViewModel>();
}