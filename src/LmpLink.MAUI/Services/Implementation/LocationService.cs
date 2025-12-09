#nullable enable

namespace LmpLink.MAUI.Services.Implementation;

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
            .Select(p => p with
            {
                DistanceKm = CalculateDistance(
                    center.Latitude, center.Longitude,
                    p.Latitude, p.Longitude)
            })
            .Where(p => p.DistanceKm <= radiusKm)
            .OrderBy(p => p.DistanceKm)
            .ToList();
    }

    /// <inheritdoc />
    public List<Person> SortByDistance(double centerLat, double centerLon, List<Person> persons)
    {
        return persons
            .Select(p => p with
            {
                DistanceKm = CalculateDistance(centerLat, centerLon, p.Latitude, p.Longitude)
            })
            .OrderBy(p => p.DistanceKm)
            .ToList();
    }

    private static double ToRadians(double degrees) => degrees * Math.PI / 180;
}

