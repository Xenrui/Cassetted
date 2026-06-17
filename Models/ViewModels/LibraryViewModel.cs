namespace Cassetted.Models.ViewModels
{
    public class LibraryReviewViewModel
    {
        public int Id { get; set; }
        public string ItemName { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public decimal Rating { get; set; }
        public string Body { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public bool IsFavorited { get; set; }
    }

    public class LibraryViewModel
    {
        public List<LibraryReviewViewModel> RecentReviews { get; set; } = [];
        public List<LibraryReviewViewModel> Favorites { get; set; } = [];
        public string UserDisplayName { get; set; } = string.Empty;
    }
}
