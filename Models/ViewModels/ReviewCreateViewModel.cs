using System.ComponentModel.DataAnnotations;

namespace Cassetted.Models.ViewModels
{
    public class ReviewCreateViewModel
    {
        public int ItemId { get; set; }
        public string ItemName { get; set; } = string.Empty;

        [Required]
        [Range(0.5, 5.0)]
        public decimal Rating { get; set; }

        [MaxLength(200)]
        public string? Title { get; set; }

        [Required]
        public string Body { get; set; } = string.Empty;
    }
}
