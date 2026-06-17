using Microsoft.AspNetCore.Identity;

namespace Cassetted.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string DisplayName { get; set; } = string.Empty;
        public string? Bio { get; set; }
        public DateTime JoinedAt { get; set; } = DateTime.UtcNow;

        public ICollection<Review> Reviews { get; set; } = new List<Review>();
        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
        public ICollection<ReviewLike> ReviewLikes { get; set; } = new List<ReviewLike>();
        public ICollection<ReviewFavorite> ReviewFavorites { get; set; } = new List<ReviewFavorite>();
        public ICollection<UserFollow> Followers { get; set; } = new List<UserFollow>();
        public ICollection<UserFollow> Following { get; set; } = new List<UserFollow>();
        public ICollection<Item> Items { get; set; } = new List<Item>();
    }
}
