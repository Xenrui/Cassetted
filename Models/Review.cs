namespace Cassetted.Models
{
    public class Review
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public int ItemId { get; set; }
        public decimal Rating { get; set; }
        public string? Title { get; set; }
        public string Body { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public ApplicationUser User { get; set; } = null!;
        public Item Item { get; set; } = null!;
        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
        public ICollection<ReviewLike> Likes { get; set; } = new List<ReviewLike>();
        public ICollection<ReviewFavorite> Favorites { get; set; } = new List<ReviewFavorite>();
    }
}
