using Cassetted.Data;
using Cassetted.Models;
using Cassetted.Models.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace Cassetted.Services
{
    public class ProfileService
    {
        private readonly ApplicationDbContext _db;

        public ProfileService(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<ProfileViewModel?> GetProfileAsync(string profileUserId, string? currentUserId)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == profileUserId);
            if (user == null) return null;

            var reviews = await _db.Reviews
                .Where(r => r.UserId == profileUserId)
                .Select(r => new
                {
                    r.Rating,
                    CategoryName = r.Item.Category.Name,
                    ItemName = r.Item.Name
                })
                .ToListAsync();

            var followerCount = await _db.UserFollows.CountAsync(uf => uf.FollowedId == profileUserId);
            var followingCount = await _db.UserFollows.CountAsync(uf => uf.FollowerId == profileUserId);

            bool isFollowing = currentUserId != null && currentUserId != profileUserId &&
                await _db.UserFollows.AnyAsync(uf => uf.FollowerId == currentUserId && uf.FollowedId == profileUserId);

            decimal avgRating = reviews.Count > 0
                ? Math.Round(reviews.Average(r => r.Rating), 1)
                : 0m;

            var categoryGroups = reviews
                .GroupBy(r => r.CategoryName)
                .Select(g => new CategoryActivityViewModel
                {
                    CategoryName = g.Key,
                    ReviewCount = g.Count()
                })
                .OrderByDescending(c => c.ReviewCount)
                .ToList();

            var mostReviewedCategory = categoryGroups.FirstOrDefault()?.CategoryName;

            var highestRatedItem = reviews.Count > 0
                ? reviews.OrderByDescending(r => r.Rating).First().ItemName
                : null;

            return new ProfileViewModel
            {
                UserId = profileUserId,
                DisplayName = user.DisplayName,
                Bio = user.Bio,
                ReviewCount = reviews.Count,
                FollowerCount = followerCount,
                FollowingCount = followingCount,
                AverageRating = avgRating,
                MostReviewedCategory = mostReviewedCategory,
                HighestRatedItemName = highestRatedItem,
                CategoryActivity = categoryGroups,
                IsOwnProfile = currentUserId == profileUserId,
                IsFollowing = isFollowing
            };
        }

        public async Task FollowAsync(string followerId, string followedId)
        {
            var exists = await _db.UserFollows
                .AnyAsync(uf => uf.FollowerId == followerId && uf.FollowedId == followedId);
            if (exists) return;

            _db.UserFollows.Add(new UserFollow
            {
                FollowerId = followerId,
                FollowedId = followedId,
                CreatedAt = DateTime.UtcNow
            });
            await _db.SaveChangesAsync();
        }

        public async Task UnfollowAsync(string followerId, string followedId)
        {
            var follow = await _db.UserFollows
                .FirstOrDefaultAsync(uf => uf.FollowerId == followerId && uf.FollowedId == followedId);
            if (follow == null) return;

            _db.UserFollows.Remove(follow);
            await _db.SaveChangesAsync();
        }
    }
}
