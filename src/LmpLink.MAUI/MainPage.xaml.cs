namespace LmpLink.MAUI;

public partial class MainPage : ContentPage
{
    private readonly MapViewModel _viewModel;
    private readonly IMapService _mapService;
    private bool _isMapReady;

    public MainPage(MapViewModel viewModel, IMapService mapService)
    {
        InitializeComponent();

        _viewModel = viewModel;
        _mapService = mapService;
        BindingContext = _viewModel;

        // Subscribe to ViewModel property changes
        _viewModel.PropertyChanged += OnViewModelPropertyChanged;

        // Subscribe to WebView Navigated event
        MapWebView.Navigated += OnWebViewNavigated;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        // Auto-load data if empty
        if (_viewModel.Users.Count == 0)
        {
            await _viewModel.LoadDataCommand.ExecuteAsync(null);
        }
    }

    private async void OnWebViewNavigated(object? sender, WebNavigatedEventArgs e)
    {
        if (e.Result != WebNavigationResult.Success)
        {
            System.Diagnostics.Debug.WriteLine($"WebView navigation failed: {e.Result}");
            return;
        }

        // Map HTML loaded successfully
        _isMapReady = true;
        System.Diagnostics.Debug.WriteLine("WebView navigation success. Initializing map...");

        try
        {
            // Set WebView reference
            _mapService.SetWebView(MapWebView);

            // Initialize map
            await Task.Delay(500); // Wait for DOM ready
            await _mapService.InitMapAsync();

            // Add all markers
            var allPersons = _viewModel.Users.Concat(_viewModel.Assistants);
            await _mapService.AddMarkersAsync(allPersons);

            System.Diagnostics.Debug.WriteLine("Map initialized successfully.");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Map initialization failed: {ex.Message}");
        }
    }

    private async void OnViewModelPropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (!_isMapReady) return;

        try
        {
            switch (e.PropertyName)
            {
                case nameof(MapViewModel.SelectedUser):
                    await HandleSelectedUserChanged();
                    break;

                case nameof(MapViewModel.CurrentRadius):
                    await HandleRadiusChanged();
                    break;

                case nameof(MapViewModel.FilteredAssistants):
                    await HandleFilteredAssistantsChanged();
                    break;
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"PropertyChanged handler error: {ex.Message}");
        }
    }

    private async Task HandleSelectedUserChanged()
    {
        if (_viewModel.SelectedUser == null)
        {
            // Clear circle if no user selected
            await _mapService.ClearCircleAsync();
            await _mapService.ShowAllMarkersAsync();
        }
        else
        {
            // Draw circle around selected user
            var user = _viewModel.SelectedUser;
            await _mapService.DrawCircleAsync(user.Latitude, user.Longitude, _viewModel.CurrentRadius);

            // Set map center to selected user
            await _mapService.SetMapCenterAsync(user.Latitude, user.Longitude);
        }
    }

    private async Task HandleRadiusChanged()
    {
        if (_viewModel.SelectedUser != null)
        {
            // Redraw circle with new radius
            var user = _viewModel.SelectedUser;
            await _mapService.DrawCircleAsync(user.Latitude, user.Longitude, _viewModel.CurrentRadius);
        }
    }

    private async Task HandleFilteredAssistantsChanged()
    {
        // Update marker visibility based on filtered results
        var filteredIds = _viewModel.FilteredAssistants.Select(a => a.Id).ToHashSet();

        foreach (var assistant in _viewModel.Assistants)
        {
            var isVisible = filteredIds.Contains(assistant.Id);
            await _mapService.SetMarkerVisibleAsync(assistant.Id, isVisible);
        }

        // Always show user markers
        foreach (var user in _viewModel.Users)
        {
            await _mapService.SetMarkerVisibleAsync(user.Id, true);
        }
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();

        // Unsubscribe
        _viewModel.PropertyChanged -= OnViewModelPropertyChanged;
        MapWebView.Navigated -= OnWebViewNavigated;
    }
}
