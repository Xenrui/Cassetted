namespace Cassetted.Models.ViewModels
{
    public class ActivityFeedViewModel
    {
        public IList<Review> FeedReviews { get; set; } = new List<Review>();
        public bool IsAuthenticated { get; set; }
    }
}
