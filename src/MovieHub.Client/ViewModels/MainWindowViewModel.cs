using MovieHub.Client.Services;

namespace MovieHub.Client.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    private readonly NavigationService _navigationService;

    public object? CurrentView => _navigationService.CurrentView;

    public MainWindowViewModel(NavigationService navigationService)
    {
        _navigationService = navigationService;
        _navigationService.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == nameof(NavigationService.CurrentView))
                OnPropertyChanged(nameof(CurrentView));
        };

        _navigationService.NavigateTo<AuthViewModel>();
    }
}