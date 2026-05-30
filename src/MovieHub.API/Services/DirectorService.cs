using Microsoft.EntityFrameworkCore;
using MovieHub.API.Data;
using MovieHub.API.DTOs.Directors;
using MovieHub.API.Models;

namespace MovieHub.API.Services;

public class DirectorService(AppDbContext dbContext)
{
    private readonly AppDbContext _dbContext = dbContext;

    public Task<List<DirectorResponse>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return _dbContext.Directors
            .AsNoTracking()
            .OrderBy(g => g.LastName)
            .Select(g => MapToResponse(g))
            .ToListAsync(cancellationToken);
    }

    public async Task<DirectorResponse> CreateAsync(
        CreateDirectorRequest request,
        CancellationToken cancellationToken = default)
    {

        var director = new Director
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Nationality = request.Nationality
        };

        _dbContext.Directors.Add(director);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return MapToResponse(director);
    }
    private static DirectorResponse MapToResponse(Director director)
    {
        return new DirectorResponse
        {
            Id = director.Id,
            FirstName = director.FirstName,
            LastName = director.LastName,
            Nationality = director.Nationality
        };
    }

}