using Microsoft.Extensions.Logging;

namespace LmpLink.MAUI;

public static class MauiProgram
{
    private static readonly string LogPath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.Desktop), 
        "maui_debug.log");

    public static void Log(string message)
    {
        var line = $"[{DateTime.Now:HH:mm:ss.fff}] {message}";
        try
        {
            File.AppendAllText(LogPath, line + Environment.NewLine);
        }
        catch { }
        System.Diagnostics.Debug.WriteLine(line);
    }

    public static MauiApp CreateMauiApp()
    {
        Log("=== MauiProgram.CreateMauiApp START ===");
        
        try
        {
            var builder = MauiApp.CreateBuilder();
            Log("[1] MauiAppBuilder created");
            
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    // fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    // fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });
            Log("[2] UseMauiApp<App> configured");

#if DEBUG
            builder.Logging.AddDebug();
#endif

            // Register Services
            builder.Services.AddSingleton<IMockDataService, MockDataService>();
            builder.Services.AddSingleton<ILocationService, LocationService>();
            builder.Services.AddSingleton<IMapService, MapService>();
            Log("[3] Services registered");

            // Register ViewModels
            builder.Services.AddTransient<MapViewModel>();
            Log("[4] ViewModels registered");

            // Register Pages
            builder.Services.AddTransient<MainPage>();
            builder.Services.AddTransient<TestPage>();
            Log("[5] Pages registered");

            var app = builder.Build();
            Log("=== MauiProgram.CreateMauiApp SUCCESS ===");
            return app;
        }
        catch (Exception ex)
        {
            Log($"!!! MauiProgram.CreateMauiApp FAILED: {ex.Message}");
            Log(ex.StackTrace ?? "");
            throw;
        }
    }
}

