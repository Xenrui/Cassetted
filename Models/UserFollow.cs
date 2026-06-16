namespace Cassetted.Models
{
    public class UserFollow
    {
        public string FollowerId { get; set; } = string.Empty;
        public string FollowedId { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ApplicationUser Follower { get; set; } = null!;
        public ApplicationUser Followed { get; set; } = null!;
    }
}
