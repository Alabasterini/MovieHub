# Project Context — MovieHub

## What are we building
A desktop movie rating application (mini Filmweb/IMDB clone). Users can browse films, read details, and leave ratings. Admins can manage the film catalog.

## Tech Stack
- **Backend:** ASP.NET Core Web API (.NET 10)
- **Frontend:** Avalonia UI (C#, XAML, cross-platform)
- **ORM:** Entity Framework Core
- **Database:** PostgreSQL (via Docker)
- **Auth:** JWT (access token only, no refresh token)
- **Validation:** FluentValidation
- **Frontend pattern:** MVVM via CommunityToolkit.Mvvm

## Solution Structure (monorepo)
```
MovieHub/
├── MovieHub.slnx
├── docker-compose.yml
└── src/
    ├── MovieHub.API/
    │   ├── Controllers/
    │   ├── Services/
    │   ├── Models/
    │   ├── DTOs/
    │   ├── Data/
    │   └── Program.cs
    └── MovieHub.Client/
        ├── Views/
        ├── ViewModels/
        ├── Services/
        ├── Assets/
        └── App.axaml
```

## Database Models (EF Core entities)

```csharp
User        { Id, Username (unique, max 50), Email (unique, max 100), PasswordHash, Role (enum: User/Admin), RegisteredAt }
Genre       { Id, Name (unique, max 50) }
Director    { Id, FirstName (max 50), LastName (max 50), Nationality (max 50) }
Movie       { Id, Title (max 150), Year, Description (max 1000), PosterUrl (nullable), GenreId (FK), DirectorId (FK) }
Rating      { Id, Value (int 1-10), Comment (max 500, nullable), CreatedAt, MovieId (FK), UserId (FK) }
```

Unique constraint: `(MovieId, UserId)` on Rating — one rating per user per movie.

## API Endpoints

| Method | Endpoint | Auth |
|--------|----------|------|
| POST | /api/auth/register | Public |
| POST | /api/auth/login | Public |
| GET | /api/movies | Public |
| GET | /api/movies/{id} | Public |
| POST | /api/movies | Admin |
| PUT | /api/movies/{id} | Admin |
| DELETE | /api/movies/{id} | Admin |
| POST | /api/movies/{id}/ratings | User |
| PUT | /api/ratings/{id} | User |
| GET | /api/genres | Public |
| POST | /api/genres | Admin |

## Backend Architecture Rules
- **No Repository Pattern** — use EF Core DbContext directly in Services
- Flow: `Controller → Service → DbContext → PostgreSQL`
- Controllers: only HTTP handling, status codes, auth checks
- Services: all business logic, DTO mapping, password hashing, average calculation
- DTOs: separate request/response classes — never expose EF entities directly
- JWT claims: UserId, Username, Role
- Admin endpoints use `[Authorize(Roles = "Admin")]`

## Frontend Architecture Rules
- Strict MVVM — no logic in Views/code-behind
- ViewModels use CommunityToolkit.Mvvm (`ObservableObject`, `[RelayCommand]`, `[ObservableProperty]`)
- All API calls go through `Services/ApiClient.cs` (typed HttpClient)
- Navigation via ViewLocator

## Key Behaviors
- GET /api/movies supports query params: `?title=&genreId=&directorId=`
- GET /api/movies/{id} returns movie details + list of ratings + calculated average
- Ratings: user can add one rating per movie, can edit their own rating
- Passwords stored as bcrypt hash

## Development Environment
- OS: Arch Linux
- IDE: Cursor
- PostgreSQL runs in Docker (docker-compose.yml in repo root)
- Avalonia renders via Skia (X11/Wayland) — no extra config needed

## Out of Scope (MVP)
- File upload for posters (URL only)
- Refresh tokens
- Pagination
- Unit tests