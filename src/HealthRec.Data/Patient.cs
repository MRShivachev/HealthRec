using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HealthRec.Data.Entities;

public class Patient : ApplicationUser
{
    public Patient()
    {
        this.Records = new HashSet<Record>();
    }

    [Required]
    [StringLength(8)]
    [RegularExpression(@"^\d{8}$", ErrorMessage = "Security code must be exactly 8 digits")]
    public string? Code { get; set; }
    public DateTime DateOfBirth { get; set; }

    public ICollection<DoctorPatient>? Doctors { get; set; } = new List<DoctorPatient>();
    public ICollection<Record>? Records { get; set; } = new List<Record>();

    internal class Configuration : IEntityTypeConfiguration<Patient>
    {
        public void Configure(EntityTypeBuilder<Patient> builder)
        {
            builder.HasMany(r => r.Records).WithOne(r => r.Patient).HasForeignKey(r => r.PatientId);
        }
    }
}