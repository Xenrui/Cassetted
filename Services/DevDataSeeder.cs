using Cassetted.Data;
using Cassetted.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Cassetted.Services
{
    public class DevDataSeeder
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;

        public DevDataSeeder(ApplicationDbContext db, UserManager<ApplicationUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        public async Task SeedAsync()
        {
            if (await _db.Users.AnyAsync()) return;

            var now = DateTime.UtcNow;
            var rng = new Random(42);

            var userSeeds = new (string DisplayName, string Email, string Bio)[]
            {
                ("aria",       "aria@gmail.com",  "tv nerd, occasional crier at anime endings"),
                ("benji",      "benji@gmail.com", "books over everything"),
                ("cleo",       "cleo@gmail.com",  "movies, music, and long walks"),
                ("dax",        "dax@gmail.com",   "gamer, mostly soulslikes"),
                ("evie",       "evie@gmail.com",  "MOBA enjoyer, sorry"),
                ("finn",       "finn@gmail.com",  "indie everything"),
                ("gigi",       "gigi@gmail.com",  "self-improvement bookshelf"),
                ("hugo",       "hugo@gmail.com",  "rateboxd refugee"),
                ("ivy",        "ivy@gmail.com",   "anime > sleep"),
                ("juno",       "juno@gmail.com",  "i log everything"),
            };

            var users = new List<ApplicationUser>();
            foreach (var (display, email, bio) in userSeeds)
            {
                var user = new ApplicationUser
                {
                    UserName = email,
                    Email = email,
                    EmailConfirmed = true,
                    DisplayName = display,
                    Bio = bio,
                    JoinedAt = now.AddDays(-rng.Next(30, 365)),
                };
                var result = await _userManager.CreateAsync(user, "pass123");
                if (!result.Succeeded)
                {
                    throw new InvalidOperationException(
                        "DevDataSeeder failed to create user " + email + ": " +
                        string.Join("; ", result.Errors.Select(e => e.Code + " " + e.Description)));
                }
                users.Add(user);
            }

            var creatorId = users[0].Id;

            var itemSeeds = new (string Name, int CategoryId, string? Description)[]
            {
                ("Inception",              1, "Heist movie inside dreams."),
                ("Interstellar",           1, "Wormhole love story with corn."),
                ("Arcane",                 2, "Riot's animated Piltover/Zaun series."),
                ("Breaking Bad",           2, "Chemistry teacher takes a wrong turn."),
                ("To Pimp a Butterfly",    3, "Kendrick Lamar's 2015 jazz-rap landmark."),
                ("OK Computer",            3, "Radiohead, end-of-century dread."),
                ("Atomic Habits",          4, "James Clear on small changes, big results."),
                ("Project Hail Mary",      4, "Andy Weir, one man and a tardigrade alien."),
                ("Frieren",                5, "Elf wizard reckons with a short human life."),
                ("Vinland Saga",           5, "Vikings, vengeance, and farming arcs."),
                ("Elden Ring",             6, "FromSoftware's open-world soulslike."),
                ("League of Legends",      6, "MOBA. You know the one."),
                ("Hades",                  6, "Roguelike escape from the Greek underworld."),
            };

            var items = itemSeeds.Select((s, idx) => new Item
            {
                Name = s.Name,
                CategoryId = s.CategoryId,
                Description = s.Description,
                CreatedByUserId = creatorId,
                CreatedAt = now.AddDays(-rng.Next(20, 200)),
            }).ToList();

            _db.Items.AddRange(items);
            await _db.SaveChangesAsync();

            var reviewBodies = new[]
            {
                "Wholly absorbing from start to finish — kept thinking about it for days.",
                "Solid craft, but the middle drags and the ending leans on a trope I'm tired of.",
                "Best thing I've touched this year. Not even close.",
                "Loved the vibe but the writing is uneven. Still recommend.",
                "Started slow, hooked me by the halfway point.",
                "Overrated, fight me.",
                "Underrated, fight me about this one too.",
                "A perfect comfort pick — I'll come back to this.",
                "Technically impressive, emotionally a bit hollow.",
                "Everything clicked. Pacing, tone, payoff — all dialed in.",
            };

            var reviewTitles = new string?[]
            {
                null, null, null,
                "stuck with me",
                "almost great",
                "actually incredible",
                "a comfort pick",
                "the highs are high",
            };

            var allRatings = new[] { 2.0m, 2.5m, 3.0m, 3.5m, 3.5m, 4.0m, 4.0m, 4.0m, 4.5m, 4.5m, 5.0m };

            var reviews = new List<Review>();
            foreach (var user in users)
            {
                var shuffled = items.OrderBy(_ => rng.Next()).ToList();
                var count = rng.Next(5, 11);
                foreach (var item in shuffled.Take(count))
                {
                    reviews.Add(new Review
                    {
                        UserId = user.Id,
                        ItemId = item.Id,
                        Rating = allRatings[rng.Next(allRatings.Length)],
                        Title = reviewTitles[rng.Next(reviewTitles.Length)],
                        Body = reviewBodies[rng.Next(reviewBodies.Length)],
                        CreatedAt = now.AddDays(-rng.Next(0, 60)).AddHours(-rng.Next(0, 24)),
                        UpdatedAt = now.AddDays(-rng.Next(0, 60)),
                    });
                }
            }

            _db.Reviews.AddRange(reviews);
            await _db.SaveChangesAsync();

            var followPairs = new HashSet<(string, string)>();
            for (int i = 0; i < users.Count; i++)
            {
                var followCount = rng.Next(2, 5);
                var targets = Enumerable.Range(0, users.Count)
                    .Where(j => j != i)
                    .OrderBy(_ => rng.Next())
                    .Take(followCount);
                foreach (var j in targets)
                {
                    followPairs.Add((users[i].Id, users[j].Id));
                }
            }

            _db.UserFollows.AddRange(followPairs.Select(p => new UserFollow
            {
                FollowerId = p.Item1,
                FollowedId = p.Item2,
                CreatedAt = now.AddDays(-rng.Next(0, 90)),
            }));
            await _db.SaveChangesAsync();

            var likePairs = new HashSet<(string, int)>();
            var reviewIds = reviews.Select(r => r.Id).ToList();
            while (likePairs.Count < 25)
            {
                var user = users[rng.Next(users.Count)];
                var reviewId = reviewIds[rng.Next(reviewIds.Count)];
                likePairs.Add((user.Id, reviewId));
            }

            _db.ReviewLikes.AddRange(likePairs.Select(p => new ReviewLike
            {
                UserId = p.Item1,
                ReviewId = p.Item2,
                CreatedAt = now.AddDays(-rng.Next(0, 45)),
            }));
            await _db.SaveChangesAsync();
        }
    }
}
