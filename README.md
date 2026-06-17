# Cassetted

A Letterboxd-style social rating app. Track, rate, and review the things you watch, read, play, and listen to — and see what your friends think.

**Categories:** Movies · TV Shows · Music · Books · Anime · Games

## Features
**Reviews** — rate 1–5 with a written take; one review per user per item.

**Likes & Saves** — endorse any review with a like, or bookmark it to your Library.
Any user can save any review; counts are public.

**Comments** — discuss any review thread.

**Follow & feed** — follow other users; the Feed has *For You* and *Friends* tabs
plus a trending sidebar.

**Browse** — by category, with popular items, community reviews, and per-item pages

**Library** — your recent reviews and everything you've saved.

**Search** — typeahead inside the review modal links to existing items
 (preventing duplicate "The Dark Knight" rows), and the navbar search finds
items and users across the app.

**Profile** — stats, category activity, and follow controls.

## Tech Stack

- ASP.NET Core 10.0 MVC
- Entity Framework Core 10.0.8 + SQL Server
- ASP.NET Core Identity (email + password auth)
- Bootstrap 5

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- SQL Server Express (default instance: `localhost\SQLEXPRESS`)

## Getting Started

```bash
# 1. Apply database migrations
dotnet ef database update

# 2. Run
dotnet watch run
```

App runs at `https://localhost:7164`.

The six categories (Movies, TV Shows, Music, Books, Anime, Games) are seeded automatically by the migration.

## Project Structure

```
Controllers/         MVC controllers (thin — delegate to Services/)
Services/            Business logic and all database queries
Models/              Domain entities
Models/ViewModels/   ViewModels passed to views
Views/               Razor views
Data/                ApplicationDbContext
Areas/Identity/      Customised login and registration pages
wwwroot/             Static assets
```

