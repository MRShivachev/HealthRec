using HealthRec.Services.Doctor.Model;
using HealthRec.Services.Patient.Models;

namespace HealthRec.Presentation.Models;

public class DoctorDetailsViewModel
{
    public DoctorModel? Doctor { get; set; }
    public IEnumerable<PatientModel>? Patients { get; set; }
}