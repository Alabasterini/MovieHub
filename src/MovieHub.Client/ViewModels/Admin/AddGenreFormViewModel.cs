using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MovieHub.Client.Models.Genres;
using MovieHub.Client.Services;

namespace MovieHub.Client.ViewModels.Admin;

public partial class AddGenreFormViewModel(ApiClient api) : ObservableObject
{
    [ObservableProperty] private string _name = string.Empty;
    [ObservableProperty] private string? _successMessage;
    [ObservableProperty] private string? _errorMessage;

    [RelayCommand]
    private async Task SubmitAsync()
    {
        if (string.IsNullOrWhiteSpace(Name))
        {
            ErrorMessage = "Genre name is required.";
            return;
        }
        ErrorMessage = null;
        try
        {
            await api.CreateGenreAsync(new CreateGenreRequest { Name = Name });
            SuccessMessage = $"Genre \"{Name}\" added.";
            Name = string.Empty;
        }
        catch (Exception ex) { ErrorMessage = ex.Message; }
    }
}