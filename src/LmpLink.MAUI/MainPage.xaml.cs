namespace LmpLink.MAUI;

public partial class MainPage : ContentPage
{
    private readonly MapViewModel _viewModel;

    public MainPage(MapViewModel viewModel)
    {
        InitializeComponent();
        
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        // Auto-load data on page appear
        if (_viewModel.Users.Count == 0)
        {
            await _viewModel.LoadDataCommand.ExecuteAsync(null);
        }
    }
}
