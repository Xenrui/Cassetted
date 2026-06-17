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

        public async Task<List<BrowseItemViewModel>> GetPopularItemsAsync(int? categoryId)
        {
            return await _db.Items
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

        public async Task<List<FeedReviewViewModel>> GetCommunityReviewsAsync(int? categoryId, int page, int pageSize)
        {
            var source = _db.Reviews.AsQueryable();
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
                    IsLikedByCurrentUser = false
                })
                .OrderByDescending(r => r.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<int> GetReviewCountAsync(int? categoryId)
        {
            var query = _db.Reviews.AsQueryable();
            if (categoryId.HasValue)
                query = query.Where(r => r.Item.CategoryId == categoryId.Value);
            return await query.CountAsync();
        }
    }
}
