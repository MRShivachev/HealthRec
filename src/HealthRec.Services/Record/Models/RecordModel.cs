using HealthRec.Data;

namespace HealthRec.Services.Record.Models;

public class RecordModel
{
    public Guid Id { get; set; }
    public string? Description { get; set; }
    public string? Result { get; set; }
    public string? ReferenceRange { get; set; }
    public string? MedicationDetails { get; set; }
    public RecordType Type { get; set; }
}