using MovieHub.API.Models;
using Microsoft.EntityFrameworkCore;
using MovieHub.API.Data;
using MovieHub.API.DTOs.Ratings;
using MovieHub.API.Exceptions;

namespace MovieHub.API.Services;

public class RatingService(AppDbContext dbContext)
{
    private readonly AppDbContext _dbContext = dbContext;


    private static RatingResponse MapToRatingResponse(Rating rating, string? username = null)
    {
        return new RatingResponse
        {
            Id = rating.Id,
            Value = rating.Value,
            Comment = rating.Comment,
            CreatedAt = rating.CreatedAt,
            UserId = rating.UserId,
            Username = username ?? rating.User?.Username ?? string.Empty
        };
    }
    public async Task<RatingResponse> CreateAsync(
        int movieId, int userId, CreateRatingRequest request,
        CancellationToken cancellationToken = default)
    {
        var movie = await _dbContext.Movies
            .Include(m => m.Ratings)
            .FirstOrDefaultAsync(m => m.Id == movieId, cancellationToken);

        if (movie is null)
        {
            throw new NotFoundException("Movie not found.");
        }

        var user = await _dbContext.Users
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

        if (user is null)
        {
            throw new NotFoundException("User not found.");
        }

        if (movie.Ratings.Any(r => r.UserId == userId))
        {
            throw new ConflictException("User already rated this movie.");
        }
        var rating = new Rating
        {
            Value = request.Value,
            Comment = request.Comment,
            CreatedAt = DateTime.UtcNow,
            UserId = userId,
            MovieId = movieId
        };
        _dbContext.Ratings.Add(rating);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return MapToRatingResponse(rating, user.Username);
    }

    public async Task<RatingResponse> UpdateAsync(
        int userId, int ratingId, UpdateRatingRequest request,
        CancellationToken cancellationToken = default)
    {
        var rating = await _dbContext.Ratings
            .Include(r => r.User)
            .FirstOrDefaultAsync(r => r.Id == ratingId, cancellationToken);

        if (rating is null)
        {
            throw new NotFoundException("Rating not found.");
        }

        if (rating.UserId != userId)
        {
            throw new ForbiddenException("You are not allowed to update this rating.");
        }

        rating.Value = request.Value;
        rating.Comment = request.Comment;

        await _dbContext.SaveChangesAsync(cancellationToken);
        return MapToRatingResponse(rating);
    }
}