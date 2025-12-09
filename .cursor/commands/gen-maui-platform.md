# Slash Command: /maui-platform

Generate platform-specific code for Android or Windows in MAUI project.

## Usage

```
/maui-platform PlatformService --android
/maui-platform PlatformService --windows
/maui-platform PlatformService --both
```

## Output Structure

### Services/Interfaces/IPlatformService.cs

```csharp
#nullable enable

namespace LmpLink.MAUI.Services;

/// <summary>
/// Platform-specific operations (Android, Windows).
/// </summary>
public interface IPlatformService
{
    /// <summary>
    /// Get device unique identifier.
    /// </summary>
    string GetDeviceId();

    /// <summary>
    /// Request runtime permission (Android 6.0+).
    /// </summary>
    Task<PermissionStatus> RequestLocationPermissionAsync();

    /// <summary>
    /// Check if device has network connectivity.
    /// </summary>
    bool IsConnected();

    /// <summary>
    /// Get device platform name.
    /// </summary>
    string GetPlatformName();
}
```

### Platforms/Android/AndroidPlatformService.cs

```csharp
#if ANDROID
using Android.Content.PM;
using Android.OS;

#nullable enable

namespace LmpLink.MAUI.Platforms.Android;

/// <summary>
/// Android-specific platform service implementation.
/// </summary>
public class AndroidPlatformService : IPlatformService
{
    public string GetDeviceId()
    {
        return Build.Serial ?? "unknown";
    }

    public async Task<PermissionStatus> RequestLocationPermissionAsync()
    {
        try
        {
            var status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
            if (status != PermissionStatus.Granted)
            {
                status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
            }
            return status;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Permission error: {ex}");
            return PermissionStatus.Denied;
        }
    }

    public bool IsConnected()
    {
        return Connectivity.Current.NetworkAccess == NetworkAccess.Internet;
    }

    public string GetPlatformName()
    {
        return $"Android {Build.VERSION.Release}";
    }
}
#endif
```

### Platforms/Windows/WindowsPlatformService.cs

```csharp
#if WINDOWS
#nullable enable

namespace LmpLink.MAUI.Platforms.Windows;

/// <summary>
/// Windows-specific platform service implementation.
/// </summary>
public class WindowsPlatformService : IPlatformService
{
    public string GetDeviceId()
    {
        return Environment.MachineName;
    }

    public Task<PermissionStatus> RequestLocationPermissionAsync()
    {
        // Windows desktop apps don't require runtime location permission
        return Task.FromResult(PermissionStatus.Granted);
    }

    public bool IsConnected()
    {
        return Connectivity.Current.NetworkAccess == NetworkAccess.Internet;
    }

    public string GetPlatformName()
    {
        return $"Windows {Environment.OSVersion.Version}";
    }
}
#endif
```

## Registration (MauiProgram.cs)

```csharp
// Add to MauiProgram.cs CreateMauiApp()

// Platform-Specific Services (Conditional Compilation)
#if ANDROID
builder.Services.AddSingleton<IPlatformService, Platforms.Android.AndroidPlatformService>();
#elif WINDOWS
builder.Services.AddSingleton<IPlatformService, Platforms.Windows.WindowsPlatformService>();
#elif IOS
builder.Services.AddSingleton<IPlatformService, Platforms.iOS.iOSPlatformService>();
#endif
```

## Rules

1. **Use `#if` Directives**: Wrap platform code in `#if ANDROID`, `#if WINDOWS`, etc.
2. **Namespace Convention**: `Platforms.Android`, `Platforms.Windows`, `Platforms.iOS`
3. **Interface First**: Define shared interface in root `Services/Interfaces/`
4. **File Location**: Platform code goes in `Platforms/{PlatformName}/` folder
5. **Null Safety**: Use `#nullable enable` in all files
6. **Error Handling**: Catch platform-specific exceptions, return safe defaults
7. **Permissions**: Always check + request permissions on Android
8. **MAUI APIs**: Prefer MAUI cross-platform APIs (e.g., `Connectivity`, `Permissions`) over platform APIs
9. **Debug Logs**: Use `System.Diagnostics.Debug.WriteLine` for platform-specific logs
10. **Testing**: Create mock implementations for unit testing

## Common Platform-Specific Scenarios

### Android Permissions

```csharp
// AndroidManifest.xml (Platforms/Android/)
<uses-permission android:name="android.permission.ACCESS_FINE_LOCATION" />
<uses-permission android:name="android.permission.ACCESS_COARSE_LOCATION" />
<uses-permission android:name="android.permission.INTERNET" />

// Request at runtime
public async Task<bool> RequestCameraPermissionAsync()
{
    var status = await Permissions.CheckStatusAsync<Permissions.Camera>();
    if (status != PermissionStatus.Granted)
    {
        status = await Permissions.RequestAsync<Permissions.Camera>();
    }
    return status == PermissionStatus.Granted;
}
```

### Windows File Picker

```csharp
#if WINDOWS
public async Task<string?> PickFileAsync()
{
    var result = await FilePicker.PickAsync(new PickOptions
    {
        PickerTitle = "Select a file",
        FileTypes = FilePickerFileType.Images
    });
    return result?.FullPath;
}
#endif
```

### Platform-Specific UI (XAML)

```xml
<!-- Show different UI per platform -->
<StackLayout>
    <OnPlatform x:TypeArguments="View">
        <On Platform="Android">
            <Label Text="Android UI" />
        </On>
        <On Platform="WinUI">
            <Label Text="Windows UI" />
        </On>
    </OnPlatform>
</StackLayout>
```

## Anti-Patterns

```csharp
// ❌ WRONG: Mixing platform code in shared code
public class BadService
{
    public void DoSomething()
    {
        #if ANDROID
        // Android code
        #elif WINDOWS
        // Windows code
        #endif
    }
}

// ✅ CORRECT: Interface + platform-specific implementation
public interface IPlatformService { void DoSomething(); }
```

## Notes

- **Platform Folders**: `Platforms/Android/`, `Platforms/Windows/`, `Platforms/iOS/`
- **Conditional Compilation**: `#if ANDROID`, `#if WINDOWS`, `#if IOS`
- **MAUI Cross-Platform APIs**: `Connectivity`, `Permissions`, `FilePicker`, `Geolocation`
- **Testing**: Create `MockPlatformService` for unit tests

