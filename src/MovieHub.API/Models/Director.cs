namespace MovieHub.API.Models;

public class Director
{
    public int Id { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required string Nationality { get; set; }

    public ICollection<Movie> Movies { get; set; } = [];
}
