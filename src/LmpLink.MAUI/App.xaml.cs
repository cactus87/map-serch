namespace LmpLink.MAUI;

public partial class App : Application
{
    public App()
    {
        MauiProgram.Log("=== App Constructor START ===");
        try
        {
            InitializeComponent();
            MauiProgram.Log("=== App Constructor SUCCESS ===");
        }
        catch (Exception ex)
        {
            MauiProgram.Log($"!!! App Constructor FAILED: {ex.Message}");
            MauiProgram.Log(ex.StackTrace ?? "");
            throw;
        }
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        MauiProgram.Log("=== App.CreateWindow START ===");
        try
        {
            MauiProgram.Log("[1] Creating AppShell...");
            var shell = new AppShell();
            MauiProgram.Log("[2] AppShell created");
            
            var window = new Window(shell)
            {
                Title = "LMP-Link MVP",
                Width = 1280,
                Height = 720
            };
            MauiProgram.Log("=== App.CreateWindow SUCCESS ===");
            return window;
        }
        catch (Exception ex)
        {
            MauiProgram.Log($"!!! App.CreateWindow FAILED: {ex.Message}");
            MauiProgram.Log(ex.StackTrace ?? "");
            throw;
        }
    }
}
