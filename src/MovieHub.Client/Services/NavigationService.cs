using System;
using CommunityToolkit.Mvvm.ComponentModel;
using MovieHub.Client.ViewModels;

namespace MovieHub.Client.Services;

public class NavigationService(Func<Type, object> viewModelFactory) : ObservableObject
{
    private object? _currentView;

    public object? CurrentView
    {
        get => _currentView;
        private set => SetProperty(ref _currentView, value);
    }

    private readonly Func<Type, object> _viewModelFactory = viewModelFactory;

    public void NavigateTo<TViewModel>() where TViewModel : class
    {
        var viewModel = _viewModelFactory(typeof(TViewModel));
        CurrentView = viewModel;
    }
    public void NavigateToMovie(int movieId)
    {
        var viewModel = _viewModelFactory(typeof(MovieDetailViewModel));
        if (viewModel is MovieDetailViewModel detailVm)
        {
            CurrentView = viewModel;
            detailVm.MovieId = movieId;
        }
    }
}