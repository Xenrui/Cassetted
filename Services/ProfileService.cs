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
            var user = await _db.Users
                .AsNoTracking()
                .Where(u => u.Id == profileUserId)
                .Select(u => new { u.DisplayName, u.Bio })
                .FirstOrDefaultAsync();

            if (user == null) return null;

            var userReviews = _db.Reviews.AsNoTracking().Where(r => r.UserId == profileUserId);

            var reviewCount = await userReviews.CountAsync();

            var avgRatingNullable = await userReviews.AverageAsync(r => (decimal?)r.Rating);
            var avgRating = avgRatingNullable.HasValue ? Math.Round(avgRatingNullable.Value, 1) : 0m;

            var followerCount = await _db.UserFollows
                .AsNoTracking()
                .CountAsync(uf => uf.FollowedId == profileUserId);

            var followingCount = await _db.UserFollows
                .AsNoTracking()
                .CountAsync(uf => uf.FollowerId == profileUserId);

            var isFollowing = currentUserId != null && currentUserId != profileUserId &&
                await _db.UserFollows
                    .AsNoTracking()
                    .AnyAsync(uf => uf.FollowerId == currentUserId && uf.FollowedId == profileUserId);

            var categoryGroups = await userReviews
                .GroupBy(r => r.Item.Category.Name)
                .Select(g => new CategoryActivityViewModel
                {
                    CategoryName = g.Key,
                    ReviewCount = g.Count()
                })
                .OrderByDescending(c => c.ReviewCount)
                .ToListAsync();

            var maxActivity = categoryGroups.Count > 0 ? categoryGroups.Max(c => c.ReviewCount) : 0;
            foreach (var group in categoryGroups)
            {
                group.ActivityPercent = maxActivity > 0
                    ? (int)Math.Round((double)group.ReviewCount / maxActivity * 100)
                    : 0;
            }

            var mostReviewedCategory = categoryGroups.FirstOrDefault()?.CategoryName;

            var highestRatedItem = await userReviews
                .OrderByDescending(r => r.Rating)
                .Select(r => r.Item.Name)
                .FirstOrDefaultAsync();

            return new ProfileViewModel
            {
                UserId = profileUserId,
                DisplayName = user.DisplayName,
                Bio = user.Bio,
                ReviewCount = reviewCount,
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
