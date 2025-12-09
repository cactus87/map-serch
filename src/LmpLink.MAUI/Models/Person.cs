#nullable enable

namespace LmpLink.MAUI.Models;

/// <summary>
/// Person type: User (이용자) or Assistant (지원사).
/// </summary>
public enum PersonType
{
    /// <summary>이용자</summary>
    User,
    
    /// <summary>지원사</summary>
    Assistant
}

/// <summary>
/// Represents a person (user or assistant) with location data.
/// </summary>
/// <param name="Id">Unique identifier</param>
/// <param name="Name">Person's name</param>
/// <param name="Type">Person type (User or Assistant)</param>
/// <param name="Latitude">Location latitude</param>
/// <param name="Longitude">Location longitude</param>
/// <param name="Address">Full address</param>
/// <param name="Phone">Phone number (optional)</param>
/// <param name="Gender">Gender: "male", "female", or null</param>
/// <param name="HasVehicle">Whether the person has a vehicle</param>
/// <param name="AvailableTimeSlots">Available time slots (e.g., "weekday_am", "weekend")</param>
/// <param name="Experience">Experience description (for assistants)</param>
public record Person(
    int Id,
    string Name,
    PersonType Type,
    double Latitude,
    double Longitude,
    string Address,
    string? Phone = null,
    string? Gender = null,
    bool HasVehicle = false,
    string? AvailableTimeSlots = null,
    string? Experience = null
)
{
    /// <summary>
    /// Distance from a reference point (calculated dynamically).
    /// </summary>
    public double? DistanceKm { get; init; }
}

