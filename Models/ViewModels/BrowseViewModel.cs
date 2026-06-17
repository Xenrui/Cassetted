namespace Cassetted.Models.ViewModels
{
    public class BrowseItemViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public decimal AverageRating { get; set; }
        public int ReviewCount { get; set; }
        public int CommentCount { get; set; }
    }

    public class BrowseViewModel
    {
        public List<BrowseItemViewModel> PopularItems { get; set; } = [];
        public List<FeedReviewViewModel> Reviews { get; set; } = [];
        public int CurrentPage { get; set; } = 1;
        public int TotalPages { get; set; } = 1;
        public int? CategoryId { get; set; }
        public string CategoryHeading { get; set; } = "Browse";
    }
}
