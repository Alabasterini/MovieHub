namespace MovieHub.API.Models;

public class Rating
{
    public int Id { get; set; }
    public int Value { get; set; }
    public string? Comment { get; set; }
    public DateTime CreatedAt { get; set; }
    public int MovieId { get; set; }
    public int UserId { get; set; }

    public Movie Movie { get; set; } = null!;
    public User User { get; set; } = null!;
}
