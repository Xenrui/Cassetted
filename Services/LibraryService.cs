using Cassetted.Data;
using Cassetted.Models.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace Cassetted.Services
{
    public class LibraryService
    {
        private readonly ApplicationDbContext _db;

        public LibraryService(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<LibraryViewModel> GetLibraryAsync(string userId, string displayName)
        {
            var recent = await _db.Reviews
                .AsNoTracking()
                .Where(r => r.UserId == userId)
                .OrderByDescending(r => r.CreatedAt)
                .Take(10)
                .Select(r => new LibraryReviewViewModel
                {
                    Id = r.Id,
                    ItemName = r.Item.Name,
                    CategoryName = r.Item.Category.Name,
                    Rating = r.Rating,
                    Body = r.Body,
                    CreatedAt = r.CreatedAt,
                    FavoriteCount = r.Favorites.Count
                })
                .ToListAsync();

            var favorites = await _db.ReviewFavorites
                .AsNoTracking()
                .Where(f => f.UserId == userId)
                .OrderByDescending(f => f.CreatedAt)
                .Select(f => new FavoriteCardViewModel
                {
                    ReviewId = f.ReviewId,
                    ItemName = f.Review.Item.Name,
                    CategoryName = f.Review.Item.Category.Name,
                    Rating = f.Review.Rating,
                    Body = f.Review.Body,
                    AuthorUserId = f.Review.UserId,
                    AuthorDisplayName = f.Review.User.DisplayName,
                    SavedAt = f.CreatedAt
                })
                .ToListAsync();

            return new LibraryViewModel
            {
                UserDisplayName = displayName,
                RecentReviews = recent,
                Favorites = favorites
            };
        }
    }
}
