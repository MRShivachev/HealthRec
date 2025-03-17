using System.ComponentModel.DataAnnotations;

namespace HealthRec.Data.Entities;

public class Patient : ApplicationUser
{
    [Required]
    [StringLength(8)]
    public string Code {get;set;}
}