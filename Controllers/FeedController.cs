using Cassetted.Models;
using Cassetted.Models.ViewModels;
using Cassetted.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Cassetted.Controllers
{
    [Authorize]
    public class FeedController : Controller
    {
        private readonly FeedService _feedService;
        private readonly UserManager<ApplicationUser> _userManager;

        public FeedController(FeedService feedService, UserManager<ApplicationUser> userManager)
        {
            _feedService = feedService;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index(string tab = "foryou")
        {
            var userId = _userManager.GetUserId(User)!;
            var user = await _userManager.GetUserAsync(User);

            var reviews = tab == "friends"
                ? await _feedService.GetFriendsFeedAsync(userId)
                : await _feedService.GetForYouFeedAsync(userId);

            var viewModel = new FeedViewModel
            {
                Reviews = reviews,
                TrendingItems = await _feedService.GetTrendingItemsAsync(),
                ActiveTab = tab,
                CurrentUserDisplayName = user?.DisplayName ?? string.Empty
            };

            return View(viewModel);
        }
    }
}
