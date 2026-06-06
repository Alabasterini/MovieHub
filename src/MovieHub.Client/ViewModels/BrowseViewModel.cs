using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MovieHub.Client.Services;

namespace MovieHub.Client.ViewModels;

public partial class BrowseViewModel(ApiClient api, NavigationService navigation) : ViewModelBase
{
    private readonly ApiClient _api = api;
    private readonly NavigationService _navigation = navigation;

    [ObservableProperty] 
    private string _searchTitle = string.Empty;
    
    [ObservableProperty] 
    private string _selectedGenreId = string.Empty;
    
    [ObservableProperty] 
    private string _selectedDirectorId = string.Empty;
    
    [ObservableProperty] 
    private bool _isLoading;
    
    [ObservableProperty] 
    private string? _errorMessage;

    public ObservableCollection<MovieSummaryViewModel> Movies { get; } = [];
    public ObservableCollection<GenreFilterItem> Genres { get; } = [];
    public ObservableCollection<DirectorFilterItem> Directors { get; } = [];
    
    
     public async Task InitializeAsync()
    {
        await Task.WhenAll(LoadGenresAsync(), LoadDirectorsAsync());
        await LoadMoviesAsync();
    }

    [RelayCommand]
    private async Task SearchAsync() => await LoadMoviesAsync();

    private async Task LoadMoviesAsync()
    {
        IsLoading = true;
        ErrorMessage = null;
        try
        {
            var genreId = string.IsNullOrEmpty(SelectedGenreId) ? (int?)null : int.Parse(SelectedGenreId);
            var directorId = string.IsNullOrEmpty(SelectedDirectorId) ? (int?)null : int.Parse(SelectedDirectorId);

            var movies = await _api.GetMoviesAsync(
                title: string.IsNullOrWhiteSpace(SearchTitle) ? null : SearchTitle,
                genreId: genreId,
                directorId: directorId
            );

            Movies.Clear();
            foreach (var m in movies)
                Movies.Add(new MovieSummaryViewModel(m, NavigateToMovie));
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Failed to load movies: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    private void NavigateToMovie(int movieId)
    {
        _navigation.NavigateToMovie(movieId);
    }

    private async Task LoadGenresAsync()
    {
        try
        {
            var genres = await _api.GetGenresAsync();
            Genres.Clear();
            Genres.Add(new GenreFilterItem(null, "All Genres"));
            foreach (var g in genres)
                Genres.Add(new GenreFilterItem(g.Id, g.Name));
            
            SelectedGenreId = string.Empty;
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
        }
    }

    private async Task LoadDirectorsAsync()
    {
        try
        {
            var directors = await _api.GetDirectorsAsync();
            Directors.Clear();
            Directors.Add(new DirectorFilterItem(null, "All Directors"));
            foreach (var d in directors)
                Directors.Add(new DirectorFilterItem(d.Id, $"{d.FirstName} {d.LastName}"));
            
            SelectedDirectorId = string.Empty;
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
        }
    }
}

public record GenreFilterItem(int? Id, string Name)
{
    public string IdString => Id?.ToString() ?? string.Empty;
}

public record DirectorFilterItem(int? Id, string Name)
{
    public string IdString => Id?.ToString() ?? string.Empty;
}