namespace Cassetted.Models.ViewModels
{
    public class FeedReviewViewModel
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string UserDisplayName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public decimal Rating { get; set; }
        public int ItemId { get; set; }
        public string ItemName { get; set; } = string.Empty;
        public string? Title { get; set; }
        public string Body { get; set; } = string.Empty;
        public int LikeCount { get; set; }
        public int CommentCount { get; set; }
        public bool IsLikedByCurrentUser { get; set; }
    }

    public class TrendingItemViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public decimal AverageRating { get; set; }
        public int ReviewCount { get; set; }
    }

    public class FeedViewModel
    {
        public List<FeedReviewViewModel> Reviews { get; set; } = [];
        public List<TrendingItemViewModel> TrendingItems { get; set; } = [];
        public string ActiveTab { get; set; } = "foryou";
        public string CurrentUserDisplayName { get; set; } = string.Empty;
    }
}
