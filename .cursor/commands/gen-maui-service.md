# Slash Command: /maui-service

Generate a new C# service with interface and implementation for MAUI project.

## Usage

```
/maui-service LocationService
/maui-service ApiService --with-httpclient
/maui-service StorageService --with-preferences
```

## Output Structure

### Services/Interfaces/ILocationService.cs

```csharp
#nullable enable

namespace LmpLink.MAUI.Services;

/// <summary>
/// Location-based operations: distance calculation, radius filtering.
/// </summary>
public interface ILocationService
{
    /// <summary>
    /// Calculate distance between two points using Haversine formula.
    /// </summary>
    /// <param name="lat1">Latitude of point 1</param>
    /// <param name="lon1">Longitude of point 1</param>
    /// <param name="lat2">Latitude of point 2</param>
    /// <param name="lon2">Longitude of point 2</param>
    /// <returns>Distance in kilometers</returns>
    double CalculateDistance(double lat1, double lon1, double lat2, double lon2);

    /// <summary>
    /// Filter persons within a given radius.
    /// </summary>
    /// <param name="center">Center person</param>
    /// <param name="candidates">List of candidates to filter</param>
    /// <param name="radiusKm">Radius in kilometers</param>
    /// <returns>Filtered list of persons within radius</returns>
    List<Person> FilterByRadius(Person center, List<Person> candidates, double radiusKm);
}
```

### Services/Implementation/LocationService.cs

```csharp
using LmpLink.MAUI.Models;

#nullable enable

namespace LmpLink.MAUI.Services;

/// <summary>
/// Location service implementation using Haversine formula.
/// </summary>
public class LocationService : ILocationService
{
    private const double EarthRadiusKm = 6371.0;

    /// <inheritdoc />
    public double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
    {
        var dLat = ToRadians(lat2 - lat1);
        var dLon = ToRadians(lon2 - lon1);

        var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos(ToRadians(lat1)) * Math.Cos(ToRadians(lat2)) *
                Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        return Math.Round(EarthRadiusKm * c, 2); // Round to 2 decimal places
    }

    /// <inheritdoc />
    public List<Person> FilterByRadius(Person center, List<Person> candidates, double radiusKm)
    {
        return candidates
            .Where(p => CalculateDistance(
                center.Latitude, center.Longitude,
                p.Latitude, p.Longitude) <= radiusKm)
            .OrderBy(p => CalculateDistance(
                center.Latitude, center.Longitude,
                p.Latitude, p.Longitude))
            .ToList();
    }

    private static double ToRadians(double degrees) => degrees * Math.PI / 180;
}
```

## Rules

1. **Always create an interface first**: Defines contract for implementation
2. **Use `#nullable enable`**: Required at the top of every file
3. **XML Comments**: Add `/// <summary>`, `/// <param>`, `/// <returns>` for all public APIs
4. **Async I/O**: Use `async Task<T>` for all I/O operations (API, file, DB)
5. **Error Handling**: Throw exceptions with clear messages, let caller handle
6. **Dependency Injection**: Inject other services via constructor
7. **Constants**: Use `const` for magic numbers (e.g., `EarthRadiusKm`)
8. **Naming**: Interface = `IMyService`, Implementation = `MyService`
9. **Return Types**: Use `List<T>` or `IEnumerable<T>`, avoid `var` in public APIs
10. **Unit Testable**: Keep logic pure (no static methods, testable dependencies)

## Registration (MauiProgram.cs)

```csharp
// Add to MauiProgram.cs CreateMauiApp()

// Business Services (Singleton)
builder.Services
    .AddSingleton<ILocationService, LocationService>()
    .AddSingleton<IApiService, ApiService>()
    .AddSingleton<IStorageService, StorageService>();

// OR with HttpClient (for API services)
builder.Services.AddHttpClient<IApiService, ApiService>(client =>
{
    client.BaseAddress = new Uri("https://api.lmplink.com");
    client.DefaultRequestHeaders.Add("User-Agent", "LmpLink/1.0");
    client.Timeout = TimeSpan.FromSeconds(30);
});
```

## Example: API Service with HttpClient

```csharp
// Services/Interfaces/IApiService.cs
public interface IApiService
{
    Task<List<Person>> GetPersonsAsync();
    Task<Person?> GetPersonByIdAsync(int id);
}

// Services/Implementation/ApiService.cs
public class ApiService : IApiService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<ApiService> _logger;

    public ApiService(HttpClient httpClient, ILogger<ApiService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<List<Person>> GetPersonsAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("/api/persons");
            response.EnsureSuccessStatusCode();
            
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<Person>>(json) ?? [];
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to fetch persons");
            throw new ApiException("Failed to fetch persons", ex);
        }
    }

    public async Task<Person?> GetPersonByIdAsync(int id)
    {
        var response = await _httpClient.GetAsync($"/api/persons/{id}");
        if (!response.IsSuccessStatusCode) return null;
        
        var json = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<Person>(json);
    }
}
```

## Example: Storage Service with Preferences

```csharp
// Services/Interfaces/IStorageService.cs
public interface IStorageService
{
    Task<T?> GetAsync<T>(string key);
    Task SetAsync<T>(string key, T value);
    Task RemoveAsync(string key);
}

// Services/Implementation/StorageService.cs
public class StorageService : IStorageService
{
    public Task<T?> GetAsync<T>(string key)
    {
        var json = Preferences.Get(key, string.Empty);
        if (string.IsNullOrEmpty(json)) return Task.FromResult<T?>(default);
        
        return Task.FromResult(JsonSerializer.Deserialize<T>(json));
    }

    public Task SetAsync<T>(string key, T value)
    {
        var json = JsonSerializer.Serialize(value);
        Preferences.Set(key, json);
        return Task.CompletedTask;
    }

    public Task RemoveAsync(string key)
    {
        Preferences.Remove(key);
        return Task.CompletedTask;
    }
}
```

