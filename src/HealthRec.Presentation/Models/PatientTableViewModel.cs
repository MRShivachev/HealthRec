using HealthRec.Data.Entities;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HealthRec.Presentation.Models;

public class PatientTableViewModel
{
    public List<PatientViewModel> Patients { get; set; } = new();
    public Guid? DoctorId { get; set; }
}
