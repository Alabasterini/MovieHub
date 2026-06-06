using Microsoft.EntityFrameworkCore;
using MovieHub.API.Data;
using MovieHub.API.DTOs.Movies;
using MovieHub.API.DTOs.Ratings;
using MovieHub.API.Exceptions;
using MovieHub.API.Models;

namespace MovieHub.API.Services;

public class MovieService (AppDbContext dbContext)
{
    private readonly AppDbContext _dbContext = dbContext;

    public async Task<List<MovieSummaryResponse>> GetAllAsync(
        MovieListQueryRequest query,
        CancellationToken cancellationToken = default)
    {
        var moviesQuery = _dbContext.Movies
            .AsNoTracking()
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(query.Title))
        {
            var title = query.Title.Trim();
            moviesQuery = moviesQuery.Where(m => EF.Functions.ILike(m.Title, $"%{title}%"));
        }

        if (query.GenreId is not null)
            moviesQuery = moviesQuery.Where(m => m.GenreId == query.GenreId);

        if (query.DirectorId is not null)
            moviesQuery = moviesQuery.Where(m => m.DirectorId == query.DirectorId);

        return await moviesQuery
            .OrderBy(m => m.Title)
            .Select(m => new MovieSummaryResponse
            {
                Id = m.Id,
                Title = m.Title,
                Year = m.Year,
                PosterUrl = m.PosterUrl,
                Genre = new MovieGenreSummaryResponse
                {
                    Id = m.Genre.Id,
                    Name = m.Genre.Name
                },
                Director = new DirectorSummaryResponse
                {
                    Id = m.Director.Id,
                    FirstName = m.Director.FirstName,
                    LastName = m.Director.LastName,
                    Nationality = m.Director.Nationality
                },
                AverageRating = m.Ratings.Any()
                    ? m.Ratings.Average(r => (double)r.Value)
                    : (double?)null
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<MovieDetailResponse> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var movie = await _dbContext.Movies
            .AsNoTracking()
            .Include(m => m.Genre)
            .Include(m => m.Director)
            .Include(m => m.Ratings)
                .ThenInclude(r => r.User)
            .FirstOrDefaultAsync(m => m.Id == id, cancellationToken);

        if (movie is null)
        {
            throw new NotFoundException("Movie not found.");
        }

        var ratings = movie.Ratings
            .OrderByDescending(r => r.CreatedAt)
            .Select(MapToRatingResponse)
            .ToList();

        double? averageScore = ratings.Count == 0
            ? null
            : ratings.Average(r => (double)r.Value);

        return new MovieDetailResponse
        {
            Id = movie.Id,
            Title = movie.Title,
            Year = movie.Year,
            Description = movie.Description,
            PosterUrl = movie.PosterUrl,
            Genre = MapToGenreSummary(movie.Genre),
            Director = MapToDirectorSummary(movie.Director),
            Ratings = ratings,
            AverageScore = averageScore
        };
    }

    public async Task<MovieSummaryResponse> CreateAsync(
        CreateMovieRequest request,
        CancellationToken cancellationToken = default)
    {
        var movie = new Movie
        {
            Title = request.Title,
            Year = request.Year,
            Description = request.Description,
            PosterUrl = request.PosterUrl,
            GenreId = request.GenreId,
            DirectorId = request.DirectorId
        };

        _dbContext.Movies.Add(movie);
        await _dbContext.SaveChangesAsync(cancellationToken);

        var created = await _dbContext.Movies
            .AsNoTracking()
            .Include(m => m.Genre)
            .Include(m => m.Director)
            .FirstAsync(m => m.Id == movie.Id, cancellationToken);

        return MapToSummaryResponse(created);
    }

    public async Task<MovieSummaryResponse> UpdateAsync(
        int id,
        UpdateMovieRequest request,
        CancellationToken cancellationToken = default)
    {
        var movie = await _dbContext.Movies
            .FirstOrDefaultAsync(m => m.Id == id, cancellationToken);

        if (movie is null)
        {
            throw new NotFoundException("Movie not found.");
        }

        movie.Title = request.Title;
        movie.Year = request.Year;
        movie.Description = request.Description;
        movie.PosterUrl = request.PosterUrl;
        movie.GenreId = request.GenreId;
        movie.DirectorId = request.DirectorId;

        await _dbContext.SaveChangesAsync(cancellationToken);

        var updated = await _dbContext.Movies
            .AsNoTracking()
            .Include(m => m.Genre)
            .Include(m => m.Director)
            .FirstAsync(m => m.Id == id, cancellationToken);

        return MapToSummaryResponse(updated);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var movie = await _dbContext.Movies
            .FirstOrDefaultAsync(m => m.Id == id, cancellationToken);

        if (movie is null)
        {
            throw new NotFoundException("Movie not found.");
        }

        _dbContext.Movies.Remove(movie);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    private static MovieSummaryResponse MapToSummaryResponse(Movie movie)
    {
        return new MovieSummaryResponse
        {
            Id = movie.Id,
            Title = movie.Title,
            Year = movie.Year,
            PosterUrl = movie.PosterUrl,
            Genre = MapToGenreSummary(movie.Genre),
            Director = MapToDirectorSummary(movie.Director),
            AverageRating = null
        };
    }

    private static MovieGenreSummaryResponse MapToGenreSummary(Genre genre)
    {
        return new MovieGenreSummaryResponse
        {
            Id = genre.Id,
            Name = genre.Name
        };
    }

    private static DirectorSummaryResponse MapToDirectorSummary(Director director)
    {
        return new DirectorSummaryResponse
        {
            Id = director.Id,
            FirstName = director.FirstName,
            LastName = director.LastName,
            Nationality = director.Nationality
        };
    }

    private static RatingResponse MapToRatingResponse(Rating rating)
    {
        return new RatingResponse
        {
            Id = rating.Id,
            Value = rating.Value,
            Comment = rating.Comment,
            CreatedAt = rating.CreatedAt,
            UserId = rating.UserId,
            Username = rating.User.Username
        };
    }
}

