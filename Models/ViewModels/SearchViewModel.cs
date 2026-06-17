namespace Cassetted.Models.ViewModels
{
    public class ItemSuggestionViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal AverageRating { get; set; }
        public int ReviewCount { get; set; }
    }

    public class SearchItemResult
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public decimal AverageRating { get; set; }
        public int ReviewCount { get; set; }
        public string BadgeSlug => CategoryName.ToLowerInvariant().Replace(" ", "");
    }

    public class SearchUserResult
    {
        public string UserId { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public int ReviewCount { get; set; }
    }

    public class SearchViewModel
    {
        public string Query { get; set; } = string.Empty;
        public List<SearchItemResult> Items { get; set; } = [];
        public List<SearchUserResult> Users { get; set; } = [];
        public bool HasResults => Items.Count > 0 || Users.Count > 0;
    }
}
