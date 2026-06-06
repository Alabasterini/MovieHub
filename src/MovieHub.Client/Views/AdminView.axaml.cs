using Avalonia.Controls;
using MovieHub.Client.ViewModels;
using MovieHub.Client.ViewModels.Admin;

namespace MovieHub.Client.Views;

public partial class AdminView : UserControl
{
    public AdminView()
    {
        InitializeComponent();
        DataContextChanged += (_, _) =>
        {
            if (DataContext is AdminViewModel vm)
                vm.ActiveFormChanged += OnActiveFormChanged;
        };
    }

    private static async void OnActiveFormChanged(object? form)
    {
        switch (form)
        {
            case AddMovieFormViewModel vm: await vm.InitializeAsync(); break;
            case EditMovieFormViewModel vm: await vm.InitializeAsync(); break;
            case DeleteMovieFormViewModel vm: await vm.InitializeAsync(); break;
        }
    }
}