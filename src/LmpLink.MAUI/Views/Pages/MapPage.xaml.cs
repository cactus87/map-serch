namespace LmpLink.MAUI.Views.Pages;

public partial class MapPage : ContentPage
{
    private MapViewModel? _viewModel;
    private IMapService? _mapService;
    private bool _isMapReady;

    public MapPage()
    {
        MauiProgram.Log("=== MapPage Constructor START ===");
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
            MauiProgram.Log("=== MapPage Constructor SUCCESS ===");
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
        MauiProgram.Log("=== MapPage.OnAppearing START ===");

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
        
        MauiProgram.Log("=== MapPage.OnAppearing DONE ===");
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

        MauiProgram.Log($"[HandleSelectedUserChanged] User: {_viewModel.SelectedUser?.Name ?? "null"}, Mode: {_viewModel.CurrentCenterMode}");

        if (_viewModel.SelectedUser == null)
        {
            // Clear circle if no user selected
            await _mapService.ClearCircleAsync();
            await _mapService.ShowAllMarkersAsync();
            await UpdateMarkerCountsFromMap();
        }
        else
        {
            var user = _viewModel.SelectedUser;
            
            // Focus on user marker
            await _mapService.FocusMarkerAsync(user.Id);

            // User-centric mode: draw circle and update visibility immediately
            if (_viewModel.CurrentCenterMode == CenterMode.User)
            {
                await _mapService.DrawCircleAsync(user.Latitude, user.Longitude, _viewModel.CurrentRadius);
                
                // Update marker visibility directly here (don't wait for FilteredAssistants change)
                var filteredIds = _viewModel.FilteredAssistants.Select(a => a.Id).ToHashSet();
                
                foreach (var assistant in _viewModel.Assistants)
                {
                    var isVisible = filteredIds.Contains(assistant.Id);
                    await _mapService.SetMarkerVisibleAsync(assistant.Id, isVisible);
                }
                
                // Show only selected user
                foreach (var u in _viewModel.Users)
                {
                    await _mapService.SetMarkerVisibleAsync(u.Id, u.Id == user.Id);
                }
                
                await UpdateMarkerCountsFromMap();
            }
            // In Assistant mode: show clicked user + selected assistant + filtered users
            else
            {
                // Make clicked user visible (even if outside radius)
                await _mapService.SetMarkerVisibleAsync(user.Id, true);
                await UpdateMarkerCountsFromMap();
            }
        }
    }

    private async Task HandleSelectedAssistantChanged()
    {
        if (_viewModel == null || _mapService == null) return;

        MauiProgram.Log($"[HandleSelectedAssistantChanged] Assistant: {_viewModel.SelectedAssistant?.Name ?? "null"}, Mode: {_viewModel.CurrentCenterMode}");

        if (_viewModel.SelectedAssistant == null)
        {
            // Clear circle if no assistant selected
            await _mapService.ClearCircleAsync();
            await _mapService.ShowAllMarkersAsync();
            await UpdateMarkerCountsFromMap();
        }
        else
        {
            var assistant = _viewModel.SelectedAssistant;
            
            // Focus on assistant marker
            await _mapService.FocusMarkerAsync(assistant.Id);

            // Assistant-centric mode: draw circle and update visibility immediately
            if (_viewModel.CurrentCenterMode == CenterMode.Assistant)
            {
                await _mapService.DrawCircleAsync(assistant.Latitude, assistant.Longitude, _viewModel.CurrentRadius);
                
                // Update marker visibility directly here (don't wait for FilteredUsers change)
                var filteredIds = _viewModel.FilteredUsers.Select(u => u.Id).ToHashSet();
                
                foreach (var user in _viewModel.Users)
                {
                    var isVisible = filteredIds.Contains(user.Id);
                    await _mapService.SetMarkerVisibleAsync(user.Id, isVisible);
                }
                
                // Show only selected assistant
                foreach (var a in _viewModel.Assistants)
                {
                    await _mapService.SetMarkerVisibleAsync(a.Id, a.Id == assistant.Id);
                }
                
                await UpdateMarkerCountsFromMap();
            }
            // In User mode: show clicked assistant + selected user + filtered assistants
            else
            {
                // Make clicked assistant visible (even if outside radius)
                await _mapService.SetMarkerVisibleAsync(assistant.Id, true);
                await UpdateMarkerCountsFromMap();
            }
        }
    }

    private async Task HandleRadiusChanged()
    {
        if (_viewModel == null || _mapService == null) return;

        MauiProgram.Log($"[HandleRadiusChanged] Radius: {_viewModel.CurrentRadius}km");

        if (_viewModel.CurrentCenterMode == CenterMode.User && _viewModel.SelectedUser != null)
        {
            // Redraw circle with new radius (User-centric)
            var user = _viewModel.SelectedUser;
            await _mapService.DrawCircleAsync(user.Latitude, user.Longitude, _viewModel.CurrentRadius);
            await UpdateMarkerCountsFromMap();
        }
        else if (_viewModel.CurrentCenterMode == CenterMode.Assistant && _viewModel.SelectedAssistant != null)
        {
            // Redraw circle with new radius (Assistant-centric)
            var assistant = _viewModel.SelectedAssistant;
            await _mapService.DrawCircleAsync(assistant.Latitude, assistant.Longitude, _viewModel.CurrentRadius);
            await UpdateMarkerCountsFromMap();
        }
    }

    private async Task HandleFilteredAssistantsChanged()
    {
        // Visibility is now handled in HandleSelectedUserChanged to avoid race conditions
        // This handler is kept for radius change updates only
        if (_viewModel == null || _mapService == null) return;
        if (_viewModel.CurrentCenterMode != CenterMode.User) return;
        if (_viewModel.SelectedUser == null) return;

        MauiProgram.Log($"[HandleFilteredAssistantsChanged] Count: {_viewModel.FilteredAssistants.Count}");

        var filteredIds = _viewModel.FilteredAssistants.Select(a => a.Id).ToHashSet();
        var selectedUserId = _viewModel.SelectedUser.Id;

        foreach (var assistant in _viewModel.Assistants)
        {
            var isVisible = filteredIds.Contains(assistant.Id);
            await _mapService.SetMarkerVisibleAsync(assistant.Id, isVisible);
        }

        foreach (var user in _viewModel.Users)
        {
            await _mapService.SetMarkerVisibleAsync(user.Id, user.Id == selectedUserId);
        }

        await UpdateMarkerCountsFromMap();
    }

    private async Task HandleFilteredUsersChanged()
    {
        // Visibility is now handled in HandleSelectedAssistantChanged to avoid race conditions
        // This handler is kept for radius change updates only
        if (_viewModel == null || _mapService == null) return;
        if (_viewModel.CurrentCenterMode != CenterMode.Assistant) return;
        if (_viewModel.SelectedAssistant == null) return;

        MauiProgram.Log($"[HandleFilteredUsersChanged] Count: {_viewModel.FilteredUsers.Count}");

        var filteredIds = _viewModel.FilteredUsers.Select(u => u.Id).ToHashSet();
        var selectedAssistantId = _viewModel.SelectedAssistant.Id;

        foreach (var user in _viewModel.Users)
        {
            var isVisible = filteredIds.Contains(user.Id);
            await _mapService.SetMarkerVisibleAsync(user.Id, isVisible);
        }

        foreach (var assistant in _viewModel.Assistants)
        {
            await _mapService.SetMarkerVisibleAsync(assistant.Id, assistant.Id == selectedAssistantId);
        }

        await UpdateMarkerCountsFromMap();
    }

    private async Task UpdateMarkerCountsFromMap()
    {
        if (_viewModel == null || _mapService == null) return;

        try
        {
            // Small delay to ensure marker visibility changes are applied
            await Task.Delay(100);

            // Get actual visible marker count from JavaScript
            var (users, assistants) = await _mapService.GetVisibleMarkerCountAsync();
            
            // Update ViewModel with accurate counts
            _viewModel.UsersInRadius = users;
            _viewModel.AssistantsInRadius = assistants;

            MauiProgram.Log($"✅ Marker count updated: 이용자 {users}명, 지원사 {assistants}명");
        }
        catch (Exception ex)
        {
            MauiProgram.Log($"❌ Failed to update marker counts: {ex.Message}");
            MauiProgram.Log(ex.StackTrace ?? "");
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
