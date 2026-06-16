using Cassetted.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Cassetted.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : IdentityDbContext<ApplicationUser>(options)
    {
        public DbSet<Category> Categories => Set<Category>();
        public DbSet<Item> Items => Set<Item>();
        public DbSet<Review> Reviews => Set<Review>();
        public DbSet<ReviewLike> ReviewLikes => Set<ReviewLike>();
        public DbSet<Comment> Comments => Set<Comment>();
        public DbSet<UserFollow> UserFollows => Set<UserFollow>();

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // ReviewLike — composite PK, cascade on both sides
            builder.Entity<ReviewLike>()
                .HasKey(rl => new { rl.UserId, rl.ReviewId });

            builder.Entity<ReviewLike>()
                .HasOne(rl => rl.User)
                .WithMany(u => u.ReviewLikes)
                .HasForeignKey(rl => rl.UserId)
                .OnDelete(DeleteBehavior.Restrict); // cascades already via User→Reviews→ReviewLikes

            builder.Entity<ReviewLike>()
                .HasOne(rl => rl.Review)
                .WithMany(r => r.Likes)
                .HasForeignKey(rl => rl.ReviewId)
                .OnDelete(DeleteBehavior.Cascade);

            // UserFollow — composite PK; both FKs Restrict to avoid SQL Server
            // "multiple cascade paths to the same table" error
            builder.Entity<UserFollow>()
                .HasKey(uf => new { uf.FollowerId, uf.FollowedId });

            builder.Entity<UserFollow>()
                .HasOne(uf => uf.Follower)
                .WithMany(u => u.Following)
                .HasForeignKey(uf => uf.FollowerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<UserFollow>()
                .HasOne(uf => uf.Followed)
                .WithMany(u => u.Followers)
                .HasForeignKey(uf => uf.FollowedId)
                .OnDelete(DeleteBehavior.Restrict);

            // Review — one review per user per item (unique index), decimal precision
            builder.Entity<Review>()
                .HasIndex(r => new { r.UserId, r.ItemId })
                .IsUnique();

            builder.Entity<Review>()
                .Property(r => r.Rating)
                .HasColumnType("decimal(3,1)");

            builder.Entity<Review>()
                .HasOne(r => r.User)
                .WithMany(u => u.Reviews)
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Review>()
                .HasOne(r => r.Item)
                .WithMany(i => i.Reviews)
                .HasForeignKey(r => r.ItemId)
                .OnDelete(DeleteBehavior.Cascade);

            // Comment — cascades through Review, not directly through User
            builder.Entity<Comment>()
                .HasOne(c => c.Review)
                .WithMany(r => r.Comments)
                .HasForeignKey(c => c.ReviewId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Comment>()
                .HasOne(c => c.User)
                .WithMany(u => u.Comments)
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Item — items outlive their creator
            builder.Entity<Item>()
                .HasOne(i => i.Category)
                .WithMany(c => c.Items)
                .HasForeignKey(i => i.CategoryId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Item>()
                .HasOne(i => i.CreatedBy)
                .WithMany(u => u.Items)
                .HasForeignKey(i => i.CreatedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Category — categories outlive their creator
            builder.Entity<Category>()
                .HasOne(cat => cat.CreatedBy)
                .WithMany(u => u.Categories)
                .HasForeignKey(cat => cat.CreatedByUserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
