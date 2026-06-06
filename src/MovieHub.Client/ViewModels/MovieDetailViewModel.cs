using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MovieHub.Client.Models.Movies;
using MovieHub.Client.Models.Ratings;
using MovieHub.Client.Services;

namespace MovieHub.Client.ViewModels;

public partial class MovieDetailViewModel(ApiClient api, SessionService session) : ViewModelBase
{

    private bool _starsAdded;
    
    [ObservableProperty] 
    private int _movieId;
    
    [ObservableProperty] 
    private string _title = string.Empty;
    
    [ObservableProperty] 
    private int _year;
    
    [ObservableProperty] 
    private string _description = string.Empty;
    
    [ObservableProperty] 
    private string? _posterUrl;
    
    [ObservableProperty] 
    private MovieGenreSummaryResponse? _genre;
    
    [ObservableProperty] 
    private DirectorSummaryResponse? _director;
    
    [ObservableProperty]
    private IReadOnlyList<RatingResponse>? _ratings;
    
    [ObservableProperty] 
    private double? _averageRating;
    
    [ObservableProperty] 
    private bool _isLoading;
    
    [ObservableProperty] 
    private string? _errorMessage;
    
    [ObservableProperty] 
    private int? _userRating;
    
    [ObservableProperty] 
    private bool _showCommentBox;
    
    [ObservableProperty] 
    private string _comment = string.Empty;
    
    [ObservableProperty] 
    private int? _existingRatingId;
    
    public ObservableCollection<StarViewModel> Stars { get; } = [];

    public string DirectorFullName => Director is null ? string.Empty : $"{Director.FirstName} {Director.LastName}";
    public string GenreName => Genre?.Name ?? string.Empty;
    public string ScoreDisplay => AverageRating.HasValue
        ? AverageRating.Value.ToString("F1")
        : "Not rated yet";

    partial void OnMovieIdChanged(int value)
        => LoadMovieCommand.ExecuteAsync(null);

    [RelayCommand]
    private async Task LoadMovieAsync()
    {
        IsLoading = true;
        ErrorMessage = null;
        try
        {
            var movie = await api.GetMovieAsync(MovieId);
            Title = movie.Title;
            Year = movie.Year;
            Description = movie.Description;
            PosterUrl = movie.PosterUrl;
            Genre = movie.Genre;
            OnPropertyChanged(nameof(GenreName));
            Director = movie.Director;
            OnPropertyChanged(nameof(DirectorFullName));
            AverageRating = movie.AverageRating;
            Ratings = movie.Ratings;
            OnPropertyChanged(nameof(ScoreDisplay));
            if (!_starsAdded)
            {
                for (var i = 1; i <= 10; i++)
                {
                    var star = new StarViewModel(i, SetRating);
                    Stars.Add(star);
                }
                _starsAdded = true;
            }
            
            var existingRating = movie.Ratings
                .FirstOrDefault(r => r.UserId == session.UserId);

            UserRating = existingRating?.Value;
            ExistingRatingId = existingRating?.Id;
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Failed to load movie: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }
    
    partial void OnUserRatingChanged(int? value)
    {
        foreach (var star in Stars)
            star.IsFilled = value.HasValue && star.Value <= value.Value;
    }
    
    private void SetRating(int value)
    {
        UserRating = value;
        ShowCommentBox = true;
    }
    
    [RelayCommand]
    private async Task SubmitRatingAsync()
    {
        if (!UserRating.HasValue) return;

        try
        {
            if (ExistingRatingId.HasValue)
                await api.UpdateRatingAsync(ExistingRatingId.Value,
                    new UpdateRatingRequest { Comment = Comment, Value = UserRating.Value });
            else
            {
                var created = await api.CreateRatingAsync(MovieId,
                    new CreateRatingRequest { Comment = Comment, Value = UserRating.Value });
                ExistingRatingId = created.Id;
            }
    
            ShowCommentBox = false;
            await LoadMovieAsync();
        }
        catch (Exception)
        {
            ErrorMessage = $"MovieId: {MovieId}, UserRating: {UserRating}";
        }
    }
}