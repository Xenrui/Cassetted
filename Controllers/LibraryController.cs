using Cassetted.Models;
using Cassetted.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Cassetted.Controllers
{
    [Authorize]
    public class LibraryController : Controller
    {
        private readonly LibraryService _libraryService;
        private readonly UserManager<ApplicationUser> _userManager;

        public LibraryController(LibraryService libraryService, UserManager<ApplicationUser> userManager)
        {
            _libraryService = libraryService;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            var viewModel = await _libraryService.GetLibraryAsync(user.Id, user.DisplayName);
            return View(viewModel);
        }
    }
}
