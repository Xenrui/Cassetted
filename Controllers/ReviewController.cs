using Cassetted.Models;
using Cassetted.Models.ViewModels;
using Cassetted.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Cassetted.Controllers
{
    [Authorize]
    public class ReviewController : Controller
    {
        private readonly ReviewService _reviewService;
        private readonly UserManager<ApplicationUser> _userManager;

        public ReviewController(ReviewService reviewService, UserManager<ApplicationUser> userManager)
        {
            _reviewService = reviewService;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var userId = _userManager.GetUserId(User)!;
            var viewModel = await _reviewService.GetReviewDetailsAsync(id, userId);

            if (viewModel == null)
                return NotFound();

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddComment(int reviewId, string body)
        {
            if (!string.IsNullOrWhiteSpace(body))
            {
                var userId = _userManager.GetUserId(User)!;
                await _reviewService.AddCommentAsync(reviewId, userId, body);
            }

            return RedirectToAction(nameof(Details), new { id = reviewId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateReviewInputModel input)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage);
                return Json(new { success = false, errors });
            }

            var user = await _userManager.GetUserAsync(User);
            var (success, error) = await _reviewService.CreateReviewAsync(user!.Id, input);

            if (!success)
                return Json(new { success = false, errors = new[] { error } });

            return Json(new { success = true });
        }
    }
}
