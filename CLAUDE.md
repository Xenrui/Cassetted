# CLAUDE.md

Convention and architecture reference for Cassetted. Specifics (route names,
service method signatures, CSS classes) live in the code — when in doubt, read
the file.

## Project Overview

**Cassetted** is a Letterboxd-style social rating app (music, movies, games,
etc.) built on ASP.NET Core MVC with SQL Server and EF Core. Users add items
to fixed categories, write reviews with ratings, follow each other, and browse
a social activity feed.

## Commands

```bash
dotnet run                                    # development
dotnet watch run                              # hot reload
dotnet build
dotnet ef migrations add <DescriptiveName>    # after any model/DbContext change
dotnet ef database update
dotnet ef migrations remove                   # undo last unapplied migration
```

Default URL: `https://localhost:7164`. No test project.

## Stack

.NET 10 · ASP.NET Core MVC · EF Core · SQL Server (SQLEXPRESS) · ASP.NET Core
Identity (Razor Pages for Login/Register, MVC for everything else).

## Domain Model

| Entity | Purpose |
|--------|---------|
| `Category` | Fixed system categories (Movies, TV Shows, Music, Books, Anime, Games) — seeded in `ApplicationDbContext`, never user-created |
| `Item` | Things being rated within a category; created by users on the fly when writing the first review |
| `Review` | One review per user per item; `Rating` is `decimal(3,1)` [0.5–5.0]; unique index `{UserId, ItemId}`; has `IsFavorited` flag |
| `ReviewLike` | Composite PK `{UserId, ReviewId}` |
| `Comment` | Belongs to a Review |
| `UserFollow` | Composite PK `{FollowerId, FollowedId}`; both FKs `Restrict` to avoid multi-cascade paths |

`ApplicationUser` extends `IdentityUser` with `DisplayName`, `Bio`, `JoinedAt`.

## Folder Conventions

```
Controllers/         HTTP entry points only — no EF, no business logic
Services/            All DbContext access and business logic
Models/              Domain entities
Models/ViewModels/   ViewModels passed to views (namespace Cassetted.Models.ViewModels)
Views/{Controller}/  Razor views named after actions
Views/Shared/        Layouts and partials (_Layout, _LoginPartial, _ReviewModal)
Data/                ApplicationDbContext
Areas/Identity/      Scaffolded Identity pages (Login + Register customised)
wwwroot/css/         site.css (global) + per-feature files loaded via @section Styles
wwwroot/js/          site.js (global)
wwwroot/images/      Static images
```

## Services Layer

All DB access lives in `Services/`. Each service is a plain class injected via
constructor and registered as `AddScoped<T>()` in `Program.cs`. Controllers
hold no `ApplicationDbContext`. For current method signatures, read the file —
they evolve.

## Layout System

One shared layout: `Views/Shared/_Layout.cshtml`. The layout sets
`d-flex flex-column min-vh-100` on `<body>` and renders the body inside
`<main class="flex-grow-1">`. It does *not* wrap `@RenderBody()` in a
container — each view is responsible for its own page-level layout.

**Nav dropdown**: `_Layout.cshtml` injects `SignInManager<ApplicationUser>` and
reads `ClaimTypes.NameIdentifier` for the current user ID. Authenticated users
see a Bootstrap dropdown with Profile, Settings, and Sign Out links.

**CSS files** — `site.css` loads globally. Page-specific files load via the
`Styles` section:

```html
@section Styles {
    <link rel="stylesheet" href="~/css/feed.css" asp-append-version="true" />
}
```

| File | Scope |
|------|-------|
| `site.css` | Global base styles, navbar (`c-nav`), footer, shared components (`c-review-card`, `c-badge`, `c-stars`, `c-avatar`, `c-action-btn`, `c-like-btn--liked`) |
| `landing.css` | Landing page only |
| `auth.css` | Login and register pages (uses `auth-` prefix, not `c-`) |
| `feed.css` | Feed page + **review modal styles** — also load on any page that renders `<partial name="_ReviewModal" />` |
| `browse.css` | Browse index, Explore, Browse/Item |
| `library.css` | Library page |
| `profile.css` | Profile page |
| `settings.css` | Settings and Change Password |
| `review.css` | Review detail and Edit |

**`ViewData` conventions used by the layout:**

| Key | Effect |
|-----|--------|
| `ViewData["BodyClass"]` | Added to `<body>` — use `"cassetted-landing"` for the dark teal background |
| `ViewData["ActiveNav"]` | Highlights the matching nav link — values: `"Feed"`, `"Browse"`, `"Library"` |

**`_ViewImports.cshtml`** globally imports `Cassetted.Models` and
`Cassetted.Models.ViewModels`, so views don't need explicit `@using`.

## Review Modal

`Views/Shared/_ReviewModal.cshtml` is a self-contained Bootstrap modal. It
POSTs to `/Review/Create` via `fetch` and returns to the current page on
success. Any page that renders it must also load `feed.css` (which contains
all `.c-review-modal__*` styles). The modal resets itself on close.

## Card Click Navigation

`wwwroot/js/site.js` has a global delegated click handler that navigates to
`data-href` on any element with that attribute. It skips clicks on
`a, button, input, textarea, form` so links, buttons, and dropdowns inside
cards still work normally.

## Identity & Auth

- Login uses **email + password** (`SignInManager.PasswordSignInAsync` with
  the email as the username).
- Registration captures `DisplayName` (shown as "Username" in the UI) — stored
  on `ApplicationUser.DisplayName`; `UserName` is set to the email.
- Login and Register are scaffolded Razor Pages under
  `Areas/Identity/Pages/Account/` — not MVC controllers.
- Settings and Change Password are **custom MVC actions** on
  `ProfileController`, not scaffolded Identity Manage pages.
- Password policy: minimum 6 characters, no other requirements
  (`RequireConfirmedAccount = false`).

## Key DbContext Details (`Data/ApplicationDbContext.cs`)

All FK behaviour is set explicitly in `OnModelCreating` — do not rely on EF
conventions for cascade/restrict. Composite keys (`ReviewLike`, `UserFollow`)
are configured with `HasKey`. The 6 fixed categories are seeded via `HasData`.
Check existing patterns before adding new relationships.

---

## Rules

Tag-prefixed, scannable. Add new ones in the same format.

- [STYLE] Never add emojis to commit messages or code comments — project convention.
- [STYLE] Use kebab-case for static asset filenames (`site.css`, `logo.png`) and CSS class names. C# and Razor files follow standard PascalCase conventions (`HomeController.cs`, `Index.cshtml`).
- [STYLE] CSS uses the `c-` BEM prefix for Cassetted UI components (`c-hero__title`) and `auth-` prefix for auth page components. Do not introduce a third naming system.
- [STYLE] Bootstrap-first: use Bootstrap utility classes for layout, spacing, alignment, and text behaviour (`d-flex`, `gap-*`, `align-items-*`, `justify-content-*`, `mb-*`, `text-end`, `text-truncate`, `rounded-pill`, etc.). Write custom CSS only for properties Bootstrap can't express: brand colors, custom backgrounds, animations, non-standard sizing, and component-specific visual overrides.

- [ARCH] Controllers handle HTTP only: model binding, calling a service, returning a view or redirect. No EF queries, no business logic.
- [ARCH] All database access belongs in a service class in `Services/`. Never inject `ApplicationDbContext` directly into a controller.
- [ARCH] Always pass a ViewModel to views — never pass a raw domain model. ViewModels live in `Models/ViewModels/` with namespace `Cassetted.Models.ViewModels`.
- [ARCH] Views must contain zero business logic. Computed values (badge slugs, percentages, rating-star counts, etc.) belong in services or as get-only properties on the ViewModel.
- [ARCH] Use `Areas/` only for distinct, self-contained feature groups (e.g., Identity). Standard features go in the root `Controllers/` and `Views/` folders.
- [ARCH] Partial views go in `Views/Shared/` and are prefixed with `_`. Use them for any repeated UI fragment and for self-contained modals.
- [ARCH] Modals use Bootstrap's `modal` component for backdrop and animation. Override visual styles (background, border-radius, colors) with custom CSS. The modal partial is rendered with `<partial name="_ModalName" />` at the bottom of the view that owns it. Always load `feed.css` on any page that uses `_ReviewModal`.
- [ARCH] Categories are fixed system data (IDs 1–6). Never create UI or logic that allows adding, editing, or deleting categories.

- [DB] Use async EF Core methods exclusively (`ToListAsync`, `FirstOrDefaultAsync`, `SaveChangesAsync`, etc.).
- [DB] Add `.AsNoTracking()` to any read-only query that projects to a ViewModel (which is all of them). Writes (`Add`/`Update`/`Remove`) need tracking and must not use `AsNoTracking`.
- [DB] Push pagination, filtering, grouping, and aggregation to the database. Don't `ToListAsync()` first and then `.Take(n)` / `.Where(...)` / `.GroupBy(...)` in memory.
- [DB] Add a migration immediately after any model or `OnModelCreating` change. Name migrations descriptively (`AddReviewBodyIndex`, not `Update2`).
- [DB] Never modify an already-applied migration. Create a new one instead.
- [DB] When adding a relationship, explicitly set both the cascade behaviour and the FK restriction in `OnModelCreating` — match the existing Restrict/Cascade patterns in `ApplicationDbContext`.

- [AUTH] Get the current user ID with `_userManager.GetUserId(User)` and guard `if (userId == null) return Challenge();`. Use `_userManager.GetUserAsync(User)` only when you also need `DisplayName` or `Email`. Don't suppress nullability with `!` — `[Authorize]` makes it non-null at runtime, but the guard documents the contract.
