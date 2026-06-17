namespace Cassetted.Models.ViewModels
{
    public class CategoryActivityViewModel
    {
        public string CategoryName { get; set; } = string.Empty;
        public int ReviewCount { get; set; }
    }

    public class ProfileViewModel
    {
        public string UserId { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string? Bio { get; set; }
        public int ReviewCount { get; set; }
        public int FollowingCount { get; set; }
        public int FollowerCount { get; set; }
        public decimal AverageRating { get; set; }
        public string? MostReviewedCategory { get; set; }
        public string? HighestRatedItemName { get; set; }
        public List<CategoryActivityViewModel> CategoryActivity { get; set; } = [];
        public bool IsOwnProfile { get; set; }
        public bool IsFollowing { get; set; }
    }
}
