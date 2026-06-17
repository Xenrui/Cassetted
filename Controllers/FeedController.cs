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
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            var reviews = tab == "friends"
                ? await _feedService.GetFriendsFeedAsync(user.Id)
                : await _feedService.GetForYouFeedAsync(user.Id);

            var trending = await _feedService.GetTrendingItemsAsync();
            var viewModel = new FeedViewModel
            {
                Reviews = reviews,
                TrendingItems = trending,
                ActiveTab = tab,
                CurrentUserDisplayName = user.DisplayName
            };

            return View(viewModel);
        }
    }
}
