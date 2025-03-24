namespace HealthRec.Presentation.Models;

public class DoctorDropdownViewModel
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? Specialty { get; set; }

    public string DisplayName => $"{this.Name} - {this.Specialty}";
}