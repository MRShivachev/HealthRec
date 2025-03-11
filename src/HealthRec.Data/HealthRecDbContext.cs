using HealthRec.Data.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace HealthRec.Data;

public class HealthRecDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>
{
    public HealthRecDbContext(DbContextOptions<HealthRecDbContext> options) 
        : base(options)
    {
        
    }
    public DbSet<Patient> Patients { get; set; }
    public DbSet<Doctor> Doctors { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Doctor>().ToTable("Doctors");
        modelBuilder.Entity<Patient>().ToTable("Patients");
    }
}