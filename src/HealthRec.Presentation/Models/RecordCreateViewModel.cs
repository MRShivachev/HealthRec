using HealthRec.Data;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HealthRec.Presentation.Models;

public class RecordCreateViewModel
{
    // The patient to whom this record belongs
    public Guid PatientId { get; set; }

    // Description of the record (e.g. additional notes or context)
    public string? Description { get; set; }

    // The type of the record (e.g. Diagnosis, TestResult, Medication, Vaccination)
    public RecordType Type { get; set; }

    // If the record is a test result, the specific result value
    public string? Result { get; set; }

    // For test results, the normal reference range
    public string? ReferenceRange { get; set; }

    // For medication records, details about the prescribed medication
    public string? MedicationDetails { get; set; }

    // This list is intended for building a dropdown of available record types in the view.
    public IEnumerable<SelectListItem>? AvailableRecordTypes { get; set; }
}