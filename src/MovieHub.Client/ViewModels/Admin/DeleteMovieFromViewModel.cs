using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MovieHub.Client.Services;

namespace MovieHub.Client.ViewModels.Admin;

public partial class DeleteMovieFormViewModel(ApiClient api) : ObservableObject
{
    [ObservableProperty] private MovieListItem? _selectedMovie;
    [ObservableProperty] private string? _successMessage;
    [ObservableProperty] private string? _errorMessage;

    public ObservableCollection<MovieListItem> Movies { get; } = [];

    public async Task InitializeAsync()
    {
        try
        {
            var movies = await api.GetMoviesAsync(null, null, null);
            Movies.Clear();
            foreach (var m in movies)
                Movies.Add(new MovieListItem(m.Id, m.Title));
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
            await api.DeleteMovieAsync(SelectedMovie.Id);
            SuccessMessage = $"Movie \"{SelectedMovie.Title}\" deleted.";
            Movies.Remove(SelectedMovie);
            SelectedMovie = null;
        }
        catch (Exception ex) { ErrorMessage = ex.Message; }
    }
}