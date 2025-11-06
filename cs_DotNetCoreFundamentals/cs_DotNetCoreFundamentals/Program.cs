using cs_DotNetCoreFundamentals.Services;
using cs_DotNetCoreFundamentals.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace cs_DotNetCoreFundamentals
{
    /*
    ================================================================================
      .NET, .NET Framework, and .NET Core — what’s the difference?
    ================================================================================

    1️)  .NET Framework
    -------------------
    - Windows-only   
    - Used for desktop apps (WinForms, WPF)
    - Tightly bound to Windows
    - Still supported for legacy projects, but no new features are added.

    2️)  .NET Core
    --------------
    - Introduced around 2016 as a complete redesign of .NET.
    - Cross-platform: runs on Windows, Linux, and macOS.
    - Open source and modular — smaller runtime, faster startup, less memory.
    - Introduced a unified "Host" model with built-in Dependency Injection (DI),
      Configuration, and Logging. 
    - Used by ASP.NET Core, Worker Services, and modern Console apps.

    3️)  Modern .NET (5, 6, 7, 8, 9…)
    ---------------------------------
    - After .NET 5, Microsoft merged everything back under one name: ".NET".
    - Technically, ".NET 5+" continues .NET Core, not .NET Framework.
    - So when we say ".NET Core style" today, we really mean "modern .NET pattern":
      - Uses HostBuilder
      - Uses Dependency Injection by default
      - Uses appsettings.json
      - Is cross-platform

    In short:
        .NET Framework - old, Windows-only.
        .NET Core - cross-platform, new runtime, foundation for modern .NET.
        .NET 8 / 9 - continuation of .NET Core — same architecture, unified branding.


    ================================================================================
    How this console app works
    ================================================================================
    This project looks like a simple console app, but it uses the same core building
    blocks as an ASP.NET Core or Worker Service app.
       
    1️)  HostBuilder
    ----------------
    - We create a "Host" using Host.CreateDefaultBuilder(args).
    - The Host acts like a container or an orchestrator:
        - Loads configuration (from appsettings.json, environment vars, etc.)
        - Sets up Dependency Injection (IServiceCollection)
        - Sets up Logging
        - Manages the application lifetime
    - You can think of the Host as the "engine" that runs your app.

    2️)  Dependency Injection (DI)
    ------------------------------
    - Instead of creating objects manually with "new", we register them as services.
    - The DI container knows how to construct each class and inject its dependencies.
    - Example:
          services.AddSingleton<IDateTimeProvider, SystemDateTimeProvider>();
          services.AddTransient<IOrderProcessor, OrderProcessor>();
    - Now when we ask for IOrderProcessor, .NET automatically gives us a new instance
      with a logger, a time provider, and configuration already injected.

    3️)  Configuration
    ------------------
    - appsettings.json is automatically loaded by the Host.
    - Configuration data (like RunMode, ProcessedBy) is bound to the class AppSettings.
    - We can access these values via IOptions<AppSettings>.
    - This keeps config separate from code and makes it environment-friendly.

    4️)  Logging
    ------------
    - Built-in logging is automatically set up by the Host.
    - Any class can request an ILogger<T> and write structured logs.
    - Output goes to the console, but could be switched to a file, Seq, Application
      Insights, or a remote log server.

    5️)  The Application Flow
    -------------------------
    - Program.Main() builds the Host - sets up DI, config, and logging.
    - We create a service scope and resolve IOrderProcessor.
    - The OrderProcessor:
          - Logs start time and configuration details.
          - Simulates order processing.
          - Logs completion time.
    - The app exits gracefully.
    
    ---------------------------------   
    In essence:
      This "console app" demonstrates the core architecture of modern .NET.
      Once you understand this structure, you understand how ASP.NET Core, Worker
      Services, Background Jobs, and APIs work under the hood.
    ================================================================================
    */

    public class Program
    {
        /*
            Entry point for the application.

            Goal:
            - use a Host (like ASP.NET Core does)
            - register our services
            - load configuration from appsettings.json
            - run our "business service"
        */
        public static async Task Main(string[] args)
        {
            var host = Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((context, config) =>
                {
                    // This ensures appsettings.json is loaded explicitly.
                    // Host.CreateDefaultBuilder already does this,
                    // but it's good to know where to customize it.
                    config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
                })
                .ConfigureServices((context, services) =>
                {
                    IConfiguration configuration = context.Configuration;

                    // bind AppSettings section to our typed class
                    services.Configure<AppSettings>(configuration.GetSection("AppSettings"));

                    // register our services
                    services.AddSingleton<IDateTimeProvider, SystemDateTimeProvider>();
                    services.AddTransient<IOrderProcessor, OrderProcessor>();
                })
                .Build();

            // create scope and run
            using var scope = host.Services.CreateScope();
            var services = scope.ServiceProvider;

            try
            {
                var processor = services.GetRequiredService<IOrderProcessor>();
                await processor.ProcessAsync();
            }
            catch (Exception ex)
            {
                var logger = services.GetRequiredService<ILogger<Program>>();
                logger.LogError(ex, "Application failed.");
            }
        }
    }
}
