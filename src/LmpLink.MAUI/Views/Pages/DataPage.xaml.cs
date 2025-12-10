namespace LmpLink.MAUI.Views.Pages;

public partial class DataPage : ContentPage
{
    private DataViewModel? _viewModel;

    public DataPage()
    {
        MauiProgram.Log("=== DataPage Constructor START ===");
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

            _viewModel = services.GetRequiredService<DataViewModel>();
            BindingContext = _viewModel;

            MauiProgram.Log("=== DataPage Constructor SUCCESS ===");
        }
        catch (Exception ex)
        {
            MauiProgram.Log("!!! DataPage Constructor FAILED");
            MauiProgram.Log($"Exception: {ex.Message}");
            MauiProgram.Log($"StackTrace: {ex.StackTrace}");
            throw;
        }
    }
}


