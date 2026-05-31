namespace MovieHub.Client.Models.Movies;

public class MovieListQueryRequest
{
    public string? Title { get; set; }
    public int? GenreId { get; set; }
    public int? DirectorId { get; set; }
}
