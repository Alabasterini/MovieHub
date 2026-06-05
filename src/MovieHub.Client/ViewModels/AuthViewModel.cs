using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MovieHub.Client.Exceptions;
using MovieHub.Client.Models.Auth;
using MovieHub.Client.Services;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace MovieHub.Client.ViewModels;

public partial class AuthViewModel(ApiClient apiClient, SessionService sessionService, NavigationService navigationService) : ViewModelBase
{
    private readonly ApiClient _apiClient = apiClient;
    private readonly SessionService _sessionService = sessionService;
    private readonly NavigationService _navigationService = navigationService;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(SubmitCommand))]
    private bool _isLoginTab = true;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(SubmitCommand))]
    private string _email = string.Empty;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(SubmitCommand))]
    private string _username = string.Empty;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(SubmitCommand))]
    private string _password = string.Empty;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(SubmitCommand))]
    private string _passwordRegister = string.Empty;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(SubmitCommand))]
    private bool _isLoading;

    [ObservableProperty]
    private string _errorMessage = string.Empty;

    [ObservableProperty]
    private string _loginColor = "#F97316";

    [ObservableProperty]
    private string _registerColor = "#636366";

    [ObservableProperty]
    private string _buttonMessage = "Login";

    private const string EmailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";

    [RelayCommand]
    private async Task LoginAsync()
    {
        try
        {
            IsLoading = true;

            var response = await _apiClient.LoginAsync(new LoginRequest { Email = Email, Password = Password });
            _sessionService.SetSession(response.AccessToken, response.Username, response.Role);
            _navigationService.NavigateTo<MainViewModel>();
        }
        catch (Exception ex)
        {
            ErrorMessage = new HttpRequestException(ex.Message).Message;
        }
        finally
        {
            IsLoading = false;
        }
    }
    [RelayCommand]
    private async Task RegisterAsync()
    {
        try
        {
            IsLoading = true;

            await _apiClient.RegisterAsync(new RegisterRequest { Username = Username, Email = Email, Password = Password });
        }
        catch (Exception)
        {
            ErrorMessage = new HttpRequestException("There was problem while registering").Message;
        }
        finally
        {
            IsLoading = false;
            IsLoginTab = true;
        }
    }
    [RelayCommand]
    private void SwitchToLogin()
    {
        IsLoginTab = true;
        LoginColor = "#F97316";
        RegisterColor = "#636366";
        ButtonMessage = "Login";
    }

    [RelayCommand]
    private void SwitchToRegister()
    {
        IsLoginTab = false;
        LoginColor = "#636366";
        RegisterColor = "#F97316";
        ButtonMessage = "Create Account";
    }
    [RelayCommand(CanExecute = nameof(CanSubmit))]
    private async Task SubmitAsync()
    {
        if (IsLoginTab)
            await LoginAsync();
        else
            await RegisterAsync();
    }

    private bool CanSubmit()
    {

        if (IsLoading)
            return false;

        if (IsLoginTab)
        {
            ErrorMessage = string.Empty;
            return Email.Length != 0 || Password.Length != 0;
        }

        if (!IsLoginTab)
        {
            if (Password != PasswordRegister) ErrorMessage = new PasswordMissMatchException("Passwords doesn't match").Message;
            if (Password == PasswordRegister) ErrorMessage = string.Empty;
            return IsValidEmail(Email) && Username.Length != 0 && Password.Length != 0 && Password == PasswordRegister;
        }
        
        return false;
    }
    private static bool IsValidEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return false;
        try
        {
            return Regex.IsMatch(email, EmailPattern, RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
        }
        catch (RegexMatchTimeoutException)
        {
            return false;
        }
    }
}