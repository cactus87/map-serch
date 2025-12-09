#nullable enable

using LmpLink.MAUI.ViewModels.Base;

namespace LmpLink.MAUI.ViewModels;

/// <summary>
/// Center mode for filtering: User-centric or Assistant-centric.
/// </summary>
public enum CenterMode
{
    User,      // 이용자 중심 - 이용자 선택 시 활동지원사 필터링
    Assistant  // 활동지원사 중심 - 활동지원사 선택 시 이용자 필터링
}

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
    private CenterMode _currentCenterMode = CenterMode.User; // Default: User-centric

    [ObservableProperty]
    private ObservableCollection<Person> _users = new();

    [ObservableProperty]
    private ObservableCollection<Person> _assistants = new();

    [ObservableProperty]
    private ObservableCollection<Person> _filteredAssistants = new();

    [ObservableProperty]
    private ObservableCollection<Person> _filteredUsers = new(); // For Assistant-centric mode

    [ObservableProperty]
    private Person? _selectedUser;

    [ObservableProperty]
    private Person? _selectedAssistant;

    [ObservableProperty]
    private double _currentRadius = 3.0; // Default 3km

    [ObservableProperty]
    private string _filterStatusText = "전체 보기";

    [ObservableProperty]
    private int _usersInRadius; // For map overlay

    [ObservableProperty]
    private int _assistantsInRadius; // For map overlay

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
            FilteredUsers = new ObservableCollection<Person>(users);

            // Initialize counts
            UsersInRadius = users.Count;
            AssistantsInRadius = assistants.Count;

            UpdateFilterStatusText();
        });
    }

    /// <summary>
    /// Toggle between User-centric and Assistant-centric mode.
    /// </summary>
    [RelayCommand]
    private void ToggleCenterMode()
    {
        CurrentCenterMode = CurrentCenterMode == CenterMode.User 
            ? CenterMode.Assistant 
            : CenterMode.User;

        // Clear selections when switching mode
        if (CurrentCenterMode == CenterMode.User)
        {
            SelectedAssistant = null;
            FilteredUsers = new ObservableCollection<Person>(Users);
        }
        else
        {
            SelectedUser = null;
            FilteredAssistants = new ObservableCollection<Person>(Assistants);
        }

        UpdateFilterStatusText();
    }

    /// <summary>
    /// Select a user (e.g., from marker click or list tap).
    /// Behavior depends on current center mode.
    /// </summary>
    [RelayCommand]
    private void SelectUser(Person? user)
    {
        SelectedUser = user;

        if (SelectedUser != null && CurrentCenterMode == CenterMode.User)
        {
            // User-centric mode: Filter assistants by radius
            FilterAssistantsByRadius();
        }
    }

    /// <summary>
    /// Select an assistant (e.g., from marker click or list tap).
    /// Behavior depends on current center mode.
    /// </summary>
    [RelayCommand]
    private void SelectAssistant(Person? assistant)
    {
        SelectedAssistant = assistant;

        if (SelectedAssistant != null && CurrentCenterMode == CenterMode.Assistant)
        {
            // Assistant-centric mode: Filter users by radius
            FilterUsersByRadius();
        }
    }

    /// <summary>
    /// Change radius (1km/3km/5km) and re-filter.
    /// </summary>
    [RelayCommand]
    private void ChangeRadius(double radiusKm)
    {
        CurrentRadius = radiusKm;

        if (CurrentCenterMode == CenterMode.User && SelectedUser != null)
        {
            FilterAssistantsByRadius();
        }
        else if (CurrentCenterMode == CenterMode.Assistant && SelectedAssistant != null)
        {
            FilterUsersByRadius();
        }
        else
        {
            // No selection, select first person automatically
            if (CurrentCenterMode == CenterMode.User)
            {
                var firstUser = Users.FirstOrDefault();
                if (firstUser != null) SelectUser(firstUser);
            }
            else
            {
                var firstAssistant = Assistants.FirstOrDefault();
                if (firstAssistant != null) SelectAssistant(firstAssistant);
            }
        }
    }

    /// <summary>
    /// Show all persons (clear filter).
    /// </summary>
    [RelayCommand]
    private void ShowAll()
    {
        CurrentRadius = 0; // 0 = show all
        SelectedUser = null;
        SelectedAssistant = null;
        FilteredAssistants = new ObservableCollection<Person>(Assistants);
        FilteredUsers = new ObservableCollection<Person>(Users);
        UsersInRadius = Users.Count;
        AssistantsInRadius = Assistants.Count;
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
            AssistantsInRadius = Assistants.Count;
            UpdateFilterStatusText();
            return;
        }

        if (CurrentRadius <= 0)
        {
            // Show all
            FilteredAssistants = new ObservableCollection<Person>(Assistants);
            AssistantsInRadius = Assistants.Count;
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
            AssistantsInRadius = filtered.Count;
        }

        UsersInRadius = 1; // Current selected user
        UpdateFilterStatusText();
    }

    /// <summary>
    /// Filter users by current radius from selected assistant.
    /// Sorts by distance (ascending).
    /// (Reverse filtering for Assistant-centric mode)
    /// </summary>
    private void FilterUsersByRadius()
    {
        if (SelectedAssistant == null)
        {
            FilteredUsers = new ObservableCollection<Person>(Users);
            UsersInRadius = Users.Count;
            UpdateFilterStatusText();
            return;
        }

        if (CurrentRadius <= 0)
        {
            // Show all
            FilteredUsers = new ObservableCollection<Person>(Users);
            UsersInRadius = Users.Count;
        }
        else
        {
            // Filter by radius (reverse: Assistant → Users)
            var filtered = _locationService.FilterByRadius(
                SelectedAssistant,
                Users.ToList(),
                CurrentRadius
            );

            FilteredUsers = new ObservableCollection<Person>(filtered);
            UsersInRadius = filtered.Count;
        }

        AssistantsInRadius = 1; // Current selected assistant
        UpdateFilterStatusText();
    }

    /// <summary>
    /// Update filter status text (e.g., "1km 내 12명", "전체 20명").
    /// </summary>
    private void UpdateFilterStatusText()
    {
        if (CurrentCenterMode == CenterMode.User)
        {
            // User-centric mode
            if (SelectedUser == null || CurrentRadius <= 0)
            {
                FilterStatusText = $"전체 {FilteredAssistants.Count}명";
            }
            else
            {
                FilterStatusText = $"{CurrentRadius}km 내 {FilteredAssistants.Count}명";
            }
        }
        else
        {
            // Assistant-centric mode
            if (SelectedAssistant == null || CurrentRadius <= 0)
            {
                FilterStatusText = $"전체 {FilteredUsers.Count}명";
            }
            else
            {
                FilterStatusText = $"{CurrentRadius}km 내 {FilteredUsers.Count}명";
            }
        }
    }
}
