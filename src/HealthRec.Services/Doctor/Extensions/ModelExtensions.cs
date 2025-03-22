using HealthRec.Services.Doctor.Model;

namespace HealthRec.Services.Doctor.Extensions;

public static class ModelExtensions
{
    public static DoctorModel ToModel(this Data.Entities.Doctor doctor) =>
        new()
        {
            Id = doctor.Id,
            FirstName = doctor.FirstName,
            LastName = doctor.LastName,
            Email = doctor.Email!,
            Specialisation = (SpecialisationModel)doctor.Specialisation,
        };
}