using System.ComponentModel.DataAnnotations;

namespace HealthRec.Services.Doctor.Models;

public enum SpecialisationModel
{
    [Display(Name = "Кардиолог")]
    Кардиолог = 1,
    [Display(Name = "Невролог")]
    Невролог = 2,
    [Display(Name = "Онколог")]
    Онколог = 3,
    [Display(Name = "Дерматолог")]
    Дерматолог = 4,
    [Display(Name = "Гастроентеролог")]
    Гастроентеролог = 5,
}