namespace LmpLink.MAUI.Services.Interfaces;

/// <summary>
/// Supabase database operations for persons (users & assistants)
/// </summary>
public interface ISupabaseService
{
    /// <summary>
    /// Initialize Supabase client
    /// </summary>
    Task InitializeAsync();

    /// <summary>
    /// Get all persons (users + assistants)
    /// </summary>
    Task<List<Person>> GetPersonsAsync();

    /// <summary>
    /// Get users only
    /// </summary>
    Task<List<Person>> GetUsersAsync();

    /// <summary>
    /// Get assistants only
    /// </summary>
    Task<List<Person>> GetAssistantsAsync();

    /// <summary>
    /// Get person by ID
    /// </summary>
    Task<Person?> GetPersonByIdAsync(int id);

    /// <summary>
    /// Create a new person
    /// </summary>
    Task<Person> CreatePersonAsync(Person person);

    /// <summary>
    /// Update an existing person
    /// </summary>
    Task<Person> UpdatePersonAsync(Person person);

    /// <summary>
    /// Create or update a person (legacy method)
    /// </summary>
    Task<Person?> UpsertPersonAsync(Person person);

    /// <summary>
    /// Delete a person
    /// </summary>
    Task<bool> DeletePersonAsync(int id);
}







