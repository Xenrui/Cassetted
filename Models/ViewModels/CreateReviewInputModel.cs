using System.ComponentModel.DataAnnotations;

namespace Cassetted.Models.ViewModels
{
    public class CreateReviewInputModel
    {
        [Required]
        [Range(1, 6)]
        public int CategoryId { get; set; }

        [Required(ErrorMessage = "Please enter a title.")]
        [StringLength(200, MinimumLength = 1)]
        public string ItemName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please select a rating.")]
        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5.")]
        public int Rating { get; set; }

        [Required(ErrorMessage = "Please write a comment.")]
        [StringLength(2000, ErrorMessage = "Comment must not exceed 2000 characters.")]
        public string Body { get; set; } = string.Empty;

        public bool IsFavorited { get; set; } = false;
    }
}
