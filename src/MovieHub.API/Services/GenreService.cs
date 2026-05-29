using Microsoft.EntityFrameworkCore;
using MovieHub.API.Data;
using MovieHub.API.DTOs.Genres;
using MovieHub.API.Exceptions;
using MovieHub.API.Models;

namespace MovieHub.API.Services;

public class GenreService (AppDbContext dbContext)
{
    private readonly AppDbContext _dbContext = dbContext;


    public Task<List<GenreResponse>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return _dbContext.Genres
            .AsNoTracking()
            .OrderBy(g => g.Name)
            .Select(g => new GenreResponse
            {
                Id = g.Id,
                Name = g.Name
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<GenreResponse> CreateAsync(
        CreateGenreRequest request,
        CancellationToken cancellationToken = default)
    {
        var nameTaken = await _dbContext.Genres
            .AnyAsync(g => g.Name == request.Name, cancellationToken);

        if (nameTaken)
        {
            throw new ConflictException("Genre name is already taken.");
        }

        var genre = new Genre
        {
            Name = request.Name
        };

        _dbContext.Genres.Add(genre);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return MapToResponse(genre);
    }

    private static GenreResponse MapToResponse(Genre genre)
    {
        return new GenreResponse
        {
            Id = genre.Id,
            Name = genre.Name
        };
    }
}
