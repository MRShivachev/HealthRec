using HealthRec.Services.Patient.Models;

namespace HealthRec.Services.Patient.Extensions;

public static class ModelExtensions
{
    public static PatientModel ToModel(this Data.Entities.Patient patient) =>
        new()
        {
            Id = patient.Id,
            Email = patient.Email,
            Code = patient.Code,
            FirstName = patient.FirstName,
            LastName = patient.LastName,
            Phone = patient.PhoneNumber,
            DateOfBirth = patient.DateOfBirth,
            SecurityCode = patient.Code,
        };
}