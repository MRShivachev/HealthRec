using HealthRec.Services.Record.Models;

namespace HealthRec.Services.Record.Extensions;

public static class ModelExtensions
{
    public static RecordModel ToModel(this Data.Record record) =>
        new()
        {
            Id = record.Id,
            Description = record.Description,
            ReferenceRange = record.ReferenceRange,
            Result = record.Result,
            Type = record.Type,
            MedicationDetails = record.MedicationDetails,
        };
}