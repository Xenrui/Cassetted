namespace Cassetted.Models.ViewModels
{
    public class ItemDetailViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public decimal AverageRating { get; set; }
        public int ReviewCount { get; set; }
        public int CommentCount { get; set; }
        public List<FeedReviewViewModel> Reviews { get; set; } = [];
    }
}
