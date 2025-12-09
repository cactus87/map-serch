namespace LmpLink.MAUI;

public partial class AppShell : Shell
{
    public AppShell()
    {
        MauiProgram.Log("=== AppShell Constructor START ===");
        try
        {
            InitializeComponent();
            MauiProgram.Log("[1] AppShell.InitializeComponent done");
            
            // Register routes for DI-based navigation
            Routing.RegisterRoute(nameof(MainPage), typeof(MainPage));
            Routing.RegisterRoute(nameof(TestPage), typeof(TestPage));
            MauiProgram.Log("=== AppShell Constructor SUCCESS ===");
        }
        catch (Exception ex)
        {
            MauiProgram.Log($"!!! AppShell Constructor FAILED: {ex.Message}");
            MauiProgram.Log(ex.StackTrace ?? "");
            throw;
        }
    }
}

