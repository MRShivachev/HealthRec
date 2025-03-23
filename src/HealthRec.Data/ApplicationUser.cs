using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace HealthRec.Data.Entities;

public class ApplicationUser : IdentityUser<Guid>
{
   [Required]
   [StringLength(50)]
   public string? FirstName { get; set; }
   [Required]
   [StringLength(50)]
   public string? LastName { get; set; }
}