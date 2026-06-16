namespace Cassetted.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public int ReviewId { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public Review Review { get; set; } = null!;
        public ApplicationUser User { get; set; } = null!;
    }
}
