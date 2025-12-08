namespace MapDemo.Services;

public class DistanceCalculator
{
    private const double EarthRadiusKm = 6371.0;

    /// <summary>
    /// Haversine 공식을 사용하여 두 좌표 간의 거리를 계산합니다.
    /// </summary>
    /// <param name="lat1">첫 번째 지점의 위도</param>
    /// <param name="lng1">첫 번째 지점의 경도</param>
    /// <param name="lat2">두 번째 지점의 위도</param>
    /// <param name="lng2">두 번째 지점의 경도</param>
    /// <returns>두 지점 간의 거리 (km)</returns>
    public static double CalculateDistance(double lat1, double lng1, double lat2, double lng2)
    {
        var dLat = DegreesToRadians(lat2 - lat1);
        var dLng = DegreesToRadians(lng2 - lng1);

        var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos(DegreesToRadians(lat1)) * Math.Cos(DegreesToRadians(lat2)) *
                Math.Sin(dLng / 2) * Math.Sin(dLng / 2);

        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        var distance = EarthRadiusKm * c;

        return distance;
    }

    private static double DegreesToRadians(double degrees)
    {
        return degrees * Math.PI / 180.0;
    }
}
