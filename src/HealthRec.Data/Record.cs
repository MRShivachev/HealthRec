using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HealthRec.Data.Entities;

namespace HealthRec.Data;

public class Record : Entity
{
    public DateTime? NextAppointment { get; set; }
    public RecordType Type { get; set; }
    public string? Result { get; set; }
    public string? ReferenceRange { get; set; }
    public string? MedicationDetails { get; set; }
    public DateTime Date { get; set; }
    public Guid PatientId { get; set; }
    public Patient? Patient { get; set; }

    [StringLength(1000)]
    public string? Description { get; set; }

    public DateTime RecordDate { get; set; }

    [Required]
    public RecordType RecordType { get; set; }

    [StringLength(100)]
    public string? Department { get; set; }

    public DateTime CreatedDate { get; set; } = DateTime.Now;

    public Guid DoctorId { get; set; }

    // Properties specific to Diagnosis
    [StringLength(20)]
    public string? DiagnosisCode { get; set; }

    [StringLength(20)]
    public string? Severity { get; set; }

    [StringLength(1000)]
    public string? TreatmentPlan { get; set; }

    // Properties specific to Test Results
    [StringLength(100)]
    public string? TestType { get; set; }

    [StringLength(200)]
    public string? TestResults { get; set; }

    [StringLength(200)]
    public string? NormalRange { get; set; }

    [StringLength(255)]
    public string? TestResultFilePath { get; set; }

    // Properties specific to Medication
    [StringLength(100)]
    public string? MedicationName { get; set; }

    [StringLength(50)]
    public string? Dosage { get; set; }

    [StringLength(50)]
    public string? Duration { get; set; }

    [StringLength(500)]
    public string? Prescription { get; set; }

    [StringLength(500)]
    public string? Instructions { get; set; }

    // Properties specific to Vaccination
    [StringLength(100)]
    public string? VaccineName { get; set; }

    [StringLength(50)]
    public string? VaccineDose { get; set; }

    // Common fields
    [StringLength(1000)]
    public string? Notes { get; set; }
}