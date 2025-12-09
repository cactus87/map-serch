namespace LmpLink.MAUI.Services.Interfaces;

/// <summary>
/// Naver Maps JS Interop service for WebView communication.
/// Handles map initialization, marker rendering, and circle drawing.
/// </summary>
public interface IMapService
{
    /// <summary>
    /// Set the WebView reference for JS Interop.
    /// Must be called before any map operations.
    /// </summary>
    /// <param name="webView">WebView instance</param>
    void SetWebView(WebView webView);

    /// <summary>
    /// Initialize the Naver Map with center coordinates and zoom level.
    /// </summary>
    /// <param name="lat">Latitude (default: 37.6688 for Dobong-gu Office)</param>
    /// <param name="lng">Longitude (default: 127.0471 for Dobong-gu Office)</param>
    /// <param name="zoom">Zoom level (default: 13)</param>
    Task InitMapAsync(double lat = 37.6688, double lng = 127.0471, int zoom = 13);

    /// <summary>
    /// Add a marker to the map.
    /// </summary>
    /// <param name="person">Person object containing location and metadata</param>
    Task AddMarkerAsync(Person person);

    /// <summary>
    /// Add multiple markers in batch.
    /// </summary>
    /// <param name="persons">List of persons to add as markers</param>
    Task AddMarkersAsync(IEnumerable<Person> persons);

    /// <summary>
    /// Draw a circle (radius overlay) on the map.
    /// </summary>
    /// <param name="lat">Center latitude</param>
    /// <param name="lng">Center longitude</param>
    /// <param name="radiusKm">Radius in kilometers</param>
    Task DrawCircleAsync(double lat, double lng, double radiusKm);

    /// <summary>
    /// Clear the circle overlay from the map.
    /// </summary>
    Task ClearCircleAsync();

    /// <summary>
    /// Set marker visibility by person ID.
    /// </summary>
    /// <param name="personId">Person ID (1-based)</param>
    /// <param name="visible">True to show, false to hide</param>
    Task SetMarkerVisibleAsync(int personId, bool visible);

    /// <summary>
    /// Show all markers on the map.
    /// </summary>
    Task ShowAllMarkersAsync();

    /// <summary>
    /// Clear all markers from the map.
    /// </summary>
    Task ClearAllMarkersAsync();

    /// <summary>
    /// Get current map center coordinates.
    /// </summary>
    /// <returns>Tuple of (lat, lng)</returns>
    Task<(double lat, double lng)> GetMapCenterAsync();

    /// <summary>
    /// Set map center to specific coordinates.
    /// </summary>
    /// <param name="lat">Latitude</param>
    /// <param name="lng">Longitude</param>
    Task SetMapCenterAsync(double lat, double lng);

    /// <summary>
    /// Focus on a marker (center map + zoom + open InfoWindow).
    /// </summary>
    /// <param name="personId">Person ID</param>
    Task FocusMarkerAsync(int personId);
}
