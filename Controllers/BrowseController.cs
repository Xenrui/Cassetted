using Cassetted.Models.ViewModels;
using Cassetted.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cassetted.Controllers
{
    [Authorize]
    public class BrowseController : Controller
    {
        private static readonly Dictionary<int, string> CategoryNames = new()
        {
            { 1, "Movies" }, { 2, "TV Shows" }, { 3, "Music" },
            { 4, "Books" }, { 5, "Anime" }, { 6, "Games" }
        };

        private const int PageSize = 10;

        private readonly BrowseService _browseService;

        public BrowseController(BrowseService browseService)
        {
            _browseService = browseService;
        }

        public async Task<IActionResult> Explore(string sort = "rating-desc", int? category = null)
        {
            var items = await _browseService.GetExploreItemsAsync(category, sort);
            return View(new ExploreViewModel { Items = items, SortBy = sort, CategoryId = category });
        }

        public async Task<IActionResult> Item(int id)
        {
            var item = await _browseService.GetItemDetailsAsync(id);
            if (item == null) return NotFound();
            return View(item);
        }

        public async Task<IActionResult> Index(int? category = null, int page = 1)
        {
            if (page < 1) page = 1;

            var popularItems = await _browseService.GetPopularItemsAsync(category);
            var reviews = await _browseService.GetCommunityReviewsAsync(category, page, PageSize);
            var totalReviews = await _browseService.GetReviewCountAsync(category);

            string heading = category.HasValue && CategoryNames.TryGetValue(category.Value, out var name)
                ? name
                : "Browse";

            var viewModel = new BrowseViewModel
            {
                PopularItems = popularItems,
                Reviews = reviews,
                CurrentPage = page,
                TotalPages = Math.Max(1, (int)Math.Ceiling((double)totalReviews / PageSize)),
                CategoryId = category,
                CategoryHeading = heading
            };

            return View(viewModel);
        }
    }
}
