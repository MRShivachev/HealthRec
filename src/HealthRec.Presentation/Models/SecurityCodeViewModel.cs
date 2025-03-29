using System.ComponentModel.DataAnnotations;

namespace HealthRec.Presentation.Models
{
    public class SecurityCodeViewModel
    {
        [Required(ErrorMessage = "Security code is required")]
        [RegularExpression(@"^\d{8}$", ErrorMessage = "Security code must be exactly 8 digits")]
        [Display(Name = "Security Code")]
        public string? SecurityCode { get; set; }

        public bool RememberMe { get; set; }
    }
}
