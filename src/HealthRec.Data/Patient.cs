using System.ComponentModel.DataAnnotations;

namespace HealthRec.Data.Entities;

public class Patient : ApplicationUser
{
    [Required]
    [StringLength(8)]
    [RegularExpression(@"^\d{8}$", ErrorMessage = "Security code must be exactly 8 digits")]
    public string? Code { get; set; }
    public DateTime DateOfBirth { get; set; }

    public ICollection<DoctorPatient>? Doctors { get; set; } = new List<DoctorPatient>();
}