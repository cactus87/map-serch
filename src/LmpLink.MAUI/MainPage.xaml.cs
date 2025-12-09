namespace LmpLink.MAUI;

public partial class MainPage : ContentPage
{
    private MapViewModel? _viewModel;
    private IMapService? _mapService;
    private bool _isMapReady;

    public MainPage()
    {
        MauiProgram.Log("=== MainPage Constructor START ===");
        try
        {
            MauiProgram.Log("[1] Calling InitializeComponent...");
            InitializeComponent();
            MauiProgram.Log("[2] InitializeComponent done");

            // Get services from DI container
            MauiProgram.Log("[3] Getting services...");
            var services = IPlatformApplication.Current?.Services;
            if (services == null)
            {
                MauiProgram.Log("!!! Services not available - returning early");
                return;
            }
            MauiProgram.Log("[4] Services obtained");

            MauiProgram.Log("[5] Getting MapViewModel...");
            _viewModel = services.GetRequiredService<MapViewModel>();
            MauiProgram.Log("[6] MapViewModel obtained");
            
            MauiProgram.Log("[7] Getting IMapService...");
            _mapService = services.GetRequiredService<IMapService>();
            MauiProgram.Log("[8] IMapService obtained");
            
            BindingContext = _viewModel;
            MauiProgram.Log("[9] BindingContext set");

            // Subscribe to ViewModel property changes
            _viewModel.PropertyChanged += OnViewModelPropertyChanged;
            MauiProgram.Log("[10] PropertyChanged subscribed");

            // Subscribe to WebView events
            MauiProgram.Log("[11] Subscribing to WebView events...");
            MapWebView.Navigated += OnWebViewNavigated;
            MapWebView.Navigating += OnWebViewNavigating;
            MauiProgram.Log("=== MainPage Constructor SUCCESS ===");
        }
        catch (Exception ex)
        {
            MauiProgram.Log($"!!! MainPage Constructor FAILED: {ex.Message}");
            MauiProgram.Log(ex.StackTrace ?? "");
            throw;
        }
    }

    private void OnWebViewNavigating(object? sender, WebNavigatingEventArgs e)
    {
        System.Diagnostics.Debug.WriteLine($"WebView navigating to: {e.Url}");
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (_viewModel == null) return;

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

        if (_mapService == null || _viewModel == null) return;

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
        if (!_isMapReady || _viewModel == null || _mapService == null) return;

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
        if (_viewModel == null || _mapService == null) return;

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
        if (_viewModel == null || _mapService == null) return;

        if (_viewModel.SelectedUser != null)
        {
            // Redraw circle with new radius
            var user = _viewModel.SelectedUser;
            await _mapService.DrawCircleAsync(user.Latitude, user.Longitude, _viewModel.CurrentRadius);
        }
    }

    private async Task HandleFilteredAssistantsChanged()
    {
        if (_viewModel == null || _mapService == null) return;

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
        if (_viewModel != null)
        {
            _viewModel.PropertyChanged -= OnViewModelPropertyChanged;
        }
        MapWebView.Navigated -= OnWebViewNavigated;
        MapWebView.Navigating -= OnWebViewNavigating;
    }
}
