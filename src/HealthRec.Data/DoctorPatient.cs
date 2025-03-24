using System.ComponentModel.DataAnnotations.Schema;
using HealthRec.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HealthRec.Data;

public class DoctorPatient
{
    public long Id { get; set; }
    public Guid PatientId { get; set; }
    public Guid DoctorId { get; set; }
    public Patient? Patient { get; set; }
    public Doctor? Doctor { get; set; }

    internal class Configuration : IEntityTypeConfiguration<DoctorPatient>
    {
        public void Configure(EntityTypeBuilder<DoctorPatient> builder)
        {
            builder.HasKey(dp => dp.Id);

            // Set the table name to plural form
            builder.ToTable("DoctorPatients");

            // Configure the relationships with NoAction to prevent cascade path cycles
            builder.HasOne(dp => dp.Doctor)
                .WithMany(d => d.Patients)
                .HasForeignKey(dp => dp.DoctorId)
                .OnDelete(DeleteBehavior.NoAction); // Changed to NoAction to prevent cascade cycles

            builder.HasOne(dp => dp.Patient)
                .WithMany(p => p.Doctors)
                .HasForeignKey(dp => dp.PatientId)
                .OnDelete(DeleteBehavior.NoAction); // Changed to NoAction to prevent cascade cycles

            // Create index on both IDs for better query performance
            builder.HasIndex(dp => new { dp.DoctorId, dp.PatientId }).IsUnique();
        }
    }
}