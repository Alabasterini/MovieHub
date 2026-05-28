namespace MovieHub.API.DTOs.Ratings;

public class CreateRatingRequest
{
    public int Value { get; set; }
    public string? Comment { get; set; }
}
