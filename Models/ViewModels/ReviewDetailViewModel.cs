namespace Cassetted.Models.ViewModels
{
    public class CommentViewModel
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string UserDisplayName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public string Body { get; set; } = string.Empty;
    }

    public class ReviewDetailViewModel
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
        public bool IsLikedByCurrentUser { get; set; }
        public bool IsOwnReview { get; set; }
        public List<CommentViewModel> Comments { get; set; } = [];
        public string BadgeSlug => CategoryName.ToLowerInvariant().Replace(" ", "");
        public int FilledStars => (int)Math.Round(Rating);
    }
}
