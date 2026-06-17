using Cassetted.Data;
using Cassetted.Models.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace Cassetted.Services
{
    public class BrowseService
    {
        private readonly ApplicationDbContext _db;

        public BrowseService(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<string?> GetCategoryNameAsync(int categoryId)
        {
            return await _db.Categories
                .AsNoTracking()
                .Where(c => c.Id == categoryId)
                .Select(c => c.Name)
                .FirstOrDefaultAsync();
        }

        public async Task<List<BrowseItemViewModel>> GetExploreItemsAsync(int? categoryId, string sortBy)
        {
            var query = _db.Items
                .AsNoTracking()
                .Where(i => i.Reviews.Any() && (!categoryId.HasValue || i.CategoryId == categoryId.Value))
                .Select(i => new BrowseItemViewModel
                {
                    Id = i.Id,
                    Name = i.Name,
                    CategoryName = i.Category.Name,
                    AverageRating = i.Reviews.Average(r => r.Rating),
                    ReviewCount = i.Reviews.Count,
                    CommentCount = i.Reviews.Sum(r => r.Comments.Count)
                });

            query = sortBy switch
            {
                "rating-asc"   => query.OrderBy(i => i.AverageRating),
                "reviews-desc" => query.OrderByDescending(i => i.ReviewCount),
                "reviews-asc"  => query.OrderBy(i => i.ReviewCount),
                _              => query.OrderByDescending(i => i.AverageRating)
            };

            return await query.ToListAsync();
        }

        public async Task<List<BrowseItemViewModel>> GetPopularItemsAsync(int? categoryId)
        {
            return await _db.Items
                .AsNoTracking()
                .Where(i => i.Reviews.Any() && (!categoryId.HasValue || i.CategoryId == categoryId.Value))
                .Select(i => new BrowseItemViewModel
                {
                    Id = i.Id,
                    Name = i.Name,
                    CategoryName = i.Category.Name,
                    AverageRating = i.Reviews.Average(r => r.Rating),
                    ReviewCount = i.Reviews.Count,
                    CommentCount = i.Reviews.Sum(r => r.Comments.Count)
                })
                .OrderByDescending(i => i.ReviewCount)
                .ThenByDescending(i => i.AverageRating)
                .Take(10)
                .ToListAsync();
        }

        public async Task<List<FeedReviewViewModel>> GetCommunityReviewsAsync(int? categoryId, int page, int pageSize, string currentUserId)
        {
            var source = _db.Reviews.AsNoTracking().AsQueryable();
            if (categoryId.HasValue)
                source = source.Where(r => r.Item.CategoryId == categoryId.Value);

            return await source
                .Select(r => new FeedReviewViewModel
                {
                    Id = r.Id,
                    UserId = r.UserId,
                    UserDisplayName = r.User.DisplayName,
                    CreatedAt = r.CreatedAt,
                    CategoryName = r.Item.Category.Name,
                    Rating = r.Rating,
                    ItemId = r.ItemId,
                    ItemName = r.Item.Name,
                    Title = r.Title,
                    Body = r.Body,
                    LikeCount = r.Likes.Count,
                    CommentCount = r.Comments.Count,
                    IsLikedByCurrentUser = r.Likes.Any(l => l.UserId == currentUserId),
                    FavoriteCount = r.Favorites.Count,
                    IsFavoritedByCurrentUser = r.Favorites.Any(f => f.UserId == currentUserId)
                })
                .OrderByDescending(r => r.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<int> GetReviewCountAsync(int? categoryId)
        {
            var query = _db.Reviews.AsNoTracking().AsQueryable();
            if (categoryId.HasValue)
                query = query.Where(r => r.Item.CategoryId == categoryId.Value);
            return await query.CountAsync();
        }

        public async Task<ItemDetailViewModel?> GetItemDetailsAsync(int itemId, string currentUserId)
        {
            var item = await _db.Items
                .AsNoTracking()
                .Where(i => i.Id == itemId)
                .Select(i => new
                {
                    i.Id,
                    i.Name,
                    CategoryName = i.Category.Name,
                    AverageRating = i.Reviews.Any() ? i.Reviews.Average(r => r.Rating) : 0m,
                    ReviewCount = i.Reviews.Count,
                    CommentCount = i.Reviews.Sum(r => r.Comments.Count)
                })
                .FirstOrDefaultAsync();

            if (item == null) return null;

            var reviews = await _db.Reviews
                .AsNoTracking()
                .Where(r => r.ItemId == itemId)
                .Select(r => new FeedReviewViewModel
                {
                    Id = r.Id,
                    UserId = r.UserId,
                    UserDisplayName = r.User.DisplayName,
                    CreatedAt = r.CreatedAt,
                    CategoryName = r.Item.Category.Name,
                    Rating = r.Rating,
                    ItemId = r.ItemId,
                    ItemName = r.Item.Name,
                    Title = r.Title,
                    Body = r.Body,
                    LikeCount = r.Likes.Count,
                    CommentCount = r.Comments.Count,
                    IsLikedByCurrentUser = r.Likes.Any(l => l.UserId == currentUserId),
                    FavoriteCount = r.Favorites.Count,
                    IsFavoritedByCurrentUser = r.Favorites.Any(f => f.UserId == currentUserId)
                })
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();

            return new ItemDetailViewModel
            {
                Id = item.Id,
                Name = item.Name,
                CategoryName = item.CategoryName,
                AverageRating = item.AverageRating,
                ReviewCount = item.ReviewCount,
                CommentCount = item.CommentCount,
                Reviews = reviews
            };
        }
    }
}
