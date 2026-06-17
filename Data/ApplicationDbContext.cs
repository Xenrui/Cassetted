using Cassetted.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Cassetted.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<ReviewLike> ReviewLikes { get; set; }
        public DbSet<ReviewFavorite> ReviewFavorites { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<UserFollow> UserFollows { get; set; }

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

            // ReviewFavorite — composite PK; User Restrict, Review Cascade
            // (mirrors ReviewLike to avoid multi-cascade-path errors)
            builder.Entity<ReviewFavorite>()
                .HasKey(rf => new { rf.UserId, rf.ReviewId });

            builder.Entity<ReviewFavorite>()
                .HasOne(rf => rf.User)
                .WithMany(u => u.ReviewFavorites)
                .HasForeignKey(rf => rf.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<ReviewFavorite>()
                .HasOne(rf => rf.Review)
                .WithMany(r => r.Favorites)
                .HasForeignKey(rf => rf.ReviewId)
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

            // Categories are fixed — seeded at startup, not user-created
            builder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Movies" },
                new Category { Id = 2, Name = "TV Shows" },
                new Category { Id = 3, Name = "Music" },
                new Category { Id = 4, Name = "Books" },
                new Category { Id = 5, Name = "Anime" },
                new Category { Id = 6, Name = "Games" }
            );
        }
    }
}
