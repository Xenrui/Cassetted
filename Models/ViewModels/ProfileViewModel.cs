namespace Cassetted.Models.ViewModels
{
    public class ProfileViewModel
    {
        public ApplicationUser ProfileUser { get; set; } = null!;
        public IList<Review> Reviews { get; set; } = new List<Review>();
        public int FollowerCount { get; set; }
        public int FollowingCount { get; set; }
        public bool IsFollowing { get; set; }
        public bool IsOwnProfile { get; set; }
    }
}
