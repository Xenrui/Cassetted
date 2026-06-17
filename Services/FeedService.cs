using Cassetted.Data;
using Cassetted.Models;
using Cassetted.Models.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace Cassetted.Services
{
    public class FeedService
    {
        private readonly ApplicationDbContext _db;

        public FeedService(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<List<FeedReviewViewModel>> GetForYouFeedAsync(string currentUserId)
        {
            return await FeedQuery(_db.Reviews.Where(r => r.UserId != currentUserId), currentUserId);
        }

        public async Task<List<FeedReviewViewModel>> GetFriendsFeedAsync(string currentUserId)
        {
            var followedIds = _db.UserFollows
                .Where(f => f.FollowerId == currentUserId)
                .Select(f => f.FollowedId);

            return await FeedQuery(
                _db.Reviews.Where(r => followedIds.Contains(r.UserId) && r.UserId != currentUserId),
                currentUserId);
        }

        public async Task<List<TrendingItemViewModel>> GetTrendingItemsAsync()
        {
            return await _db.Items
                .Where(i => i.Reviews.Any())
                .Select(i => new TrendingItemViewModel
                {
                    Id = i.Id,
                    Name = i.Name,
                    CategoryName = i.Category.Name,
                    AverageRating = i.Reviews.Average(r => r.Rating),
                    ReviewCount = i.Reviews.Count
                })
                .OrderByDescending(i => i.ReviewCount)
                .ThenByDescending(i => i.AverageRating)
                .Take(5)
                .ToListAsync();
        }

        private static Task<List<FeedReviewViewModel>> FeedQuery(
            IQueryable<Review> source, string currentUserId)
        {
            return source.Select(r => new FeedReviewViewModel
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
                IsLikedByCurrentUser = r.Likes.Any(l => l.UserId == currentUserId)
            })
            .OrderByDescending(r => r.CreatedAt)
            .Take(50)
            .ToListAsync();
        }
    }
}
