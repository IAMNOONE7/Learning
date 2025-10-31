using cs_DependencyInjection.MVVM.ViewModel;
using Microsoft.Extensions.Hosting;
using System.Configuration;
using System.Data;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using cs_DependencyInjection.Services;

namespace cs_DependencyInjection
{
    // App.xaml.cs = "entry point" of WPF app.
    // This is where I set up Dependency Injection (composition root).
    public partial class App : Application
    {
        private IHost? _host; // Host = object that holds all registered services (DI container)

        protected override void OnStartup(StartupEventArgs e)
        {
            // 1) Build the DI container (called "Host")
            _host = Host.CreateDefaultBuilder()
                .ConfigureServices(services =>
                {
                    // Here I tell DI which implementation to use when someone asks for IGreeter
                    // Try switching to FormalGreeter to instantly change the app's behavior.
                    services.AddTransient<IGreeter, CasualGreeter>();
                    // services.AddTransient<IGreeter, FormalGreeter>();

                    // Register my ViewModel and View too
                    services.AddTransient<MainViewModel>();
                    services.AddTransient<MainWindow>();

                    // Register singletons — these live for the whole app lifetime
                    services.AddSingleton<NavigationStore>();
                    services.AddSingleton<INavigationService, NavigationService>();

                    // Register the MainWindow and MainViewModel
                    services.AddSingleton<MainViewModel>();
                    services.AddSingleton<MainWindow>();

                    // Register screen-specific ViewModels
                    services.AddTransient<HomeViewModel>();
                    services.AddTransient<SettingsViewModel>();
                })
                .Build();

            // Get NavigationStore and set the first screen
            var store = _host.Services.GetRequiredService<NavigationStore>();
            store.CurrentViewModel = _host.Services.GetRequiredService<HomeViewModel>();

            //  Create the main window using DI (so it can receive injected dependencies)
            var window = _host.Services.GetRequiredService<MainWindow>();
            window.DataContext = _host.Services.GetRequiredService<MainViewModel>();
            window.Show();

            base.OnStartup(e);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            // Properly dispose of the host when the app closes
            _host?.Dispose();
            base.OnExit(e);
        }
    }
}
