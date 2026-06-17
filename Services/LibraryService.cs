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
            var recent = await BaseQuery(userId)
                .OrderByDescending(r => r.CreatedAt)
                .Take(10)
                .ToListAsync();

            var favorites = await BaseQuery(userId)
                .Where(r => r.IsFavorited)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();

            return new LibraryViewModel
            {
                UserDisplayName = displayName,
                RecentReviews = recent,
                Favorites = favorites
            };
        }

        private IQueryable<LibraryReviewViewModel> BaseQuery(string userId)
        {
            return _db.Reviews
                .AsNoTracking()
                .Where(r => r.UserId == userId)
                .Select(r => new LibraryReviewViewModel
                {
                    Id = r.Id,
                    ItemName = r.Item.Name,
                    CategoryName = r.Item.Category.Name,
                    Rating = r.Rating,
                    Body = r.Body,
                    CreatedAt = r.CreatedAt,
                    IsFavorited = r.IsFavorited
                });
        }
    }
}
