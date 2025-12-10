namespace LmpLink.MAUI.Views.Pages;

public partial class DataPage : ContentPage
{
    public DataPage()
    {
        MauiProgram.Log("=== DataPage Constructor START ===");
        try
        {
            InitializeComponent();
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

