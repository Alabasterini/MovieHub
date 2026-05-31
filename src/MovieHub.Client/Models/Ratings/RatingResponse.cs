namespace MovieHub.Client.Models.Ratings;

using System;

public class RatingResponse
{
    public int Id { get; set; }
    public int Value { get; set; }
    public string? Comment { get; set; }
    public DateTime CreatedAt { get; set; }
    public int UserId { get; set; }
    public required string Username { get; set; }
}
