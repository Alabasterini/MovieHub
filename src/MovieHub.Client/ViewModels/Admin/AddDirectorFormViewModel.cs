using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MovieHub.Client.Models.Directors;
using MovieHub.Client.Services;

namespace MovieHub.Client.ViewModels.Admin;

public partial class AddDirectorFormViewModel(ApiClient api) : ObservableObject
{
    [ObservableProperty] private string _firstName = string.Empty;
    [ObservableProperty] private string _lastName = string.Empty;
    [ObservableProperty] private string _nationality = string.Empty;
    [ObservableProperty] private string? _successMessage;
    [ObservableProperty] private string? _errorMessage;

    [RelayCommand]
    private async Task SubmitAsync()
    {
        if (string.IsNullOrWhiteSpace(FirstName) || string.IsNullOrWhiteSpace(LastName))
        {
            ErrorMessage = "Name and surname are required.";
            return;
        }
        ErrorMessage = null;
        try
        {
            await api.CreateDirectorAsync(new CreateDirectorRequest
            {
                FirstName = FirstName,
                LastName = LastName,
                Nationality = Nationality
            });
            SuccessMessage = $"{FirstName} {LastName} Successfully added.";
            FirstName = string.Empty;
            LastName = string.Empty;
            Nationality = string.Empty;
        }
        catch (Exception ex) { ErrorMessage = ex.Message; }
    }
}