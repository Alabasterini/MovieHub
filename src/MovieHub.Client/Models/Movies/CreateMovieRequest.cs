namespace MovieHub.Client.Models.Movies;

public class CreateMovieRequest
{
    public required string Title { get; set; }
    public int Year { get; set; }
    public required string Description { get; set; }
    public string? PosterUrl { get; set; }
    public int GenreId { get; set; }
    public int DirectorId { get; set; }
}
