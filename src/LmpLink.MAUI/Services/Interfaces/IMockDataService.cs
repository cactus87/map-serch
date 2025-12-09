#nullable enable

namespace LmpLink.MAUI.Services.Interfaces;

/// <summary>
/// Mock data service for providing test users and assistants.
/// </summary>
public interface IMockDataService
{
    /// <summary>
    /// Get all users (이용자).
    /// </summary>
    /// <returns>List of 10 users</returns>
    Task<List<Person>> GetUsersAsync();

    /// <summary>
    /// Get all assistants (지원사).
    /// </summary>
    /// <returns>List of 20 assistants</returns>
    Task<List<Person>> GetAssistantsAsync();

    /// <summary>
    /// Get all persons (users + assistants).
    /// </summary>
    /// <returns>List of all 30 persons</returns>
    Task<List<Person>> GetAllPersonsAsync();
}

