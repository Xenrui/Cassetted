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
            var userId = _userManager.GetUserId(User);
            if (userId == null) return Challenge();

            var viewModel = await _reviewService.GetReviewDetailsAsync(id, userId);
            if (viewModel == null) return NotFound();

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddComment(int reviewId, string body)
        {
            if (!string.IsNullOrWhiteSpace(body))
            {
                var userId = _userManager.GetUserId(User);
                if (userId == null) return Challenge();

                await _reviewService.AddCommentAsync(reviewId, userId, body);
            }

            return RedirectToAction(nameof(Details), new { id = reviewId });
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null) return Challenge();

            var viewModel = await _reviewService.GetReviewForEditAsync(id, userId);
            if (viewModel == null) return NotFound();
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Edit")]
        public async Task<IActionResult> EditPost(int id, EditReviewInputModel input)
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null) return Challenge();

            if (!ModelState.IsValid)
            {
                var vm = await _reviewService.GetReviewForEditAsync(id, userId);
                if (vm == null) return NotFound();
                vm.Input = input;
                return View(vm);
            }

            var success = await _reviewService.UpdateReviewAsync(id, userId, input);
            if (!success) return NotFound();
            return RedirectToAction(nameof(Details), new { id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleLike(int reviewId)
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null) return Challenge();

            var (liked, likeCount) = await _reviewService.ToggleLikeAsync(reviewId, userId);
            return Json(new { liked, likeCount });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleFavorite(int reviewId)
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null) return Challenge();

            var (favorited, favoriteCount) = await _reviewService.ToggleFavoriteAsync(reviewId, userId);
            return Json(new { favorited, favoriteCount });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null) return Challenge();

            await _reviewService.DeleteReviewAsync(id, userId);
            return RedirectToAction("Index", "Feed");
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

            var userId = _userManager.GetUserId(User);
            if (userId == null) return Challenge();

            var (success, error) = await _reviewService.CreateReviewAsync(userId, input);
            if (!success)
                return Json(new { success = false, errors = new[] { error } });

            return Json(new { success = true });
        }
    }
}
