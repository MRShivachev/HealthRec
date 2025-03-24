using System.Collections;

namespace HealthRec.Presentation.Models;

public class DoctorIndexViewModel
{
    public List<DoctorListItemViewModel>? Doctors { get; set; }
    public string? SearchQuery { get; set; }
}