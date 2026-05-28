namespace MovieHub.API.Models;

public class Genre
{
    public int Id { get; set; }
    public required string Name { get; set; }

    public ICollection<Movie> Movies { get; set; } = [];
}
