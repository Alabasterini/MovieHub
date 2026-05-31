using MovieHub.Client.Models.Ratings;
using System.Collections.Generic;

namespace MovieHub.Client.Models.Movies;

public class MovieDetailResponse
{
    public int Id { get; set; }
    public required string Title { get; set; }
    public int Year { get; set; }
    public required string Description { get; set; }
    public string? PosterUrl { get; set; }
    public required MovieGenreSummaryResponse Genre { get; set; }
    public required DirectorSummaryResponse Director { get; set; }
    public required IReadOnlyList<RatingResponse> Ratings { get; set; }
    public double? AverageScore { get; set; }
}
