using System.Collections;

namespace HealthRec.Presentation.Models;

public class DoctorIndexViewModel
{
    public IEnumerable<DoctorViewModel>? Doctors { get; set; }
    public string? SearchQuery { get; set; }
}