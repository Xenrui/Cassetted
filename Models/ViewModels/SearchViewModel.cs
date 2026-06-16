namespace Cassetted.Models.ViewModels
{
    public class SearchViewModel
    {
        public string? Query { get; set; }
        public IList<Item> Items { get; set; } = new List<Item>();
        public IList<ApplicationUser> Users { get; set; } = new List<ApplicationUser>();
    }
}
