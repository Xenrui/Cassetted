# Cassetted

A social media rating app. Track, rate, and review the things you watch, read, play, and listen to — and see what your friends think.

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

## Contributing

### Step 1: Fork & Clone the Repository

1. Click the **Fork** button at the top right of this repository.
2. Clone your forked repository to your local machine:

```bash
git clone https://github.com/<your-username>/Cassetted.git
cd Cassetted
```

### Step 2: Install the .NET 10 SDK

Download and install from [dotnet.microsoft.com](https://dotnet.microsoft.com/download).
Verify the install:

```bash
dotnet --list-sdks
```

You need a `10.0.x` SDK in the list.

> *Error if missing:* `The current .NET SDK does not support targeting .NET 10.0`.

### Step 3: Install SQL Server Express

1. Install [SQL Server Express](https://www.microsoft.com/sql-server/sql-server-downloads)
   with the **default instance name `SQLEXPRESS`**.
2. Make sure the SQL Server service is running (Services app → `SQL Server (SQLEXPRESS)` → *Running*).

The committed connection string in `appsettings.json` points at `localhost\SQLEXPRESS`
using Windows authentication — no username/password needed.

> *Error if missing/misnamed:* `A network-related or instance-specific error occurred while establishing a connection to SQL Server` (error 26 / 40).

### Step 4: Install the EF Core CLI Tool

The `dotnet ef` command is not bundled with the SDK — install it globally once
per machine:

```bash
dotnet tool install --global dotnet-ef
```

If you already have an older version:

```bash
dotnet tool update --global dotnet-ef
```

> *Error if missing:* `Could not execute because the specified command or file was not found... dotnet-ef`.

### Step 5: Configure Local Settings (Optional)

The committed `appsettings.json` assumes `localhost\SQLEXPRESS` with Windows
auth, which works out of the box for most Windows setups. **Only do this step
if your SQL Server instance has a different name, or you need to point at a
different database.**

1. In the project root, you'll see a file named `appsettings.Development.json`.
   This file overrides `appsettings.json` when running locally, and it's
   already in the repo — open it directly, no copy needed.

**Windows (PowerShell):**

```powershell
notepad appsettings.Development.json
```

2. Add a `ConnectionStrings` section that overrides the default:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost\\YOUR_INSTANCE;Database=CassettedDb;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  }
}
```

Replace `YOUR_INSTANCE` with your actual SQL Server instance name (e.g.
`MSSQLSERVER01`). Do **not** edit `appsettings.json` directly — that file is
shared with the team.

### Step 6: Trust the HTTPS Development Certificate

Only needed once per machine:

```bash
dotnet dev-certs https --trust
```

> *Error if missing:* browser warning `NET::ERR_CERT_AUTHORITY_INVALID` on `https://localhost:7164`.

### Step 7: Restore, Migrate, and Run

```bash
dotnet restore
dotnet ef database update
dotnet watch run
```

The app will be available at `https://localhost:7164`. The six categories
(Movies, TV Shows, Music, Books, Anime, Games) are seeded by the initial
migration, and `DevDataSeeder` populates sample users and reviews on first
run in Development.

### Conventions

Before opening a PR, skim `CLAUDE.md` — it documents the architectural rules
(controllers stay thin, all DB access in `Services/`, ViewModels in
`Models/ViewModels/`, BEM `c-` CSS prefix, migration naming, etc.).

