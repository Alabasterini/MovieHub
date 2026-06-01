using System;
using System.Net.Http;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;
using MovieHub.Client.Services;
using MovieHub.Client.ViewModels;
using MovieHub.Client.Views;

namespace MovieHub.Client;

public partial class App : Application
{
    public static IServiceProvider Services { get; private set; } = null!;

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        var collection = new ServiceCollection();
        ConfigureServices(collection);
        Services = collection.BuildServiceProvider();

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow
            {
                DataContext = Services.GetRequiredService<MainWindowViewModel>(),
            };
        }

        base.OnFrameworkInitializationCompleted();
    }

    private static void ConfigureServices(IServiceCollection services)
    {
        services.AddSingleton<HttpClient>(_ => new HttpClient
        {
            BaseAddress = new Uri("http://localhost:5080")
        });

        services.AddSingleton<ApiClient>();
        services.AddSingleton<SessionService>();
        services.AddSingleton<NavigationService>(sp => new NavigationService(
            type => sp.GetRequiredService(type)
        ));

        services.AddSingleton<MainWindowViewModel>();
        services.AddTransient<AuthViewModel>();
        //services.AddTransient<BrowseViewModel>();
        //services.AddTransient<MovieDetailViewModel>();
        //services.AddTransient<AdminViewModel>();
    }
}