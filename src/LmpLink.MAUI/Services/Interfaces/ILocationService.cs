#nullable enable

namespace LmpLink.MAUI.Services.Interfaces;

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
    /// <returns>Filtered list of persons within radius, sorted by distance</returns>
    List<Person> FilterByRadius(Person center, List<Person> candidates, double radiusKm);

    /// <summary>
    /// Sort persons by distance from a center point.
    /// </summary>
    /// <param name="centerLat">Center latitude</param>
    /// <param name="centerLon">Center longitude</param>
    /// <param name="persons">List of persons to sort</param>
    /// <returns>Sorted list with distance property set</returns>
    List<Person> SortByDistance(double centerLat, double centerLon, List<Person> persons);
}

