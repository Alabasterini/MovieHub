using Avalonia.Controls;
using MovieHub.Client.ViewModels;

namespace MovieHub.Client.Views;

public partial class MainView : UserControl
{
    public MainView()
    {
        InitializeComponent();
        DataContextChanged += async (_, _) =>
        {
            if (DataContext is MainViewModel vm)
                await vm.InitializeAsync();
        };
    }
}