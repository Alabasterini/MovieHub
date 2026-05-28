namespace MovieHub.API.DTOs.Movies;

public class MovieSummaryResponse
{
    public int Id { get; set; }
    public required string Title { get; set; }
    public int Year { get; set; }
    public string? PosterUrl { get; set; }
    public required MovieGenreSummaryResponse Genre { get; set; }
    public required DirectorSummaryResponse Director { get; set; }
}
