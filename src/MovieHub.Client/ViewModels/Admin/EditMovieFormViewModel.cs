using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MovieHub.Client.Models.Movies;
using MovieHub.Client.Services;

namespace MovieHub.Client.ViewModels.Admin;

public partial class EditMovieFormViewModel(ApiClient api) : ObservableObject
{
    [ObservableProperty] private string _searchTitle = string.Empty;
    [ObservableProperty] private MovieListItem? _selectedMovie;
    [ObservableProperty] private string _title = string.Empty;
    [ObservableProperty] private int _year;
    [ObservableProperty] private string _description = string.Empty;
    [ObservableProperty] private string? _posterUrl;
    [ObservableProperty] private GenreListItem? _selectedGenre;
    [ObservableProperty] private DirectorListItem? _selectedDirector;
    [ObservableProperty] private string? _successMessage;
    [ObservableProperty] private string? _errorMessage;
    [ObservableProperty] private bool _movieLoaded;

    public ObservableCollection<MovieListItem> Movies { get; } = [];
    public ObservableCollection<GenreListItem> Genres { get; } = [];
    public ObservableCollection<DirectorListItem> Directors { get; } = [];

    public async Task InitializeAsync()
    {
        try
        {
            await LoadMoviesAsync();

            var genres = await api.GetGenresAsync();
            foreach (var g in genres)
                Genres.Add(new GenreListItem(g.Id, g.Name));

            var directors = await api.GetDirectorsAsync();
            foreach (var d in directors)
                Directors.Add(new DirectorListItem(d.Id, $"{d.FirstName} {d.LastName}"));
        }
        catch (Exception ex) { ErrorMessage = ex.Message; }
    }

    private async Task LoadMoviesAsync()
    {
        var movies = await api.GetMoviesAsync();
        Movies.Clear();
        foreach (var m in movies)
            Movies.Add(new MovieListItem(m.Id, m.Title));
    }

    partial void OnSelectedMovieChanged(MovieListItem? value)
    {
        if (value is null) return;
        LoadMovieDetails(value.Id);
    }

    private async void LoadMovieDetails(int id)
    {
        try
        {
            var movie = await api.GetMovieAsync(id);
            Title = movie.Title;
            Year = movie.Year;
            Description = movie.Description;
            PosterUrl = movie.PosterUrl;
            SelectedGenre = Genres.FirstOrDefault(g => g.Id == movie.Genre.Id);
            SelectedDirector = Directors.FirstOrDefault(d => d.Id == movie.Director.Id);
            MovieLoaded = true;
        }
        catch (Exception ex) { ErrorMessage = ex.Message; }
    }

    [RelayCommand]
    private async Task SubmitAsync()
    {
        if (SelectedMovie is null) return;
        ErrorMessage = null;
        try
        {
            await api.UpdateMovieAsync(SelectedMovie.Id, new UpdateMovieRequest
            {
                Title = Title,
                Year = Year,
                Description = Description,
                PosterUrl = PosterUrl,
                GenreId = SelectedGenre!.Id,
                DirectorId = SelectedDirector!.Id
            });
            SuccessMessage = $"Movie \"{Title}\" updated.";
        }
        catch (Exception ex) { ErrorMessage = ex.Message; }
    }
}

public record MovieListItem(int Id, string Title);