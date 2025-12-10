namespace LmpLink.MAUI.Views.Pages;

public partial class SettingsPage : ContentPage
{
    public SettingsPage()
    {
        MauiProgram.Log("=== SettingsPage Constructor START ===");
        try
        {
            InitializeComponent();
            MauiProgram.Log("=== SettingsPage Constructor SUCCESS ===");
        }
        catch (Exception ex)
        {
            MauiProgram.Log("!!! SettingsPage Constructor FAILED");
            MauiProgram.Log($"Exception: {ex.Message}");
            MauiProgram.Log($"StackTrace: {ex.StackTrace}");
            throw;
        }
    }
}

