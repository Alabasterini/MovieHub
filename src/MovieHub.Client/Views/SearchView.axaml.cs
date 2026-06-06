using Avalonia.Controls;
using MovieHub.Client.ViewModels;

namespace MovieHub.Client.Views;

public partial class SearchView : UserControl
{
    public SearchView()
    {
        InitializeComponent();
        DataContextChanged += async (_, _) =>
        {
            if (DataContext is SearchViewModel vm)
                await vm.InitializeAsync();
        };
    }
}