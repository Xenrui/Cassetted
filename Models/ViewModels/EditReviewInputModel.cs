using System.ComponentModel.DataAnnotations;

namespace Cassetted.Models.ViewModels
{
    public class EditReviewInputModel
    {
        [Required]
        [Range(1, 5)]
        public int Rating { get; set; }

        [Required(ErrorMessage = "Please write a review.")]
        [StringLength(2000, ErrorMessage = "Review must not exceed 2000 characters.")]
        public string Body { get; set; } = string.Empty;

        public bool IsFavorited { get; set; }
    }

    public class EditReviewViewModel
    {
        public int ReviewId { get; set; }
        public string ItemName { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public EditReviewInputModel Input { get; set; } = new();
    }
}
