namespace LmpLink.MAUI.Views.Pages;

public partial class ManagementPage : ContentPage
{
    private ManagementViewModel? _viewModel;

    public ManagementPage()
    {
        MauiProgram.Log("=== ManagementPage Constructor START ===");
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

            _viewModel = services.GetRequiredService<ManagementViewModel>();
            BindingContext = _viewModel;

            MauiProgram.Log("=== ManagementPage Constructor SUCCESS ===");
        }
        catch (Exception ex)
        {
            MauiProgram.Log("!!! ManagementPage Constructor FAILED");
            MauiProgram.Log($"Exception: {ex.Message}");
            MauiProgram.Log($"StackTrace: {ex.StackTrace}");
            throw;
        }
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        MauiProgram.Log("=== ManagementPage.OnAppearing START ===");

        if (_viewModel != null)
        {
            await _viewModel.LoadDataCommand.ExecuteAsync(null);
        }

        MauiProgram.Log("=== ManagementPage.OnAppearing DONE ===");
    }
}


