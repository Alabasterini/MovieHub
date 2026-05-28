namespace MovieHub.API.DTOs.Movies;

public class MovieListQueryRequest
{
    public string? Title { get; set; }
    public int? GenreId { get; set; }
    public int? DirectorId { get; set; }
}
