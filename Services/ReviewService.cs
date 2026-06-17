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

        public async Task<(bool Success, string? Error)> CreateReviewAsync(string userId, CreateReviewInputModel input)
        {
            var itemName = input.ItemName.Trim();

            var item = await _db.Items.FirstOrDefaultAsync(i =>
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

            var alreadyReviewed = await _db.Reviews.AnyAsync(r => r.UserId == userId && r.ItemId == item.Id);
            if (alreadyReviewed)
                return (false, "You have already reviewed this item.");

            _db.Reviews.Add(new Review
            {
                UserId = userId,
                ItemId = item.Id,
                Rating = input.Rating,
                Body = input.Body.Trim(),
                IsFavorited = input.IsFavorited
            });
            await _db.SaveChangesAsync();

            return (true, null);
        }
    }
}
