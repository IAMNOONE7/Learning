using System.Configuration;
using System.Data;
using System.Windows;
using CsClean.Application.UseCases;
using CsClean.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using CsClean.Presentation.Wpf.MVVM.ViewModel;

namespace CsClean
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : System.Windows.Application
    {
        private IHost? _host; // NOTE: Generic Host for DI + config + logging.


        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);


            // NOTE TO SELF: Build the host (composition root)
            _host = Host.CreateDefaultBuilder()
            .ConfigureServices(services =>
            {
                // Application layer services (use cases)
                services.AddTransient<GetAllLocalMessages>();
                services.AddTransient<SaveLocalMessage>();
                services.AddTransient<DeleteLocalMessageByName>();


                // Infrastructure bindings (implement ports)
                services.AddInfrastructure();


                // Presentation registrations
                services.AddSingleton<MainWindow>();
                services.AddSingleton<MainViewModel>();
            })
            .Build();


            // Resolve main window from DI so its VM also gets DI
            var mainWindow = _host.Services.GetRequiredService<MainWindow>();
            mainWindow.DataContext = _host.Services.GetRequiredService<MainViewModel>();
            MainWindow = mainWindow;
            mainWindow.Show();
        }


        protected override async void OnExit(ExitEventArgs e)
        {
            // Graceful stop if needed later
            if (_host is IAsyncDisposable asyncHost) await asyncHost.DisposeAsync();
            else _host?.Dispose();


            base.OnExit(e);
        }
    }
}
