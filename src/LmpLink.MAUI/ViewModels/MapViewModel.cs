#nullable enable

using LmpLink.MAUI.ViewModels.Base;

namespace LmpLink.MAUI.ViewModels;

/// <summary>
/// ViewModel for the main map page.
/// Manages users, assistants, filtering by radius.
/// 
/// Converted from Blazor Home.razor @code block:
/// - private List<Person> filteredPeople → ObservableCollection<Person> FilteredAssistants
/// - private Person? selectedUser → Person? SelectedUser (ObservableProperty)
/// - private double currentRadius → double CurrentRadius (ObservableProperty)
/// - FilterByRadius(double radiusKm) → ChangeRadiusCommand (RelayCommand)
/// - ShowAll() → ShowAllCommand (RelayCommand)
/// </summary>
public partial class MapViewModel : BaseViewModel
{
    private readonly IMockDataService _mockDataService;
    private readonly ILocationService _locationService;

    // --- Observable Properties ---

    [ObservableProperty]
    private ObservableCollection<Person> _users = new();

    [ObservableProperty]
    private ObservableCollection<Person> _assistants = new();

    [ObservableProperty]
    private ObservableCollection<Person> _filteredAssistants = new();

    [ObservableProperty]
    private Person? _selectedUser;

    [ObservableProperty]
    private double _currentRadius = 3.0; // Default 3km

    [ObservableProperty]
    private string _filterStatusText = "전체 보기";

    // --- Constructor ---

    /// <summary>
    /// Constructor with DI.
    /// </summary>
    public MapViewModel(IMockDataService mockDataService, ILocationService locationService)
    {
        _mockDataService = mockDataService;
        _locationService = locationService;
    }

    // --- Commands ---

    /// <summary>
    /// Load users and assistants from MockDataService.
    /// </summary>
    [RelayCommand]
    private async Task LoadDataAsync()
    {
        await ExecuteAsync(async () =>
        {
            var users = await _mockDataService.GetUsersAsync();
            var assistants = await _mockDataService.GetAssistantsAsync();

            Users = new ObservableCollection<Person>(users);
            Assistants = new ObservableCollection<Person>(assistants);
            FilteredAssistants = new ObservableCollection<Person>(assistants);

            UpdateFilterStatusText();
        });
    }

    /// <summary>
    /// Select a user (e.g., from marker click or list tap).
    /// Automatically filters assistants by current radius.
    /// </summary>
    [RelayCommand]
    private void SelectUser(Person? user)
    {
        SelectedUser = user;

        if (SelectedUser != null)
        {
            // Filter assistants by current radius
            FilterAssistantsByRadius();
        }
    }

    /// <summary>
    /// Change radius (1km/3km/5km) and re-filter.
    /// </summary>
    [RelayCommand]
    private void ChangeRadius(double radiusKm)
    {
        CurrentRadius = radiusKm;

        if (SelectedUser != null)
        {
            FilterAssistantsByRadius();
        }
        else
        {
            // No user selected, select first user automatically
            var firstUser = Users.FirstOrDefault();
            if (firstUser != null)
            {
                SelectUser(firstUser);
            }
        }
    }

    /// <summary>
    /// Show all assistants (clear filter).
    /// </summary>
    [RelayCommand]
    private void ShowAll()
    {
        CurrentRadius = 0; // 0 = show all
        SelectedUser = null;
        FilteredAssistants = new ObservableCollection<Person>(Assistants);
        UpdateFilterStatusText();
    }

    // --- Private Methods ---

    /// <summary>
    /// Filter assistants by current radius from selected user.
    /// Sorts by distance (ascending).
    /// </summary>
    private void FilterAssistantsByRadius()
    {
        if (SelectedUser == null)
        {
            FilteredAssistants = new ObservableCollection<Person>(Assistants);
            UpdateFilterStatusText();
            return;
        }

        if (CurrentRadius <= 0)
        {
            // Show all
            FilteredAssistants = new ObservableCollection<Person>(Assistants);
        }
        else
        {
            // Filter by radius
            var filtered = _locationService.FilterByRadius(
                SelectedUser,
                Assistants.ToList(),
                CurrentRadius
            );

            FilteredAssistants = new ObservableCollection<Person>(filtered);
        }

        UpdateFilterStatusText();
    }

    /// <summary>
    /// Update filter status text (e.g., "1km 내 12명", "전체 20명").
    /// </summary>
    private void UpdateFilterStatusText()
    {
        if (SelectedUser == null)
        {
            FilterStatusText = $"전체 {FilteredAssistants.Count}명";
        }
        else if (CurrentRadius <= 0)
        {
            FilterStatusText = $"전체 {FilteredAssistants.Count}명";
        }
        else
        {
            FilterStatusText = $"{CurrentRadius}km 내 {FilteredAssistants.Count}명";
        }
    }
}
