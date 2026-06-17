using Cassetted.Data;
using Cassetted.Models;
using Cassetted.Models.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace Cassetted.Services
{
    public class ReviewService
    {
        private readonly ApplicationDbContext _db;

        public ReviewService(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<ReviewDetailViewModel?> GetReviewDetailsAsync(int reviewId, string currentUserId)
        {
            return await _db.Reviews
                .AsNoTracking()
                .Where(r => r.Id == reviewId)
                .Select(r => new ReviewDetailViewModel
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
                    IsLikedByCurrentUser = r.Likes.Any(l => l.UserId == currentUserId),
                    FavoriteCount = r.Favorites.Count,
                    IsFavoritedByCurrentUser = r.Favorites.Any(f => f.UserId == currentUserId),
                    IsOwnReview = r.UserId == currentUserId,
                    Comments = r.Comments
                        .OrderBy(c => c.CreatedAt)
                        .Select(c => new CommentViewModel
                        {
                            Id = c.Id,
                            UserId = c.UserId,
                            UserDisplayName = c.User.DisplayName,
                            CreatedAt = c.CreatedAt,
                            Body = c.Body
                        })
                        .ToList()
                })
                .FirstOrDefaultAsync();
        }

        public async Task AddCommentAsync(int reviewId, string userId, string body)
        {
            _db.Comments.Add(new Comment
            {
                ReviewId = reviewId,
                UserId = userId,
                Body = body.Trim()
            });
            await _db.SaveChangesAsync();
        }

        public async Task<(bool Liked, int LikeCount)> ToggleLikeAsync(int reviewId, string userId)
        {
            var like = await _db.ReviewLikes
                .FirstOrDefaultAsync(l => l.ReviewId == reviewId && l.UserId == userId);

            if (like != null)
                _db.ReviewLikes.Remove(like);
            else
                _db.ReviewLikes.Add(new ReviewLike { ReviewId = reviewId, UserId = userId });

            await _db.SaveChangesAsync();

            var likeCount = await _db.ReviewLikes.CountAsync(l => l.ReviewId == reviewId);
            return (like == null, likeCount);
        }

        public async Task<(bool Favorited, int FavoriteCount)> ToggleFavoriteAsync(int reviewId, string userId)
        {
            var fav = await _db.ReviewFavorites
                .FirstOrDefaultAsync(f => f.ReviewId == reviewId && f.UserId == userId);

            if (fav != null)
                _db.ReviewFavorites.Remove(fav);
            else
                _db.ReviewFavorites.Add(new ReviewFavorite { ReviewId = reviewId, UserId = userId });

            await _db.SaveChangesAsync();

            var favoriteCount = await _db.ReviewFavorites.CountAsync(f => f.ReviewId == reviewId);
            return (fav == null, favoriteCount);
        }

        public async Task<EditReviewViewModel?> GetReviewForEditAsync(int reviewId, string userId)
        {
            return await _db.Reviews
                .AsNoTracking()
                .Where(r => r.Id == reviewId && r.UserId == userId)
                .Select(r => new EditReviewViewModel
                {
                    ReviewId = r.Id,
                    ItemName = r.Item.Name,
                    CategoryName = r.Item.Category.Name,
                    Input = new EditReviewInputModel
                    {
                        Rating = (int)r.Rating,
                        Body = r.Body
                    }
                })
                .FirstOrDefaultAsync();
        }

        public async Task<bool> UpdateReviewAsync(int reviewId, string userId, EditReviewInputModel input)
        {
            var review = await _db.Reviews.FirstOrDefaultAsync(r => r.Id == reviewId && r.UserId == userId);
            if (review == null) return false;

            review.Rating = input.Rating;
            review.Body = input.Body.Trim();
            review.UpdatedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteReviewAsync(int reviewId, string userId)
        {
            var review = await _db.Reviews.FirstOrDefaultAsync(r => r.Id == reviewId && r.UserId == userId);
            if (review == null) return false;

            _db.Reviews.Remove(review);
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<(bool Success, string? Error)> CreateReviewAsync(string userId, CreateReviewInputModel input)
        {
            Item? item;
            if (input.ItemId.HasValue)
            {
                item = await _db.Items.FirstOrDefaultAsync(i =>
                    i.Id == input.ItemId.Value && i.CategoryId == input.CategoryId);
                if (item == null)
                    return (false, "Selected item is no longer available.");
            }
            else
            {
                var itemName = input.ItemName.Trim();
                item = await _db.Items.FirstOrDefaultAsync(i =>
                    i.CategoryId == input.CategoryId && i.Name == itemName);

                if (item == null)
                {
                    item = new Item
                    {
                        Name = itemName,
                        CategoryId = input.CategoryId,
                        CreatedByUserId = userId
                    };
                    _db.Items.Add(item);
                    await _db.SaveChangesAsync();
                }
            }

            var alreadyReviewed = await _db.Reviews.AnyAsync(r => r.UserId == userId && r.ItemId == item.Id);
            if (alreadyReviewed)
                return (false, "You have already reviewed this item.");

            _db.Reviews.Add(new Review
            {
                UserId = userId,
                ItemId = item.Id,
                Rating = input.Rating,
                Body = input.Body.Trim()
            });
            await _db.SaveChangesAsync();

            return (true, null);
        }
    }
}
