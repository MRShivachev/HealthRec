using HealthRec.Data;

namespace HealthRec.Presentation.Models;

public class MedicalRecordViewModel
{
    public long? Id { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public DateTime RecordDate { get; set; }
    public RecordType RecordType { get; set; } // "Diagnosis", "Test", "Medication", "Vaccination", "Appointment"
    public string? DoctorName { get; set; }
    public string? DoctorSpecialty { get; set; }
    public string? Department { get; set; }
    public DateTime CreatedDate { get; set; }
    public Guid PatientId { get; set; }
    public string? DiagnosisCode { get; set; }
    public string? TreatmentPlan { get; set; }
    public string? TestResults { get; set; }
    public string? NormalRange { get; set; }
    public string? MedicationDetails { get; set; }
    public string? Dosage { get; set; }
    public string? Instructions { get; set; }
    public string? Notes { get; set; }
    public string? Severity { get; set; }

    public string? TestType { get; set; }
    public string? MedicationName { get; set; }
    public string? Duration { get; set; }
    public string? Prescription { get; set; }
    public string? VaccineName { get; set; }
    public string? VaccineDose { get; set; }
}
