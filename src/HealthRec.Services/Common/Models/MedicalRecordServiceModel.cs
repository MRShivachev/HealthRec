namespace HealthRec.Services.MedicalRecords.Models;

public class MedicalRecordEntryViewModel
{
    public int Id { get; set; }
    public DateTime EntryDate { get; set; }
    public int PatientId { get; set; }
    public string PatientName { get; set; }
    public int DoctorId { get; set; }
    public string DoctorName { get; set; }
    public string VisitType { get; set; }
    public string Symptoms { get; set; }
    public string Diagnosis { get; set; }
    public string Treatment { get; set; }
    public string Notes { get; set; }
    public List<PrescriptionViewModel> Prescriptions { get; set; }
}

public class PrescriptionViewModel
{
    public int Id { get; set; }
    public string MedicationName { get; set; }
    public string Dosage { get; set; }
    public string Frequency { get; set; }
    public int DurationDays { get; set; }
    public string Instructions { get; set; }
    public DateTime PrescribedDate { get; set; }
    public string DoctorName { get; set; }
}