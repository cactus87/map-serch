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
        MauiProgram.Log($"[WebView] Navigating to: {e.Url}");
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        MauiProgram.Log("=== MainPage.OnAppearing START ===");

        if (_viewModel == null)
        {
            MauiProgram.Log("!!! ViewModel is null in OnAppearing");
            return;
        }

        // Auto-load data if empty
        if (_viewModel.Users.Count == 0)
        {
            MauiProgram.Log($"[OnAppearing] Loading data... (current count: {_viewModel.Users.Count})");
            await _viewModel.LoadDataCommand.ExecuteAsync(null);
            MauiProgram.Log($"[OnAppearing] Data loaded: {_viewModel.Users.Count} users, {_viewModel.Assistants.Count} assistants");
        }
        else
        {
            MauiProgram.Log($"[OnAppearing] Data already loaded: {_viewModel.Users.Count} users");
        }
        
        MauiProgram.Log("=== MainPage.OnAppearing DONE ===");
    }

    private async void OnWebViewNavigated(object? sender, WebNavigatedEventArgs e)
    {
        MauiProgram.Log($"=== OnWebViewNavigated START === Result: {e.Result}");
        
        if (e.Result != WebNavigationResult.Success)
        {
            MauiProgram.Log($"!!! WebView navigation FAILED: {e.Result}");
            return;
        }

        if (_mapService == null || _viewModel == null)
        {
            MauiProgram.Log($"!!! Services null: MapService={_mapService == null}, ViewModel={_viewModel == null}");
            return;
        }

        // Map HTML loaded successfully
        _isMapReady = true;
        MauiProgram.Log("[WebView] Navigation SUCCESS. Initializing map...");

        try
        {
            // Set WebView reference
            _mapService.SetWebView(MapWebView);
            MauiProgram.Log("[WebView] MapService.SetWebView() called");

            // Initialize map
            MauiProgram.Log("[WebView] Waiting 500ms for DOM ready...");
            await Task.Delay(500);
            
            MauiProgram.Log("[WebView] Calling MapService.InitMapAsync()...");
            await _mapService.InitMapAsync();
            MauiProgram.Log("[WebView] Map initialized!");

            // Add all markers
            var allPersons = _viewModel.Users.Concat(_viewModel.Assistants).ToList();
            MauiProgram.Log($"[WebView] Adding {allPersons.Count} markers...");
            await _mapService.AddMarkersAsync(allPersons);

            MauiProgram.Log("=== Map initialization SUCCESS ===");
        }
        catch (Exception ex)
        {
            MauiProgram.Log($"!!! Map initialization FAILED: {ex.Message}");
            MauiProgram.Log(ex.StackTrace ?? "");
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

                case nameof(MapViewModel.SelectedAssistant):
                    await HandleSelectedAssistantChanged();
                    break;

                case nameof(MapViewModel.CurrentRadius):
                    await HandleRadiusChanged();
                    break;

                case nameof(MapViewModel.FilteredAssistants):
                    await HandleFilteredAssistantsChanged();
                    break;

                case nameof(MapViewModel.FilteredUsers):
                    await HandleFilteredUsersChanged();
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
            var user = _viewModel.SelectedUser;
            
            // Focus on user marker
            await _mapService.FocusMarkerAsync(user.Id);

            // Draw circle if User-centric mode
            if (_viewModel.CurrentCenterMode == CenterMode.User)
            {
                await _mapService.DrawCircleAsync(user.Latitude, user.Longitude, _viewModel.CurrentRadius);
            }
        }
    }

    private async Task HandleSelectedAssistantChanged()
    {
        if (_viewModel == null || _mapService == null) return;

        if (_viewModel.SelectedAssistant == null)
        {
            // Clear circle if no assistant selected
            await _mapService.ClearCircleAsync();
            await _mapService.ShowAllMarkersAsync();
        }
        else
        {
            var assistant = _viewModel.SelectedAssistant;
            
            // Focus on assistant marker
            await _mapService.FocusMarkerAsync(assistant.Id);

            // Draw circle if Assistant-centric mode
            if (_viewModel.CurrentCenterMode == CenterMode.Assistant)
            {
                await _mapService.DrawCircleAsync(assistant.Latitude, assistant.Longitude, _viewModel.CurrentRadius);
            }
        }
    }

    private async Task HandleRadiusChanged()
    {
        if (_viewModel == null || _mapService == null) return;

        if (_viewModel.CurrentCenterMode == CenterMode.User && _viewModel.SelectedUser != null)
        {
            // Redraw circle with new radius (User-centric)
            var user = _viewModel.SelectedUser;
            await _mapService.DrawCircleAsync(user.Latitude, user.Longitude, _viewModel.CurrentRadius);
        }
        else if (_viewModel.CurrentCenterMode == CenterMode.Assistant && _viewModel.SelectedAssistant != null)
        {
            // Redraw circle with new radius (Assistant-centric)
            var assistant = _viewModel.SelectedAssistant;
            await _mapService.DrawCircleAsync(assistant.Latitude, assistant.Longitude, _viewModel.CurrentRadius);
        }
    }

    private async Task HandleFilteredAssistantsChanged()
    {
        if (_viewModel == null || _mapService == null) return;

        // Only filter markers if in User-centric mode
        if (_viewModel.CurrentCenterMode == CenterMode.User)
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
    }

    private async Task HandleFilteredUsersChanged()
    {
        if (_viewModel == null || _mapService == null) return;

        // Only filter markers if in Assistant-centric mode
        if (_viewModel.CurrentCenterMode == CenterMode.Assistant)
        {
            // Update marker visibility based on filtered results
            var filteredIds = _viewModel.FilteredUsers.Select(u => u.Id).ToHashSet();

            foreach (var user in _viewModel.Users)
            {
                var isVisible = filteredIds.Contains(user.Id);
                await _mapService.SetMarkerVisibleAsync(user.Id, isVisible);
            }

            // Always show assistant markers
            foreach (var assistant in _viewModel.Assistants)
            {
                await _mapService.SetMarkerVisibleAsync(assistant.Id, true);
            }
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
