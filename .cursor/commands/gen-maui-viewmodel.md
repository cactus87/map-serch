# Slash Command: /maui-viewmodel

Generate a new MAUI ViewModel with MVVM Toolkit (CommunityToolkit.Mvvm).

## Usage

```
/maui-viewmodel MapViewModel
/maui-viewmodel MatchingViewModel --with-collection
/maui-viewmodel SettingsViewModel --simple
```

## Output Structure

### ViewModels/MapViewModel.cs

```csharp
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using LmpLink.MAUI.Models;
using LmpLink.MAUI.Services;
using System.Collections.ObjectModel;

#nullable enable

namespace LmpLink.MAUI.ViewModels;

/// <summary>
/// ViewModel for MapPage.
/// Handles map markers, radius filtering, and user selection.
/// </summary>
public partial class MapViewModel : BaseViewModel
{
    private readonly ILocationService _locationService;
    private readonly IApiService _apiService;

    [ObservableProperty]
    private ObservableCollection<Person> items = [];

    [ObservableProperty]
    private Person? selectedItem;

    [ObservableProperty]
    private string searchQuery = string.Empty;

    [ObservableProperty]
    private bool isRefreshing;

    public MapViewModel(
        ILocationService locationService,
        IApiService apiService,
        IMessenger messenger)
        : base(messenger)
    {
        _locationService = locationService;
        _apiService = apiService;
    }

    /// <summary>
    /// Load data from API.
    /// </summary>
    [RelayCommand]
    public async Task LoadData()
    {
        await ExecuteAsync(async () =>
        {
            var data = await _apiService.GetPersonsAsync();
            Items = new ObservableCollection<Person>(data);
        });
    }

    /// <summary>
    /// Refresh data (pull-to-refresh).
    /// </summary>
    [RelayCommand]
    public async Task Refresh()
    {
        IsRefreshing = true;
        try
        {
            await LoadDataCommand.ExecuteAsync(null);
        }
        finally
        {
            IsRefreshing = false;
        }
    }

    /// <summary>
    /// Select an item.
    /// </summary>
    [RelayCommand]
    public void SelectItem(Person item)
    {
        SelectedItem = item;
    }

    /// <summary>
    /// Search items by query.
    /// </summary>
    partial void OnSearchQueryChanged(string value)
    {
        // Filter logic here
    }
}
```

## Rules

1. **Always inherit from BaseViewModel**: Provides `IsLoading`, `ErrorMessage`, `ExecuteAsync` wrapper
2. **Use `#nullable enable`**: Required at the top of every file
3. **Use `[ObservableProperty]`**: Auto-generates property changed events
4. **Use `[RelayCommand]`**: Auto-generates Command properties (e.g., `LoadDataCommand`)
5. **Constructor Injection**: Inject services via constructor, store in private fields
6. **Async Commands**: Use `async Task` for all async operations
7. **Error Handling**: Wrap async calls in `ExecuteAsync()` for automatic error handling
8. **Collections**: Use `ObservableCollection<T>` for data binding
9. **XML Comments**: Add `/// <summary>` for all public methods/properties

## Notes

- **BaseViewModel** provides: `IsLoading`, `ErrorMessage`, `HasError`, `ExecuteAsync`, `Messenger`
- **MVVM Toolkit** auto-generates: `OnPropertyChanged`, `OnPropertyChanging`, Commands
- **Messenger** for cross-ViewModel communication (WeakReferenceMessenger pattern)
- **Register in MauiProgram.cs**: `builder.Services.AddSingleton<MapViewModel>()`

## Example Registration (MauiProgram.cs)

```csharp
// ViewModels
builder.Services
    .AddSingleton<MapViewModel>()
    .AddSingleton<MatchingViewModel>();

// Pages
builder.Services
    .AddSingleton<MapPage>()
    .AddSingleton<MatchingPage>();
```

