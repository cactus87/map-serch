# Cursor AI 2025 종합 가이드 - Session 2.5: .NET 10 & Desktop 규칙

## 📌 개요

이 파일은 `.cursor/rules/50-desktop-dotnet.mdc`에 저장되며, **모든 .NET/C#/WPF/MAUI 데스크톱 개발**에 자동으로 로드됩니다.

**핵심**: .NET 10 LTS의 안정성과 모던 C# 문법을 활용한 엔터프라이즈 데스크톱 애플리케이션.

---

## 📄 파일 내용: `.cursor/rules/50-desktop-dotnet.mdc`

```yaml
---
description: ".NET 10 LTS, C# 13/14, WPF & MAUI Desktop Applications"
globs:
  - "dotnet/**/*.cs"
  - "dotnet/**/*.xaml"
  - "**/*.csproj"
  - "**/App.xaml.cs"
alwaysApply: false
priority: 8
---

# .NET 10 & C# 13/14 Standards for Enterprise Desktop (2025)

## 🎯 Core Philosophy

1. **MVVM Pattern**: Strict separation of Model, View, ViewModel
2. **Modern C#**: Use C# 13/14 features (records, init properties, required keyword)
3. **Type Safety**: Nullable reference types enabled (#nullable enable)
4. **Async/Await**: All I/O and long operations are async
5. **DI Container**: Dependency Injection for all services
6. **Cross-Platform**: Support both WPF (Windows) and MAUI (iOS/Android)

---

## 📁 Project Structure

```
dotnet/
├── EnterpriseApp/                 # Main WPF application
│   ├── App.xaml                   # App entry point
│   ├── App.xaml.cs
│   ├── EnterpriseApp.csproj
│   ├── appsettings.json           # Configuration
│   │
│   ├── Views/                     # XAML Views (UI)
│   │   ├── MainWindow.xaml
│   │   ├── MainWindow.xaml.cs
│   │   ├── Pages/
│   │   │   ├── DashboardPage.xaml
│   │   │   ├── DashboardPage.xaml.cs
│   │   │   ├── UsersPage.xaml
│   │   │   └── UsersPage.xaml.cs
│   │   └── Controls/
│   │       ├── StatCard.xaml
│   │       └── DataGrid.xaml
│   │
│   ├── ViewModels/                # MVVM ViewModels
│   │   ├── MainViewModel.cs
│   │   ├── DashboardViewModel.cs
│   │   ├── UsersViewModel.cs
│   │   └── Base/
│   │       └── BaseViewModel.cs
│   │
│   ├── Models/                    # Data Models
│   │   ├── User.cs
│   │   ├── Task.cs
│   │   └── Dashboard.cs
│   │
│   ├── Services/                  # Business Logic Layer
│   │   ├── IUserService.cs        # Interface
│   │   ├── UserService.cs         # Implementation
│   │   ├── IApiClient.cs
│   │   ├── ApiClient.cs           # HTTP client to FastAPI
│   │   ├── ISettingsService.cs
│   │   └── SettingsService.cs
│   │
│   ├── Repositories/              # Data Access Layer
│   │   ├── IUserRepository.cs
│   │   ├── UserRepository.cs
│   │   ├── ILocalDatabase.cs      # SQLite local cache
│   │   └── LocalDatabase.cs
│   │
│   ├── Converters/                # Value Converters (XAML)
│   │   ├── BoolToVisibilityConverter.cs
│   │   ├── DateTimeConverter.cs
│   │   └── EnumToStringConverter.cs
│   │
│   ├── Behaviors/                 # Attached Behaviors
│   │   ├── TextBoxBehavior.cs
│   │   └── CommandBehavior.cs
│   │
│   ├── Resources/
│   │   ├── Styles.xaml            # Global styles and themes
│   │   ├── Colors.xaml
│   │   └── Icons.xaml
│   │
│   └── Utils/
│       ├── Logger.cs              # Structured logging
│       └── Validators.cs          # Input validation
│
├── EnterpriseApp.Maui/            # MAUI app (iOS/Android)
│   ├── MauiProgram.cs
│   ├── App.xaml
│   ├── Views/
│   ├── ViewModels/
│   └── Platforms/                 # Platform-specific code
│       ├── Android/
│       ├── iOS/
│       └── Windows/
│
├── EnterpriseApp.Tests/           # Unit Tests
│   ├── ViewModels/
│   │   └── DashboardViewModelTests.cs
│   ├── Services/
│   │   └── UserServiceTests.cs
│   └── Repositories/
│       └── UserRepositoryTests.cs
│
└── global.json                    # .NET version lock
```

---

## 🔧 Core Patterns

### Pattern 1: Global Setup & DI Container

```csharp
// dotnet/EnterpriseApp/App.xaml.cs
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

#nullable enable

namespace EnterpriseApp;

public partial class App : Application
{
    public static IServiceProvider ServiceProvider { get; private set; } = null!;

    public App()
    {
        InitializeComponent();

        // Configure services
        var services = new ServiceCollection();
        ConfigureServices(services);
        ServiceProvider = services.BuildServiceProvider();

        // Create and show main window
        MainWindow = new MainWindow
        {
            DataContext = ServiceProvider.GetRequiredService<MainViewModel>()
        };
    }

    private static void ConfigureServices(IServiceCollection services)
    {
        // Configuration
        var config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .AddEnvironmentVariables()
            .Build();

        services.AddSingleton<IConfiguration>(config);

        // ViewModels
        services.AddSingleton<MainViewModel>();
        services.AddSingleton<DashboardViewModel>();
        services.AddSingleton<UsersViewModel>();

        // Services (Scoped for each request, Singleton for app-wide)
        services.AddSingleton<IUserService, UserService>();
        services.AddSingleton<IApiClient, ApiClient>();
        services.AddSingleton<ISettingsService, SettingsService>();
        services.AddSingleton<ILogger, Logger>();

        // Repositories
        services.AddSingleton<IUserRepository, UserRepository>();
        services.AddSingleton<ILocalDatabase, LocalDatabase>();

        // Views
        services.AddSingleton<MainWindow>();
        services.AddSingleton<DashboardPage>();
        services.AddSingleton<UsersPage>();
    }

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        MainWindow?.Show();
    }
}
```

---

### Pattern 2: Base ViewModel (MVVM Toolkit)

```csharp
// dotnet/EnterpriseApp/ViewModels/Base/BaseViewModel.cs
using CommunityToolkit.Mvvm.ComponentModel;

#nullable enable

namespace EnterpriseApp.ViewModels;

/// <summary>
/// Base class for all ViewModels using CommunityToolkit.Mvvm
/// </summary>
public abstract class BaseViewModel : ObservableObject
{
    private bool _isLoading;
    private string? _errorMessage;

    /// <summary>
    /// Gets or sets loading state
    /// </summary>
    public bool IsLoading
    {
        get => _isLoading;
        set => SetProperty(ref _isLoading, value);
    }

    /// <summary>
    /// Gets or sets error message
    /// </summary>
    public string? ErrorMessage
    {
        get => _errorMessage;
        set => SetProperty(ref _errorMessage, value);
    }

    /// <summary>
    /// Clears error message
    /// </summary>
    public void ClearError() => ErrorMessage = null;

    /// <summary>
    /// Displays error message
    /// </summary>
    public void ShowError(string message) => ErrorMessage = message;
}
```

---

### Pattern 3: ViewModel with ObservableProperty & RelayCommand

```csharp
// dotnet/EnterpriseApp/ViewModels/UsersViewModel.cs
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using EnterpriseApp.Models;
using EnterpriseApp.Services;

#nullable enable

namespace EnterpriseApp.ViewModels;

public partial class UsersViewModel : BaseViewModel
{
    private readonly IUserService _userService;
    private readonly ILogger _logger;

    // Observable collection of users
    [ObservableProperty]
    private ObservableCollection<User> users = new();

    // Selected user
    [ObservableProperty]
    private User? selectedUser;

    // Form state
    [ObservableProperty]
    private string userName = string.Empty;

    [ObservableProperty]
    private string userEmail = string.Empty;

    [ObservableProperty]
    private bool isFormVisible;

    public UsersViewModel(IUserService userService, ILogger logger)
    {
        _userService = userService;
        _logger = logger;
    }

    // Load users on initialization
    [RelayCommand]
    public async Task LoadUsersAsync()
    {
        IsLoading = true;
        ErrorMessage = null;

        try
        {
            var users = await _userService.GetUsersAsync();
            Users.Clear();
            foreach (var user in users)
            {
                Users.Add(user);
            }

            _logger.Info($"Loaded {users.Count} users");
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Failed to load users: {ex.Message}";
            _logger.Error($"LoadUsersAsync failed: {ex}");
        }
        finally
        {
            IsLoading = false;
        }
    }

    // Create new user
    [RelayCommand]
    public async Task CreateUserAsync()
    {
        if (string.IsNullOrWhiteSpace(UserName) || string.IsNullOrWhiteSpace(UserEmail))
        {
            ErrorMessage = "Please fill in all fields";
            return;
        }

        IsLoading = true;
        ErrorMessage = null;

        try
        {
            var newUser = new User
            {
                Name = UserName,
                Email = UserEmail
            };

            var createdUser = await _userService.CreateUserAsync(newUser);
            Users.Add(createdUser);

            // Reset form
            UserName = string.Empty;
            UserEmail = string.Empty;
            IsFormVisible = false;

            _logger.Info($"User {createdUser.Name} created successfully");
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Failed to create user: {ex.Message}";
            _logger.Error($"CreateUserAsync failed: {ex}");
        }
        finally
        {
            IsLoading = false;
        }
    }

    // Delete user
    [RelayCommand]
    public async Task DeleteUserAsync(User user)
    {
        if (user?.Id == null) return;

        IsLoading = true;
        ErrorMessage = null;

        try
        {
            await _userService.DeleteUserAsync(user.Id.Value);
            Users.Remove(user);

            _logger.Info($"User {user.Name} deleted");
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Failed to delete user: {ex.Message}";
            _logger.Error($"DeleteUserAsync failed: {ex}");
        }
        finally
        {
            IsLoading = false;
        }
    }

    // Show form for creating new user
    [RelayCommand]
    public void ShowCreateForm()
    {
        UserName = string.Empty;
        UserEmail = string.Empty;
        IsFormVisible = true;
    }

    // Cancel form
    [RelayCommand]
    public void CancelForm()
    {
        IsFormVisible = false;
        ClearError();
    }
}
```

---

### Pattern 4: Service Layer with API Client

```csharp
// dotnet/EnterpriseApp/Services/IUserService.cs
using EnterpriseApp.Models;

#nullable enable

namespace EnterpriseApp.Services;

public interface IUserService
{
    Task<List<User>> GetUsersAsync();
    Task<User> GetUserByIdAsync(int id);
    Task<User> CreateUserAsync(User user);
    Task<User> UpdateUserAsync(int id, User user);
    Task DeleteUserAsync(int id);
}

// dotnet/EnterpriseApp/Services/UserService.cs
using EnterpriseApp.Models;
using EnterpriseApp.Repositories;

#nullable enable

namespace EnterpriseApp.Services;

public class UserService : IUserService
{
    private readonly IApiClient _apiClient;
    private readonly IUserRepository _repository;
    private readonly ILogger _logger;

    public UserService(IApiClient apiClient, IUserRepository repository, ILogger logger)
    {
        _apiClient = apiClient;
        _repository = repository;
        _logger = logger;
    }

    // Try FastAPI first, fallback to local cache
    public async Task<List<User>> GetUsersAsync()
    {
        try
        {
            // Call FastAPI backend
            var users = await _apiClient.GetAsync<List<User>>("/api/v1/users");
            
            // Cache locally
            await _repository.SaveUsersAsync(users);
            
            return users;
        }
        catch (HttpRequestException ex)
        {
            _logger.Warn($"FastAPI unavailable, using cached users: {ex.Message}");
            
            // Fallback to local cache
            return await _repository.GetAllUsersAsync();
        }
    }

    public async Task<User> CreateUserAsync(User user)
    {
        var response = await _apiClient.PostAsync<User, User>("/api/v1/users", user);
        await _repository.SaveUserAsync(response);
        return response;
    }

    public async Task<User> UpdateUserAsync(int id, User user)
    {
        var response = await _apiClient.PutAsync<User, User>($"/api/v1/users/{id}", user);
        await _repository.SaveUserAsync(response);
        return response;
    }

    public async Task DeleteUserAsync(int id)
    {
        await _apiClient.DeleteAsync($"/api/v1/users/{id}");
        await _repository.DeleteUserAsync(id);
    }

    public async Task<User> GetUserByIdAsync(int id)
    {
        return await _apiClient.GetAsync<User>($"/api/v1/users/{id}");
    }
}
```

---

### Pattern 5: XAML View with Data Binding

```xml
<!-- dotnet/EnterpriseApp/Views/Pages/UsersPage.xaml -->
<Page
    x:Class="EnterpriseApp.Views.Pages.UsersPage"
    xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    Title="Users Management"
    Background="White">

    <Grid RowDefinitions="Auto,*,Auto">
        <!-- Header -->
        <StackPanel Grid.Row="0" Margin="20" Orientation="Vertical" Spacing="10">
            <TextBlock Text="Users Management" FontSize="24" FontWeight="Bold" />
            
            <StackPanel Orientation="Horizontal" Spacing="10">
                <Button 
                    Content="➕ Add User"
                    Command="{Binding ShowCreateFormCommand}"
                    Padding="12,8"
                    Background="#2563eb"
                    Foreground="White" />
                
                <Button 
                    Content="🔄 Refresh"
                    Command="{Binding LoadUsersCommand}"
                    Padding="12,8" />
            </StackPanel>

            <!-- Error message -->
            <TextBlock 
                Text="{Binding ErrorMessage}"
                Foreground="Red"
                Visibility="{Binding ErrorMessage, Converter={StaticResource NotNullToVisibilityConverter}}" />
        </StackPanel>

        <!-- Users DataGrid -->
        <DataGrid 
            Grid.Row="1"
            Margin="20"
            ItemsSource="{Binding Users}"
            SelectedItem="{Binding SelectedUser}"
            AutoGenerateColumns="False"
            CanUserAddRows="False">
            
            <DataGrid.Columns>
                <DataGridTextColumn Header="Name" Binding="{Binding Name}" Width="*" />
                <DataGridTextColumn Header="Email" Binding="{Binding Email}" Width="200" />
                <DataGridTextColumn Header="Joined" Binding="{Binding CreatedAt, StringFormat=d}" Width="120" />
                
                <DataGridTemplateColumn Header="Actions" Width="100">
                    <DataGridTemplateColumn.CellTemplate>
                        <DataTemplate>
                            <StackPanel Orientation="Horizontal" Spacing="5">
                                <Button 
                                    Content="Edit"
                                    Command="{Binding DataContext.EditUserCommand, 
                                        RelativeSource={RelativeSource FindAncestor, 
                                        AncestorType=DataGrid}}"
                                    CommandParameter="{Binding}"
                                    Padding="8,4" />
                                
                                <Button 
                                    Content="Delete"
                                    Command="{Binding DataContext.DeleteUserCommand, 
                                        RelativeSource={RelativeSource FindAncestor, 
                                        AncestorType=DataGrid}}"
                                    CommandParameter="{Binding}"
                                    Foreground="Red"
                                    Padding="8,4" />
                            </StackPanel>
                        </DataTemplate>
                    </DataGridTemplateColumn.CellTemplate>
                </DataGridTemplateColumn>
            </DataGrid.Columns>
        </DataGrid>

        <!-- Create User Form (Dialog) -->
        <Border 
            Grid.Row="1"
            Background="White"
            BorderBrush="#e5e7eb"
            BorderThickness="1"
            CornerRadius="8"
            Margin="20"
            Padding="20"
            Visibility="{Binding IsFormVisible, Converter={StaticResource BoolToVisibilityConverter}}">
            
            <StackPanel Orientation="Vertical" Spacing="15">
                <TextBlock Text="Create New User" FontSize="18" FontWeight="Bold" />
                
                <StackPanel Orientation="Vertical" Spacing="5">
                    <TextBlock Text="Full Name:" FontWeight="SemiBold" />
                    <TextBox 
                        Text="{Binding UserName, UpdateSourceTrigger=PropertyChanged}"
                        Padding="10,8"
                        BorderThickness="1"
                        BorderBrush="#d1d5db" />
                </StackPanel>

                <StackPanel Orientation="Vertical" Spacing="5">
                    <TextBlock Text="Email:" FontWeight="SemiBold" />
                    <TextBox 
                        Text="{Binding UserEmail, UpdateSourceTrigger=PropertyChanged}"
                        Padding="10,8"
                        BorderThickness="1"
                        BorderBrush="#d1d5db" />
                </StackPanel>

                <StackPanel Orientation="Horizontal" Spacing="10">
                    <Button 
                        Content="Create"
                        Command="{Binding CreateUserCommand}"
                        Background="#2563eb"
                        Foreground="White"
                        Padding="12,8" />
                    
                    <Button 
                        Content="Cancel"
                        Command="{Binding CancelFormCommand}"
                        Padding="12,8" />
                </StackPanel>

                <TextBlock 
                    Text="{Binding ErrorMessage}"
                    Foreground="Red"
                    TextWrapping="Wrap"
                    Visibility="{Binding ErrorMessage, 
                        Converter={StaticResource NotNullToVisibilityConverter}}" />
            </StackPanel>
        </Border>

        <!-- Loading indicator -->
        <ProgressBar 
            Grid.Row="2"
            Height="4"
            Background="#f3f4f6"
            Foreground="#2563eb"
            IsIndeterminate="{Binding IsLoading}"
            Visibility="{Binding IsLoading, 
                Converter={StaticResource BoolToVisibilityConverter}}" />
    </Grid>
</Page>
```

---

### Pattern 6: API Client (Abstraction for FastAPI)

```csharp
// dotnet/EnterpriseApp/Services/IApiClient.cs
#nullable enable

namespace EnterpriseApp.Services;

public interface IApiClient
{
    Task<T> GetAsync<T>(string endpoint);
    Task<TResponse> PostAsync<TRequest, TResponse>(string endpoint, TRequest data);
    Task<TResponse> PutAsync<TRequest, TResponse>(string endpoint, TRequest data);
    Task DeleteAsync(string endpoint);
}

// dotnet/EnterpriseApp/Services/ApiClient.cs
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Configuration;

#nullable enable

namespace EnterpriseApp.Services;

public class ApiClient : IApiClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger _logger;
    private readonly string _baseUrl;

    public ApiClient(IConfiguration config, ILogger logger)
    {
        _logger = logger;
        _baseUrl = config["ApiUrl"] ?? "http://localhost:8000";
        
        _httpClient = new HttpClient
        {
            BaseAddress = new Uri(_baseUrl),
            Timeout = TimeSpan.FromSeconds(30)
        };
    }

    public async Task<T> GetAsync<T>(string endpoint)
    {
        try
        {
            var response = await _httpClient.GetAsync(endpoint);
            response.EnsureSuccessStatusCode();
            
            var content = await response.Content.ReadAsAsync<T>();
            return content ?? throw new InvalidOperationException("Empty response");
        }
        catch (HttpRequestException ex)
        {
            _logger.Error($"GET {endpoint} failed: {ex.Message}");
            throw;
        }
    }

    public async Task<TResponse> PostAsync<TRequest, TResponse>(
        string endpoint, 
        TRequest data)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync(endpoint, data);
            response.EnsureSuccessStatusCode();
            
            var content = await response.Content.ReadAsAsync<TResponse>();
            return content ?? throw new InvalidOperationException("Empty response");
        }
        catch (HttpRequestException ex)
        {
            _logger.Error($"POST {endpoint} failed: {ex.Message}");
            throw;
        }
    }

    public async Task<TResponse> PutAsync<TRequest, TResponse>(
        string endpoint, 
        TRequest data)
    {
        try
        {
            var response = await _httpClient.PutAsJsonAsync(endpoint, data);
            response.EnsureSuccessStatusCode();
            
            var content = await response.Content.ReadAsAsync<TResponse>();
            return content ?? throw new InvalidOperationException("Empty response");
        }
        catch (HttpRequestException ex)
        {
            _logger.Error($"PUT {endpoint} failed: {ex.Message}");
            throw;
        }
    }

    public async Task DeleteAsync(string endpoint)
    {
        try
        {
            var response = await _httpClient.DeleteAsync(endpoint);
            response.EnsureSuccessStatusCode();
        }
        catch (HttpRequestException ex)
        {
            _logger.Error($"DELETE {endpoint} failed: {ex.Message}");
            throw;
        }
    }
}
```

---

## 🚫 Anti-Patterns (NEVER DO THIS)

```csharp
// ❌ WRONG: Code-behind with business logic
public partial class UsersPage : Page
{
    private List<User> users;

    public UsersPage()
    {
        InitializeComponent();
        users = FetchUsersFromDatabase(); // BAD!
        DisplayUsers(users);
    }
}

// ✅ CORRECT: MVVM with ViewModel
public partial class UsersPage : Page
{
    public UsersPage(UsersViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
        viewModel.LoadUsersCommand.Execute(null);
    }
}
```

```csharp
// ❌ WRONG: No null safety
public class User
{
    public string Name { get; set; }      // Can be null!
    public string Email { get; set; }
}

// ✅ CORRECT: Nullable reference types
#nullable enable

public class User
{
    public required string Name { get; set; }      // Required
    public required string Email { get; set; }
    public string? PhoneNumber { get; set; }       // Optional
}
```

```csharp
// ❌ WRONG: Synchronous I/O blocks UI
public void LoadUsers()
{
    var users = _userService.GetUsers(); // Blocks entire UI thread!
}

// ✅ CORRECT: Async/Await
public async Task LoadUsersAsync()
{
    var users = await _userService.GetUsersAsync();
}
```

---

## ✅ Code Review Checklist

- [ ] All ViewModels inherit from `BaseViewModel`
- [ ] All properties use `[ObservableProperty]` or `ObservableObject`
- [ ] All commands use `[RelayCommand]` attribute
- [ ] Nullable reference types enabled (`#nullable enable`)
- [ ] All async methods return `Task` or `Task<T>`
- [ ] No business logic in code-behind (XAML.cs files)
- [ ] DI container configured in App.xaml.cs
- [ ] Error handling with try-catch and user feedback
- [ ] Logging at appropriate levels
- [ ] XAML bindings use `Binding` with proper modes

---

## 🎓 2025 Features

- ✅ **.NET 10 LTS**: Long-term support through 2027
- ✅ **C# 13/14**: Required keyword, init properties, records
- ✅ **CommunityToolkit.Mvvm**: Generator-based observable properties
- ✅ **Nullable Reference Types**: null-safety by default
- ✅ **MAUI**: Single codebase for iOS, Android, Windows

---

## 📦 Required NuGet Packages

```xml
<ItemGroup>
    <PackageReference Include="CommunityToolkit.Mvvm" Version="8.2.2" />
    <PackageReference Include="Microsoft.Extensions.DependencyInjection" Version="8.1.0" />
    <PackageReference Include="Microsoft.Extensions.Configuration" Version="8.1.0" />
    <PackageReference Include="Microsoft.Extensions.Configuration.Json" Version="8.1.0" />
    <PackageReference Include="System.Net.Http.Json" Version="8.0.0" />
    <PackageReference Include="Serilog" Version="3.1.1" />
    <PackageReference Include="Serilog.Sinks.File" Version="5.0.0" />
</ItemGroup>
```

---

**Document Version**: 1.0 (2025-12-09)
**Framework**: .NET 10 LTS
**Status**: Production-ready
```

---

이제 **Session 3: Slash Commands** 작성을 시작하겠습니다.
