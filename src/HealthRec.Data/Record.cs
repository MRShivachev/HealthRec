using HealthRec.Data.Entities;

namespace HealthRec.Data;

public class Record
{
    public Guid Id { get; set; }
    public DateTime? NextAppointment { get; set; }
    public string? Description { get; set; }
    public RecordType Type { get; set; }
    public string? Result { get; set; }
    public string? ReferenceRange { get; set; }
    public string? MedicationDetails { get; set; }
    public DateTime Date { get; set; }
    public Guid PatientId { get; set; }
    public Patient? Patient { get; set; }
}