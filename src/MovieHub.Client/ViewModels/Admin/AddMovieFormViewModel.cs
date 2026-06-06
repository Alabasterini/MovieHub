using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MovieHub.Client.Models.Directors;
using MovieHub.Client.Models.Genres;
using MovieHub.Client.Models.Movies;
using MovieHub.Client.Services;

namespace MovieHub.Client.ViewModels.Admin;

public partial class AddMovieFormViewModel(ApiClient api) : ObservableObject
{
    [ObservableProperty] private string _title = string.Empty;
    [ObservableProperty] private int _year = DateTime.Now.Year;
    [ObservableProperty] private string _description = string.Empty;
    [ObservableProperty] private string? _posterUrl;
    [ObservableProperty] private GenreListItem? _selectedGenre;
    [ObservableProperty] private DirectorListItem? _selectedDirector;
    [ObservableProperty] private string? _successMessage;
    [ObservableProperty] private string? _errorMessage;

    public ObservableCollection<GenreListItem> Genres { get; } = [];
    public ObservableCollection<DirectorListItem> Directors { get; } = [];

    public async Task InitializeAsync()
    {
        try
        {
            var genres = await api.GetGenresAsync();
            Genres.Clear();
            foreach (var g in genres)
                Genres.Add(new GenreListItem(g.Id, g.Name));

            var directors = await api.GetDirectorsAsync();
            Directors.Clear();
            foreach (var d in directors)
                Directors.Add(new DirectorListItem(d.Id, $"{d.FirstName} {d.LastName}"));
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
        }
    }

    [RelayCommand]
    private async Task SubmitAsync()
    {
        if (string.IsNullOrWhiteSpace(Title) || SelectedGenre is null || SelectedDirector is null)
        {
            ErrorMessage = "Fill in all fields.";
            return;
        }

        ErrorMessage = null;
        try
        {
            await api.CreateMovieAsync(new CreateMovieRequest
            {
                Title = Title,
                Year = Year,
                Description = Description,
                PosterUrl = PosterUrl,
                GenreId = SelectedGenre.Id,
                DirectorId = SelectedDirector.Id
            });
            SuccessMessage = $"Movie \"{Title}\" added successfully.";
            Title = string.Empty;
            Description = string.Empty;
            PosterUrl = null;
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
        }
    }
}

public record GenreListItem(int Id, string Name);
public record DirectorListItem(int Id, string Name);