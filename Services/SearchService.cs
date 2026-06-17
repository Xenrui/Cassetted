using Cassetted.Data;
using Cassetted.Models.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace Cassetted.Services
{
    public class SearchService
    {
        private readonly ApplicationDbContext _db;

        public SearchService(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<List<ItemSuggestionViewModel>> SearchItemsForCategoryAsync(
            int categoryId, string q, int limit = 8)
        {
            var needle = (q ?? string.Empty).Trim();
            if (needle.Length < 2) return new();

            var pattern = $"%{needle}%";

            return await _db.Items
                .AsNoTracking()
                .Where(i => i.CategoryId == categoryId && EF.Functions.Like(i.Name, pattern))
                .OrderByDescending(i => i.Reviews.Count)
                .ThenBy(i => i.Name)
                .Take(limit)
                .Select(i => new ItemSuggestionViewModel
                {
                    Id = i.Id,
                    Name = i.Name,
                    ReviewCount = i.Reviews.Count,
                    AverageRating = i.Reviews.Any() ? i.Reviews.Average(r => r.Rating) : 0m
                })
                .ToListAsync();
        }

        public async Task<SearchViewModel> SearchAsync(string? q, int limit = 20)
        {
            var vm = new SearchViewModel { Query = q ?? string.Empty };
            var needle = (q ?? string.Empty).Trim();
            if (needle.Length < 2) return vm;

            var pattern = $"%{needle}%";

            vm.Items = await _db.Items
                .AsNoTracking()
                .Where(i => EF.Functions.Like(i.Name, pattern))
                .OrderByDescending(i => i.Reviews.Count)
                .ThenBy(i => i.Name)
                .Take(limit)
                .Select(i => new SearchItemResult
                {
                    Id = i.Id,
                    Name = i.Name,
                    CategoryName = i.Category.Name,
                    AverageRating = i.Reviews.Any() ? i.Reviews.Average(r => r.Rating) : 0m,
                    ReviewCount = i.Reviews.Count
                })
                .ToListAsync();

            vm.Users = await _db.Users
                .AsNoTracking()
                .Where(u => EF.Functions.Like(u.DisplayName, pattern))
                .OrderByDescending(u => u.Reviews.Count)
                .ThenBy(u => u.DisplayName)
                .Take(limit)
                .Select(u => new SearchUserResult
                {
                    UserId = u.Id,
                    DisplayName = u.DisplayName,
                    ReviewCount = u.Reviews.Count
                })
                .ToListAsync();

            return vm;
        }
    }
}
