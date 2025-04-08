using HealthRec.Data.Entities;
using HealthRec.Services.Doctor.Model;
using HealthRec.Services.Doctor.Models;

namespace HealthRec.Presentation.Models;

public class DoctorViewModel
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public Specialisation? Specialisation { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
}