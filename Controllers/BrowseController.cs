using Cassetted.Models.ViewModels;
using Cassetted.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cassetted.Controllers
{
    [Authorize]
    public class BrowseController : Controller
    {
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

            string heading = "Browse";
            if (category.HasValue)
            {
                var name = await _browseService.GetCategoryNameAsync(category.Value);
                if (name != null) heading = name;
            }

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
