# Cursor AI 2025 종합 가이드 - Session 2.6: MAUI & Blazor 규칙

## 📌 개요

이 파일은 `.cursor/rules/60-maui-blazor.mdc`에 저장되며, **모든 .NET MAUI/Blazor 크로스플랫폼 개발**에 자동으로 로드됩니다.

**핵심**: 하나의 C# 코드로 iOS, Android, Web(Blazor)을 동시에 개발합니다.

---

## 📄 파일 내용: `.cursor/rules/60-maui-blazor.mdc`

```yaml
---
description: ".NET MAUI & Blazor Cross-Platform Development (2025)"
globs:
  - "dotnet/**/*.cs"
  - "dotnet/**/*.xaml"
  - "dotnet/**/*.razor"
  - "**/MauiProgram.cs"
  - "**/Program.cs"
alwaysApply: false
priority: 9
---

# .NET MAUI & Blazor Standards for Cross-Platform Apps (2025)

## 🎯 Core Philosophy

1. **Single Codebase**: Share business logic across iOS, Android, Web, Windows
2. **MVVM Pattern**: Consistent architecture (same as WPF/MAUI)
3. **Adaptive UI**: Respond to platform-specific requirements
4. **Offline-First**: Support offline sync with cloud backend
5. **Type Safety**: Full C# type hints, nullable reference types

---

## 📁 MAUI Project Structure

```
dotnet/
├── MyApp.MAUI/                    # Mobile/Desktop cross-platform app
│   ├── MauiProgram.cs            # MAUI configuration
│   ├── App.xaml                  # App shell definition
│   ├── App.xaml.cs
│   │
│   ├── Views/                    # XAML views (cross-platform)
│   │   ├── MainPage.xaml
│   │   ├── MainPage.xaml.cs
│   │   ├── Pages/
│   │   │   ├── DashboardPage.xaml
│   │   │   ├── DashboardPage.xaml.cs
│   │   │   ├── DetailsPage.xaml
│   │   │   └── DetailsPage.xaml.cs
│   │   └── Controls/
│   │       ├── CustomButton.xaml
│   │       └── StatusIndicator.xaml
│   │
│   ├── ViewModels/               # Shared business logic
│   │   ├── MainViewModel.cs
│   │   ├── DashboardViewModel.cs
│   │   └── Base/BaseViewModel.cs
│   │
│   ├── Models/                   # Data models
│   │   ├── User.cs
│   │   ├── Task.cs
│   │   └── SyncState.cs
│   │
│   ├── Services/                 # Platform-agnostic services
│   │   ├── IDataService.cs
│   │   ├── DataService.cs
│   │   ├── IApiClient.cs
│   │   ├── ApiClient.cs
│   │   ├── ISyncService.cs
│   │   └── SyncService.cs
│   │
│   ├── Platforms/                # Platform-specific code
│   │   ├── Android/
│   │   │   ├── MainActivity.cs
│   │   │   ├── AndroidManifest.xml
│   │   │   └── PlatformSpecific.cs
│   │   ├── iOS/
│   │   │   ├── AppDelegate.cs
│   │   │   ├── Info.plist
│   │   │   └── PlatformSpecific.cs
│   │   ├── MacCatalyst/
│   │   ├── Windows/
│   │   │   ├── App.xaml
│   │   │   └── MainWindow.xaml.cs
│   │   └── Tizen/
│   │
│   ├── Helpers/
│   │   ├── Converters.cs
│   │   ├── Behaviors.cs
│   │   └── Validators.cs
│   │
│   └── Resources/
│       ├── Styles/
│       │   ├── Colors.xaml
│       │   ├── Styles.xaml
│       │   └── PlatformStyles.xaml
│       └── Fonts/
│
├── MyApp.Blazor/                 # Blazor Web app
│   ├── Program.cs
│   ├── App.razor
│   ├── Pages/
│   │   ├── Index.razor
│   │   ├── Dashboard.razor
│   │   ├── Users.razor
│   │   └── Settings.razor
│   ├── Components/
│   │   ├── Layout/
│   │   │   ├── MainLayout.razor
│   │   │   └── NavMenu.razor
│   │   ├── Features/
│   │   │   ├── UserList.razor
│   │   │   ├── UserForm.razor
│   │   │   └── Chart.razor
│   │   └── Shared/
│   │       ├── Card.razor
│   │       ├── Button.razor
│   │       └── Modal.razor
│   ├── Services/
│   │   ├── IApiClient.cs
│   │   └── ApiClient.cs
│   └── wwwroot/
│       ├── css/
│       ├── js/
│       └── images/
│
└── MyApp.Shared/                 # Shared library (MAUI + Blazor)
    ├── Models/
    ├── Services/
    ├── ViewModels/
    └── Utilities/
```

---

## 🔧 MAUI Patterns

### Pattern 1: MauiProgram.cs Setup (DI Container)

```csharp
// dotnet/MyApp.MAUI/MauiProgram.cs
using Microsoft.Maui;
using Microsoft.Maui.Hosting;
using Microsoft.Extensions.DependencyInjection;
using CommunityToolkit.Mvvm.Messaging;

#nullable enable

namespace MyApp.MAUI;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        // Register Services
        builder.Services.AddSingleton<IApiClient, ApiClient>();
        builder.Services.AddSingleton<IDataService, DataService>();
        builder.Services.AddSingleton<ISyncService, SyncService>();
        builder.Services.AddSingleton<IMessenger>(WeakReferenceMessenger.Default);

        // Register ViewModels
        builder.Services.AddSingleton<MainViewModel>();
        builder.Services.AddSingleton<DashboardViewModel>();
        builder.Services.AddSingleton<DetailsViewModel>();

        // Register Views
        builder.Services.AddSingleton<MainPage>();
        builder.Services.AddSingleton<DashboardPage>();
        builder.Services.AddSingleton<DetailsPage>();

        // Shell Navigation
        builder.Services.AddSingleton<AppShell>();

        return builder.Build();
    }
}

// dotnet/MyApp.MAUI/App.xaml.cs
public partial class App : Application
{
    public App()
    {
        InitializeComponent();
        MainPage = new AppShell();
    }
}
```

---

### Pattern 2: AppShell.xaml (Navigation)

```xml
<!-- dotnet/MyApp.MAUI/AppShell.xaml -->
<?xml version="1.0" encoding="UTF-8" ?>
<Shell
    x:Class="MyApp.MAUI.AppShell"
    xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
    xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
    FlyoutBehavior="Flyout"
    Title="My App">

    <TabBar>
        <!-- Dashboard Tab -->
        <ShellContent
            Title="Dashboard"
            Icon="dashboard.png"
            ContentTemplate="{DataTemplate local:DashboardPage}"
            Route="dashboard" />

        <!-- Users Tab -->
        <ShellContent
            Title="Users"
            Icon="users.png"
            ContentTemplate="{DataTemplate local:UsersPage}"
            Route="users" />

        <!-- Settings Tab -->
        <ShellContent
            Title="Settings"
            Icon="settings.png"
            ContentTemplate="{DataTemplate local:SettingsPage}"
            Route="settings" />
    </TabBar>

    <!-- Flyout Items -->
    <FlyoutItem Title="About" Icon="info.png" Route="about">
        <ShellContent ContentTemplate="{DataTemplate local:AboutPage}" />
    </FlyoutItem>
</Shell>
```

---

### Pattern 3: MAUI Page with ViewModel

```csharp
// dotnet/MyApp.MAUI/Views/Pages/DashboardPage.xaml.cs
using Microsoft.Maui.Controls;
using MyApp.MAUI.ViewModels;

#nullable enable

namespace MyApp.MAUI.Views;

public partial class DashboardPage : ContentPage
{
    private readonly DashboardViewModel _viewModel;

    public DashboardPage(DashboardViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;

        // Load data on navigation
        this.Loaded += async (s, e) => await _viewModel.LoadDataCommand.ExecuteAsync(null);
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        // Cleanup if needed
    }
}
```

---

### Pattern 4: Platform-Specific Code

```csharp
// dotnet/MyApp.MAUI/Platforms/Android/PlatformSpecific.cs
#if __ANDROID__
using Android.Content.PM;
using Android.OS;

#nullable enable

namespace MyApp.MAUI.Platforms.Android;

public static class AndroidSpecific
{
    /// <summary>
    /// Initialize Android-specific features
    /// </summary>
    public static void InitializeAndroid(Activity activity)
    {
        // Camera permissions
        if (Build.VERSION.SdkInt >= BuildVersionCodes.M)
        {
            // Handle runtime permissions
        }

        // Push notifications setup
        // Background services
    }

    /// <summary>
    /// Request runtime permission (Android 6.0+)
    /// </summary>
    public static async Task<PermissionStatus> RequestPermissionAsync<T>() where T : Permissions.BasePermission, new()
    {
        try
        {
            var status = await Permissions.CheckStatusAsync<T>();
            if (status != PermissionStatus.Granted)
            {
                status = await Permissions.RequestAsync<T>();
            }
            return status;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Permission error: {ex}");
            return PermissionStatus.Denied;
        }
    }
}
#endif
```

```csharp
// dotnet/MyApp.MAUI/Platforms/iOS/PlatformSpecific.cs
#if __IOS__
using UIKit;

#nullable enable

namespace MyApp.MAUI.Platforms.iOS;

public static class iOSSpecific
{
    /// <summary>
    /// Initialize iOS-specific features
    /// </summary>
    public static void InitializeiOS()
    {
        // Dark mode support
        // Notch handling
        // App Clips configuration
    }

    /// <summary>
    /// Set app appearance (light/dark mode)
    /// </summary>
    public static void SetAppearance(AppTheme theme)
    {
        var appearance = UIAppearance.Appearance;
        
        switch (theme)
        {
            case AppTheme.Dark:
                if (@available(iOS 13.0, *))
                {
                    UIApplication.SharedApplication.Windows[0].OverrideUserInterfaceStyle = UIUserInterfaceStyle.Dark;
                }
                break;
            case AppTheme.Light:
                if (@available(iOS 13.0, *))
                {
                    UIApplication.SharedApplication.Windows[0].OverrideUserInterfaceStyle = UIUserInterfaceStyle.Light;
                }
                break;
        }
    }
}
#endif
```

---

## 🌐 Blazor Patterns

### Pattern 1: Blazor Program.cs Setup

```csharp
// dotnet/MyApp.Blazor/Program.cs
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MyApp.Blazor;

#nullable enable

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Register HttpClient
builder.Services.AddScoped(sp =>
    new HttpClient
    {
        BaseAddress = new Uri(builder.HostEnvironment.BaseAddress)
    });

// Register Services
builder.Services.AddScoped<IApiClient, ApiClient>();
builder.Services.AddScoped<IDataService, DataService>();

// Add authentication if needed
builder.Services.AddOidcAuthentication(options =>
{
    builder.Configuration.Bind("Auth0", options.ProviderOptions);
});

await builder.Build().RunAsync();
```

---

### Pattern 2: Blazor Component (.razor)

```razor
<!-- dotnet/MyApp.Blazor/Components/Features/UserList.razor -->
@page "/users"
@using MyApp.Shared.Models
@inject IApiClient ApiClient
@inject NavigationManager Nav

<PageTitle>Users</PageTitle>

<div class="container py-4">
    <div class="d-flex justify-content-between align-items-center mb-4">
        <h1>Users Management</h1>
        <button class="btn btn-primary" @onclick="OpenCreateDialog">
            ➕ Add User
        </button>
    </div>

    @if (IsLoading)
    {
        <div class="spinner-border" role="status">
            <span class="visually-hidden">Loading...</span>
        </div>
    }
    else if (Users == null || !Users.Any())
    {
        <div class="alert alert-info">No users found</div>
    }
    else
    {
        <table class="table table-striped">
            <thead>
                <tr>
                    <th>Name</th>
                    <th>Email</th>
                    <th>Role</th>
                    <th>Actions</th>
                </tr>
            </thead>
            <tbody>
                @foreach (var user in Users)
                {
                    <tr>
                        <td>@user.Name</td>
                        <td>@user.Email</td>
                        <td>
                            <span class="badge bg-@(user.IsAdmin ? "danger" : "info")">
                                @(user.IsAdmin ? "Admin" : "User")
                            </span>
                        </td>
                        <td>
                            <button class="btn btn-sm btn-warning" @onclick="() => EditUser(user)">Edit</button>
                            <button class="btn btn-sm btn-danger" @onclick="() => DeleteUser(user.Id)">Delete</button>
                        </td>
                    </tr>
                }
            </tbody>
        </table>
    }

    @if (ShowError)
    {
        <div class="alert alert-danger alert-dismissible fade show" role="alert">
            @ErrorMessage
            <button type="button" class="btn-close" @onclick="() => ShowError = false"></button>
        </div>
    }
</div>

@code {
    private List<User>? Users;
    private bool IsLoading = false;
    private bool ShowError = false;
    private string ErrorMessage = string.Empty;

    protected override async Task OnInitializedAsync()
    {
        await LoadUsers();
    }

    private async Task LoadUsers()
    {
        IsLoading = true;
        ShowError = false;

        try
        {
            Users = await ApiClient.GetAsync<List<User>>("/api/v1/users");
        }
        catch (Exception ex)
        {
            ShowError = true;
            ErrorMessage = $"Failed to load users: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    private async Task DeleteUser(int userId)
    {
        if (!await JS.InvokeAsync<bool>("confirm", "Are you sure?"))
            return;

        try
        {
            await ApiClient.DeleteAsync($"/api/v1/users/{userId}");
            Users?.RemoveAll(u => u.Id == userId);
        }
        catch (Exception ex)
        {
            ShowError = true;
            ErrorMessage = $"Delete failed: {ex.Message}";
        }
    }

    private void OpenCreateDialog()
    {
        Nav.NavigateTo("/users/create");
    }

    private void EditUser(User user)
    {
        Nav.NavigateTo($"/users/edit/{user.Id}");
    }
}
```

---

### Pattern 3: Blazor Form Component

```razor
<!-- dotnet/MyApp.Blazor/Components/Features/UserForm.razor -->
@page "/users/create"
@page "/users/edit/{UserId:int}"
@using MyApp.Shared.Models
@inject IApiClient ApiClient
@inject NavigationManager Nav

<PageTitle>@(UserId == 0 ? "Create User" : "Edit User")</PageTitle>

<div class="container py-4">
    <h1>@(UserId == 0 ? "Create New User" : "Edit User")</h1>

    <EditForm Model="@FormModel" OnValidSubmit="@HandleSubmit">
        <DataAnnotationsValidator />
        <ValidationSummary class="alert alert-danger" />

        <div class="mb-3">
            <label for="name" class="form-label">Full Name</label>
            <InputText id="name" class="form-control" @bind-Value="FormModel.Name" />
            <ValidationMessage For="() => FormModel.Name" class="text-danger" />
        </div>

        <div class="mb-3">
            <label for="email" class="form-label">Email</label>
            <InputText id="email" type="email" class="form-control" @bind-Value="FormModel.Email" />
            <ValidationMessage For="() => FormModel.Email" class="text-danger" />
        </div>

        <div class="mb-3 form-check">
            <InputCheckbox id="is-admin" class="form-check-input" @bind-Value="FormModel.IsAdmin" />
            <label class="form-check-label" for="is-admin">Admin User</label>
        </div>

        <div class="mb-3">
            <button type="submit" class="btn btn-primary" disabled="@IsSubmitting">
                @if (IsSubmitting)
                {
                    <span class="spinner-border spinner-border-sm me-2" role="status" aria-hidden="true"></span>
                    Saving...
                }
                else
                {
                    @(UserId == 0 ? "Create User" : "Update User")
                }
            </button>
            <a href="/users" class="btn btn-secondary ms-2">Cancel</a>
        </div>
    </EditForm>

    @if (ShowError)
    {
        <div class="alert alert-danger">@ErrorMessage</div>
    }
}

@code {
    [Parameter]
    public int UserId { get; set; }

    private UserFormModel FormModel = new();
    private bool IsSubmitting = false;
    private bool ShowError = false;
    private string ErrorMessage = string.Empty;

    protected override async Task OnInitializedAsync()
    {
        if (UserId > 0)
        {
            try
            {
                var user = await ApiClient.GetAsync<User>($"/api/v1/users/{UserId}");
                FormModel = new UserFormModel
                {
                    Name = user.Name,
                    Email = user.Email,
                    IsAdmin = user.IsAdmin
                };
            }
            catch (Exception ex)
            {
                ShowError = true;
                ErrorMessage = $"Failed to load user: {ex.Message}";
            }
        }
    }

    private async Task HandleSubmit()
    {
        IsSubmitting = true;
        ShowError = false;

        try
        {
            if (UserId == 0)
            {
                await ApiClient.PostAsync<UserFormModel, User>("/api/v1/users", FormModel);
            }
            else
            {
                await ApiClient.PutAsync<UserFormModel, User>($"/api/v1/users/{UserId}", FormModel);
            }

            Nav.NavigateTo("/users");
        }
        catch (Exception ex)
        {
            ShowError = true;
            ErrorMessage = $"Submit failed: {ex.Message}";
        }
        finally
        {
            IsSubmitting = false;
        }
    }

    private class UserFormModel
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public bool IsAdmin { get; set; }
    }
}
```

---

## 🚫 Anti-Patterns (NEVER DO THIS)

```csharp
// ❌ WRONG: Platform-specific code in shared code
public class DataService
{
    public void SaveData(string data)
    {
        #if __ANDROID__
        // Android code here
        #elif __IOS__
        // iOS code here
        #endif
    }
}

// ✅ CORRECT: Use interface pattern
public interface IPlatformService
{
    void SaveData(string data);
}

// Platform-specific implementations
#if __ANDROID__
public class AndroidPlatformService : IPlatformService { ... }
#elif __IOS__
public class iOSPlatformService : IPlatformService { ... }
#endif
```

```csharp
// ❌ WRONG: UI code in ViewModel
public class DashboardViewModel : BaseViewModel
{
    public void ShowAlert(string message)
    {
        Application.Current?.MainThread.BeginInvokeOnMainThread(() =>
        {
            Application.Current.MainPage?.DisplayAlert("Alert", message, "OK");
        });
    }
}

// ✅ CORRECT: Use Messenger pattern
public class DashboardViewModel : BaseViewModel
{
    private readonly IMessenger _messenger;

    public DashboardViewModel(IMessenger messenger)
    {
        _messenger = messenger;
    }

    public void NotifyUser(string message)
    {
        _messenger.Send(new AlertMessage(message));
    }
}
```

```xml
<!-- ❌ WRONG: Hardcoded platform-specific UI -->
<Grid RowDefinitions="Auto,*">
    <Label Text="iPhone Safe Area" Padding="10,44,10,0" />
</Grid>

<!-- ✅ CORRECT: Use platform-specific styles -->
<Grid RowDefinitions="Auto,*" Padding="{OnPlatform iOS='10,44,10,0', Android='10,10,10,0'}">
    <Label Text="Adaptive Padding" />
</Grid>
```

---

## ✅ Shared Code Checklist

- [ ] Business logic in shared library
- [ ] ViewModels shared across platforms
- [ ] Data models in shared library
- [ ] Services use dependency injection
- [ ] Platform-specific code isolated
- [ ] No UI code in ViewModels
- [ ] Async/await throughout
- [ ] Proper error handling
- [ ] Type-safe throughout

---

## 🎓 2025 Features

- ✅ **MAUI 9.0+**: Full cross-platform support
- ✅ **Blazor WebAssembly**: Client-side .NET in browser
- ✅ **Shared Code**: ~80% code reuse across platforms
- ✅ **Hot Reload**: XAML/C# changes without rebuild
- ✅ **Accessibility**: WCAG 2.1 compliance built-in

---

## 📚 Related Sessions

- **Session 2.5**: WPF/MAUI (Desktop focus)
- **Session 3**: Slash Commands (code generation)

---

**Document Version**: 1.0 (2025-12-09)
**Frameworks**: .NET MAUI 9.0+, Blazor WebAssembly
**Status**: Production-ready
```

---

이제 이 파일도 `.cursor/rules/60-maui-blazor.mdc`로 저장하시면 됩니다.

**최종 규칙 파일 목록:**

| 파일명 | 내용 |
|--------|------|
| `00-index.mdc` | 전역 설정 |
| `10-web-nextjs-react.mdc` | Next.js/React |
| `20-ui-tailwind-shadcn.mdc` | UI/Tailwind |
| `30-backend-fastapi.mdc` | FastAPI |
| `40-dashboard-streamlit.mdc` | Streamlit |
| `50-desktop-dotnet.mdc` | WPF/MAUI |
| `60-maui-blazor.mdc` | ✨ **MAUI/Blazor (새로 추가!)** |

---

**이제 7개 규칙 파일로 완전한 기업 자동화 스택 커버!** 🚀
