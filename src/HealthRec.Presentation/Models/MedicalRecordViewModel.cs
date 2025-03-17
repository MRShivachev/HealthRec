namespace HealthRec.Presentation.Models;

public class MedicalRecordViewModel
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public DateTime RecordDate { get; set; }
    public string RecordType { get; set; } // "Diagnosis", "Test", "Medication", "Vaccination", "Appointment"
    public string DoctorName { get; set; }
    public string DoctorSpecialty { get; set; }
    public string Department { get; set; }
    public DateTime CreatedDate { get; set; }
        
    public string DiagnosisCode { get; set; }
    public string TreatmentPlan { get; set; }
    public string TestResults { get; set; }
    public string NormalRange { get; set; }
    public string MedicationDetails { get; set; }
    public string Dosage { get; set; }
    public string Instructions { get; set; }
    public string Notes { get; set; }
}
