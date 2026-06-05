using CommunityToolkit.Mvvm.Input;
using MovieHub.Client.Models.Movies;
using System;

namespace MovieHub.Client.ViewModels;

public partial class MovieSummaryViewModel(MovieSummaryResponse movie, Action<int> onSelected) : ViewModelBase
{
    public int Id { get; } = movie.Id;
    public string Title { get; } = movie.Title;
    public int Year { get; } = movie.Year;
    public string GenreName { get; } = movie.Genre.Name;
    public string? PosterUrl { get; } = movie.PosterUrl;
    public double? AverageRating { get; } = movie.AverageRating;
    public string ScoreDisplay => AverageRating.HasValue
        ? AverageRating.Value.ToString("F1")
        : "—";

    private readonly Action<int> _onSelected = onSelected;
    

    [RelayCommand]
    private void Select() => _onSelected(Id);
}