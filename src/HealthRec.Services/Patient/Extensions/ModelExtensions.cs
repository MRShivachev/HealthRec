using HealthRec.Services.Patient.Models;

namespace HealthRec.Services.Patient.Extensions;

public static class ModelExtensions
{
    public static PatientModel ToModel(this Data.Entities.Patient patient) =>
        new()
        {
            Email = patient.Email,
            Code = patient.Code,
            FirstName = patient.FirstName,
            LastName = patient.LastName,
            Id = patient.Id,
            Phone = patient.PhoneNumber

        };

}