using Cassetted.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cassetted.Controllers
{
    [Authorize]
    public class SearchController : Controller
    {
        private readonly SearchService _searchService;

        public SearchController(SearchService searchService)
        {
            _searchService = searchService;
        }

        public async Task<IActionResult> Index(string? q)
        {
            var viewModel = await _searchService.SearchAsync(q);
            return View(viewModel);
        }
    }
}
