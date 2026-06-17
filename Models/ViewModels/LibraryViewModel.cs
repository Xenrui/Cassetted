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
        public int FavoriteCount { get; set; }
        public string BadgeSlug => CategoryName.ToLowerInvariant().Replace(" ", "");
        public int FilledStars => (int)Math.Round(Rating);
    }

    public class FavoriteCardViewModel
    {
        public int ReviewId { get; set; }
        public string ItemName { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public decimal Rating { get; set; }
        public string Body { get; set; } = string.Empty;
        public string AuthorUserId { get; set; } = string.Empty;
        public string AuthorDisplayName { get; set; } = string.Empty;
        public DateTime SavedAt { get; set; }
        public string BadgeSlug => CategoryName.ToLowerInvariant().Replace(" ", "");
        public int FilledStars => (int)Math.Round(Rating);
    }

    public class LibraryViewModel
    {
        public List<LibraryReviewViewModel> RecentReviews { get; set; } = [];
        public List<FavoriteCardViewModel> Favorites { get; set; } = [];
        public string UserDisplayName { get; set; } = string.Empty;
    }
}
