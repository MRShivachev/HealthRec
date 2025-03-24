using HealthRec.Services.Doctor.Model;
using HealthRec.Services.Doctor.Models;
using HealthRec.Services.Patient.Extensions;

namespace HealthRec.Services.Doctor.Extensions;

public static class ModelExtensions
{
    public static DoctorModel ToModel(this Data.Entities.Doctor doctor) =>
        new()
        {
            Id = doctor.Id,
            FirstName = doctor.FirstName!,
            LastName = doctor.LastName!,
            Email = doctor.Email!,
            Specialisation = (SpecialisationModel)doctor.Specialisation,
            Patients = doctor.Patients?
                .Select(dp =>
                {
                    if (dp.Patient != null)
                    {
                        return dp.Patient.ToModel();
                    }
                    else
                    {
                        return null;
                    }
                })
                .ToList(),
        };
}