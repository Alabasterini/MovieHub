using CommunityToolkit.Mvvm.Input;
using MovieHub.Client.Services;

namespace MovieHub.Client.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    private readonly NavigationService _navigationService;
    private readonly SessionService _sessionService;

    public object? CurrentView => _navigationService.CurrentView;
    public bool ShowShell => CurrentView is not AuthViewModel;
    public bool IsAdmin => _sessionService.IsAdmin;
    public string? Username => _sessionService.Username;

    public MainWindowViewModel(NavigationService navigationService, SessionService sessionService)
    {
        _navigationService = navigationService;
        _sessionService = sessionService;

        _navigationService.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == nameof(NavigationService.CurrentView))
            {
                OnPropertyChanged(nameof(CurrentView));
                OnPropertyChanged(nameof(ShowShell));
                OnPropertyChanged(nameof(IsAdmin));
                OnPropertyChanged(nameof(Username));
            }
        };

        _navigationService.NavigateTo<AuthViewModel>();
    }

    [RelayCommand]
    private void NavigateToAdminPanel()
    {
        // _navigationService.NavigateTo<AdminViewModel>();
    }
    
    [RelayCommand]
    private void NavigateToBrowsePanel()
    {
         _navigationService.NavigateTo<SearchViewModel>();
    }

    [RelayCommand]
    private void Logout()
    {
        _sessionService.Clear();
        _navigationService.NavigateTo<AuthViewModel>();
    }
}