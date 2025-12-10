#nullable enable

using LmpLink.MAUI.ViewModels.Base;

namespace LmpLink.MAUI.ViewModels;

/// <summary>
/// ViewModel for SettingsPage (UI customization, theme settings).
/// </summary>
public partial class SettingsViewModel : BaseViewModel
{
    // --- Observable Properties ---

    // Marker Settings
    [ObservableProperty]
    private string _userMarkerColor = "#60A5FA"; // Default: Blue

    [ObservableProperty]
    private string _assistantMarkerColor = "#FB923C"; // Default: Orange

    [ObservableProperty]
    private string _markerSize = "Medium"; // Small, Medium, Large

    // Circle Settings
    [ObservableProperty]
    private string _circleColor = "#60A5FA"; // Default: Blue

    [ObservableProperty]
    private double _circleOpacity = 0.3; // 0.0 - 1.0

    [ObservableProperty]
    private int _circleBorderWidth = 2; // 1 - 5 px

    // Theme Settings
    [ObservableProperty]
    private bool _isDarkMode = true; // Default: Dark mode

    // UI Size Settings
    [ObservableProperty]
    private string _fontSize = "Medium"; // Small, Medium, Large

    [ObservableProperty]
    private string _cardSize = "Normal"; // Compact, Normal, Large

    // --- Constructor ---

    public SettingsViewModel()
    {
        MauiProgram.Log("[SettingsViewModel] Constructor called");
        LoadSettings();
    }

    // --- Commands ---

    /// <summary>
    /// Load settings from Preferences.
    /// </summary>
    [RelayCommand]
    private void LoadSettings()
    {
        MauiProgram.Log("[SettingsViewModel] LoadSettings START");

        try
        {
            // Marker Settings
            UserMarkerColor = Preferences.Default.Get("UserMarkerColor", "#60A5FA");
            AssistantMarkerColor = Preferences.Default.Get("AssistantMarkerColor", "#FB923C");
            MarkerSize = Preferences.Default.Get("MarkerSize", "Medium");

            // Circle Settings
            CircleColor = Preferences.Default.Get("CircleColor", "#60A5FA");
            CircleOpacity = Preferences.Default.Get("CircleOpacity", 0.3);
            CircleBorderWidth = Preferences.Default.Get("CircleBorderWidth", 2);

            // Theme Settings
            IsDarkMode = Preferences.Default.Get("IsDarkMode", true);

            // UI Size Settings
            FontSize = Preferences.Default.Get("FontSize", "Medium");
            CardSize = Preferences.Default.Get("CardSize", "Normal");

            MauiProgram.Log($"[SettingsViewModel] Settings loaded: DarkMode={IsDarkMode}, MarkerSize={MarkerSize}");
        }
        catch (Exception ex)
        {
            MauiProgram.Log($"[SettingsViewModel] LoadSettings FAILED: {ex.Message}");
        }
    }

    /// <summary>
    /// Save current settings to Preferences.
    /// </summary>
    [RelayCommand]
    private async Task SaveSettingsAsync()
    {
        await ExecuteAsync(async () =>
        {
            MauiProgram.Log("[SettingsViewModel] SaveSettings START");

            try
            {
                // Marker Settings
                Preferences.Default.Set("UserMarkerColor", UserMarkerColor);
                Preferences.Default.Set("AssistantMarkerColor", AssistantMarkerColor);
                Preferences.Default.Set("MarkerSize", MarkerSize);

                // Circle Settings
                Preferences.Default.Set("CircleColor", CircleColor);
                Preferences.Default.Set("CircleOpacity", CircleOpacity);
                Preferences.Default.Set("CircleBorderWidth", CircleBorderWidth);

                // Theme Settings
                Preferences.Default.Set("IsDarkMode", IsDarkMode);

                // UI Size Settings
                Preferences.Default.Set("FontSize", FontSize);
                Preferences.Default.Set("CardSize", CardSize);

                MauiProgram.Log("[SettingsViewModel] Settings saved successfully");
                await Shell.Current.DisplayAlertAsync("저장 완료", "설정이 저장되었습니다.", "확인");
            }
            catch (Exception ex)
            {
                MauiProgram.Log($"[SettingsViewModel] SaveSettings FAILED: {ex.Message}");
                await Shell.Current.DisplayAlertAsync("오류", $"설정 저장 실패:\n{ex.Message}", "확인");
            }
        });
    }

    /// <summary>
    /// Reset settings to defaults.
    /// </summary>
    [RelayCommand]
    private async Task ResetSettingsAsync()
    {
        var confirm = await Shell.Current.DisplayAlertAsync(
            "설정 초기화",
            "모든 설정을 기본값으로 되돌리시겠습니까?",
            "초기화",
            "취소");

        if (!confirm)
        {
            return;
        }

        await ExecuteAsync(async () =>
        {
            MauiProgram.Log("[SettingsViewModel] ResetSettings START");

            try
            {
                // Clear all preferences
                Preferences.Default.Clear();

                // Reload defaults
                LoadSettings();

                MauiProgram.Log("[SettingsViewModel] Settings reset successfully");
                await Shell.Current.DisplayAlertAsync("초기화 완료", "설정이 초기화되었습니다.", "확인");
            }
            catch (Exception ex)
            {
                MauiProgram.Log($"[SettingsViewModel] ResetSettings FAILED: {ex.Message}");
                await Shell.Current.DisplayAlertAsync("오류", $"설정 초기화 실패:\n{ex.Message}", "확인");
            }
        });
    }

    /// <summary>
    /// Update marker size.
    /// </summary>
    [RelayCommand]
    private void UpdateMarkerSize(string size)
    {
        MauiProgram.Log($"[SettingsViewModel] UpdateMarkerSize: {size}");
        MarkerSize = size;
    }

    /// <summary>
    /// Update user marker color.
    /// </summary>
    [RelayCommand]
    private void UpdateUserMarkerColor(string color)
    {
        MauiProgram.Log($"[SettingsViewModel] UpdateUserMarkerColor: {color}");
        UserMarkerColor = color;
    }

    /// <summary>
    /// Update assistant marker color.
    /// </summary>
    [RelayCommand]
    private void UpdateAssistantMarkerColor(string color)
    {
        MauiProgram.Log($"[SettingsViewModel] UpdateAssistantMarkerColor: {color}");
        AssistantMarkerColor = color;
    }

    /// <summary>
    /// Update circle color.
    /// </summary>
    [RelayCommand]
    private void UpdateCircleColor(string color)
    {
        MauiProgram.Log($"[SettingsViewModel] UpdateCircleColor: {color}");
        CircleColor = color;
    }

    /// <summary>
    /// Update font size.
    /// </summary>
    [RelayCommand]
    private void UpdateFontSize(string size)
    {
        MauiProgram.Log($"[SettingsViewModel] UpdateFontSize: {size}");
        FontSize = size;
    }

    /// <summary>
    /// Update card size.
    /// </summary>
    [RelayCommand]
    private void UpdateCardSize(string size)
    {
        MauiProgram.Log($"[SettingsViewModel] UpdateCardSize: {size}");
        CardSize = size;
    }

    /// <summary>
    /// Toggle dark mode.
    /// </summary>
    [RelayCommand]
    private void ToggleDarkMode()
    {
        IsDarkMode = !IsDarkMode;
        MauiProgram.Log($"[SettingsViewModel] DarkMode toggled: {IsDarkMode}");
        
        // TODO: Apply theme change to app
        // Application.Current.UserAppTheme = IsDarkMode ? AppTheme.Dark : AppTheme.Light;
    }
}

