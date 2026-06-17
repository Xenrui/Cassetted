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
                Body = input.Body.Trim()
            });
            await _db.SaveChangesAsync();

            return (true, null);
        }
    }
}
