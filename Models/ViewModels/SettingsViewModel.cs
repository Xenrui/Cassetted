using System.ComponentModel.DataAnnotations;

namespace Cassetted.Models.ViewModels
{
    public class SettingsViewModel
    {
        [Required(ErrorMessage = "Username is required.")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Username must be between 2 and 50 characters.")]
        [RegularExpression(@"^[a-zA-Z0-9_\-\.]+$", ErrorMessage = "Username can only contain letters, numbers, underscores, hyphens, and periods.")]
        [Display(Name = "Username")]
        public string DisplayName { get; set; } = string.Empty;

        [StringLength(300, ErrorMessage = "Bio cannot exceed 300 characters.")]
        [Display(Name = "Bio")]
        public string? Bio { get; set; }

        public string Email { get; set; } = string.Empty;
    }
}
