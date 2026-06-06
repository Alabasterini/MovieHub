using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MovieHub.Client.Services;
using MovieHub.Client.ViewModels.Admin;

namespace MovieHub.Client.ViewModels;

public partial class AdminViewModel( ApiClient api) : ViewModelBase
{
    [ObservableProperty] 
    private object? _activeForm;
    
    [ObservableProperty] 
    private string? _activeButton;
    
    public event Action<object?>? ActiveFormChanged;

    [RelayCommand]
    private void ShowAddMovie()
    {
        ActiveButton = "AddMovie";
        ActiveForm = new AddMovieFormViewModel(api);
    }

    [RelayCommand]
    private void ShowEditMovie()
    {
        ActiveButton = "EditMovie";
        ActiveForm = new EditMovieFormViewModel(api);
    }

    [RelayCommand]
    private void ShowDeleteMovie()
    {
        ActiveButton = "DeleteMovie";
        ActiveForm = new DeleteMovieFormViewModel(api);
    }

    [RelayCommand]
    private void ShowAddGenre()
    {
        ActiveButton = "AddGenre";
        ActiveForm = new AddGenreFormViewModel(api);
    }

    [RelayCommand]
    private void ShowAddDirector()
    {
        ActiveButton = "AddDirector";
        ActiveForm = new AddDirectorFormViewModel(api);
    }
    
    partial void OnActiveFormChanged(object? value)
        => ActiveFormChanged?.Invoke(value);
}