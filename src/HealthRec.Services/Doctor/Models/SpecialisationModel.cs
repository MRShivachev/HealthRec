using System.ComponentModel.DataAnnotations;

namespace HealthRec.Services.Doctor.Models;

public enum SpecialisationModel
{
    [Display(Name = "Cardiologist")]
    Cardiologist = 1,
    [Display(Name = "Neurologist")]
    Neurologist = 2,
    [Display(Name = "Oncologist")]
    Oncologist = 3,
    [Display(Name = "Dermatologist")]
    Dermatologist = 4,
    [Display(Name = "Gastroenterologist")]
    Gastroenterologist = 5,
}