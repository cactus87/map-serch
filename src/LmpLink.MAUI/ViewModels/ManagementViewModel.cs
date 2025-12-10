#nullable enable

using LmpLink.MAUI.ViewModels.Base;
using System.Collections.ObjectModel;

namespace LmpLink.MAUI.ViewModels;

/// <summary>
/// ViewModel for ManagementPage (CRUD for users and assistants).
/// </summary>
public partial class ManagementViewModel : BaseViewModel
{
    private readonly IMockDataService _mockDataService;
    private readonly ISupabaseService _supabaseService;

    // --- Observable Properties ---

    [ObservableProperty]
    private ObservableCollection<Person> _allPersons = new();

    [ObservableProperty]
    private ObservableCollection<Person> _filteredPersons = new();

    [ObservableProperty]
    private Person? _selectedPerson;

    [ObservableProperty]
    private string _searchText = string.Empty;

    [ObservableProperty]
    private string _selectedTypeFilter = "all"; // "all", "user", "assistant" - for UI binding

    // --- Constructor ---

    public ManagementViewModel(
        IMockDataService mockDataService,
        ISupabaseService supabaseService)
    {
        _mockDataService = mockDataService;
        _supabaseService = supabaseService;
        MauiProgram.Log("[ManagementViewModel] Constructor called");
    }

    // --- Commands ---

    /// <summary>
    /// Load all persons from Supabase (or mock data).
    /// </summary>
    [RelayCommand]
    private async Task LoadDataAsync()
    {
        await ExecuteAsync(async () =>
        {
            MauiProgram.Log("[ManagementViewModel] LoadDataAsync START");
            List<Person> users;
            List<Person> assistants;

            try
            {
                MauiProgram.Log("[ManagementViewModel] Attempting Supabase connection...");
                await _supabaseService.InitializeAsync();
                users = await _supabaseService.GetUsersAsync();
                assistants = await _supabaseService.GetAssistantsAsync();
                MauiProgram.Log($"[ManagementViewModel] Supabase SUCCESS: {users.Count} users, {assistants.Count} assistants");
            }
            catch (Exception ex)
            {
                MauiProgram.Log($"[ManagementViewModel] Supabase FAILED: {ex.Message}. Falling back to Mock data.");
                users = await _mockDataService.GetUsersAsync();
                assistants = await _mockDataService.GetAssistantsAsync();
                MauiProgram.Log($"[ManagementViewModel] Loaded from Mock: {users.Count} users, {assistants.Count} assistants");
            }

            AllPersons.Clear();
            foreach (var user in users)
            {
                AllPersons.Add(user);
            }
            foreach (var assistant in assistants)
            {
                AllPersons.Add(assistant);
            }

            ApplyFilter();
            MauiProgram.Log("[ManagementViewModel] LoadDataAsync END");
        });
    }

    /// <summary>
    /// Apply search and type filter.
    /// </summary>
    [RelayCommand]
    private void ApplyFilter(string? typeFilter = null)
    {
        // Update type filter if provided
        if (!string.IsNullOrEmpty(typeFilter))
        {
            SelectedTypeFilter = typeFilter;
        }

        MauiProgram.Log($"[ManagementViewModel] ApplyFilter: SearchText='{SearchText}', Type='{SelectedTypeFilter}'");

        var filtered = AllPersons.AsEnumerable();

        // Filter by type
        if (SelectedTypeFilter == "user")
        {
            filtered = filtered.Where(p => p.Type == PersonType.User);
        }
        else if (SelectedTypeFilter == "assistant")
        {
            filtered = filtered.Where(p => p.Type == PersonType.Assistant);
        }
        // "all" - no filter

        // Filter by search text (name, address)
        if (!string.IsNullOrWhiteSpace(SearchText))
        {
            var search = SearchText.Trim().ToLower();
            filtered = filtered.Where(p =>
                (p.Name != null && p.Name.ToLower().Contains(search)) ||
                (p.Address != null && p.Address.ToLower().Contains(search)));
        }

        FilteredPersons.Clear();
        foreach (var person in filtered)
        {
            FilteredPersons.Add(person);
        }

        MauiProgram.Log($"[ManagementViewModel] FilteredPersons count: {FilteredPersons.Count}");
    }

    /// <summary>
    /// Select a person (for editing or deletion).
    /// </summary>
    [RelayCommand]
    private void SelectPerson(Person? person)
    {
        if (person == null)
        {
            return;
        }

        MauiProgram.Log($"[ManagementViewModel] SelectPerson: {person.Name}");
        SelectedPerson = person;
    }

    /// <summary>
    /// Create a new person.
    /// </summary>
    [RelayCommand]
    private async Task CreatePersonAsync()
    {
        MauiProgram.Log("[ManagementViewModel] CreatePersonAsync - TODO: Navigate to create page");
        // TODO: Navigate to create/edit page with null person (new entry)
        await Shell.Current.DisplayAlertAsync("알림", "신규 등록 기능은 Week 3 Day 21에 구현 예정입니다.", "확인");
    }

    /// <summary>
    /// Edit the selected person.
    /// </summary>
    [RelayCommand]
    private async Task EditPersonAsync()
    {
        if (SelectedPerson == null)
        {
            await Shell.Current.DisplayAlertAsync("오류", "편집할 사람을 선택하세요.", "확인");
            return;
        }

        MauiProgram.Log($"[ManagementViewModel] EditPersonAsync: {SelectedPerson.Name}");
        // TODO: Navigate to edit page with SelectedPerson
        await Shell.Current.DisplayAlertAsync("알림", "편집 기능은 Week 3 Day 21에 구현 예정입니다.", "확인");
    }

    /// <summary>
    /// Delete the selected person.
    /// </summary>
    [RelayCommand]
    private async Task DeletePersonAsync()
    {
        if (SelectedPerson == null)
        {
            await Shell.Current.DisplayAlertAsync("오류", "삭제할 사람을 선택하세요.", "확인");
            return;
        }

        MauiProgram.Log($"[ManagementViewModel] DeletePersonAsync: {SelectedPerson.Name}");

        var confirm = await Shell.Current.DisplayAlertAsync(
            "삭제 확인",
            $"{SelectedPerson.Name}님을 삭제하시겠습니까?",
            "삭제",
            "취소");

        if (!confirm)
        {
            return;
        }

        await ExecuteAsync(async () =>
        {
            try
            {
                await _supabaseService.DeletePersonAsync(SelectedPerson.Id);
                AllPersons.Remove(SelectedPerson);
                ApplyFilter();
                SelectedPerson = null;
                await Shell.Current.DisplayAlertAsync("성공", "삭제되었습니다.", "확인");
                MauiProgram.Log($"[ManagementViewModel] Person deleted: {SelectedPerson?.Name}");
            }
            catch (Exception ex)
            {
                MauiProgram.Log($"[ManagementViewModel] Delete FAILED: {ex.Message}");
                await Shell.Current.DisplayAlertAsync("오류", $"삭제 실패: {ex.Message}", "확인");
            }
        });
    }
}

