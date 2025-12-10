namespace LmpLink.MAUI.Views.Pages;

public partial class SettingsPage : ContentPage
{
    private SettingsViewModel? _viewModel;

    public SettingsPage()
    {
        MauiProgram.Log("=== SettingsPage Constructor START ===");
        try
        {
            InitializeComponent();

            // Get ViewModel from DI
            var services = IPlatformApplication.Current?.Services;
            if (services == null)
            {
                MauiProgram.Log("!!! Services not available - returning early");
                return;
            }

            _viewModel = services.GetRequiredService<SettingsViewModel>();
            BindingContext = _viewModel;

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


