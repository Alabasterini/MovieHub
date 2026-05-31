namespace MovieHub.Client.Models.Ratings;

public class UpdateRatingRequest
{
    public int Value { get; set; }
    public string? Comment { get; set; }
}
