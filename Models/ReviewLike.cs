namespace Cassetted.Models
{
    public class ReviewLike
    {
        public string UserId { get; set; } = string.Empty;
        public int ReviewId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ApplicationUser User { get; set; } = null!;
        public Review Review { get; set; } = null!;
    }
}
