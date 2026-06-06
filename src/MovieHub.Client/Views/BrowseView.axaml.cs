using Avalonia.Controls;
using MovieHub.Client.ViewModels;

namespace MovieHub.Client.Views;

public partial class BrowseView : UserControl
{
    public BrowseView()
    {
        InitializeComponent();
        DataContextChanged += async (_, _) =>
        {
            if (DataContext is BrowseViewModel vm)
                await vm.InitializeAsync();
        };
    }
}