using System.ComponentModel.DataAnnotations;

namespace HealthRec.Presentation.Models;

public class PatientViewModel
{
    public Guid Id { get; set; }

    [Display(Name = "Name")]
    public string? Name { get; set; }

    [Display(Name = "Email")]
    public string? Email { get; set; }

    [Display(Name = "Security Code")]
    public string? SecurityCode { get; set; }

    [Display(Name = "Date of Birth")]
    [DataType(DataType.Date)]
    public DateTime DateOfBirth { get; set; }

    [Display(Name = "Medical Record Number")]
    public string? MedicalRecordNumber { get; set; }

    [Display(Name = "Phone Number")]
    public string? PhoneNumber { get; set; }
}