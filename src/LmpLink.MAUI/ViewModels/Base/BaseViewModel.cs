#nullable enable

namespace LmpLink.MAUI.ViewModels.Base;

/// <summary>
/// Base class for all ViewModels.
/// Provides common functionality: IsBusy, error handling, async execution.
/// </summary>
public abstract partial class BaseViewModel : ObservableObject
{
    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private bool _hasError;

    [ObservableProperty]
    private string? _errorMessage;

    /// <summary>
    /// Execute an async operation with loading state and error handling.
    /// </summary>
    protected async Task ExecuteAsync(Func<Task> operation)
    {
        IsLoading = true;
        ClearError();

        try
        {
            await operation();
        }
        catch (Exception ex)
        {
            HandleError(ex);
        }
        finally
        {
            IsLoading = false;
        }
    }

    /// <summary>
    /// Execute an async operation with loading state, error handling, and return value.
    /// </summary>
    protected async Task<T?> ExecuteAsync<T>(Func<Task<T>> operation)
    {
        IsLoading = true;
        ClearError();

        try
        {
            return await operation();
        }
        catch (Exception ex)
        {
            HandleError(ex);
            return default;
        }
        finally
        {
            IsLoading = false;
        }
    }

    /// <summary>
    /// Handle error by setting HasError and ErrorMessage.
    /// Override to customize error handling (e.g., logging, telemetry).
    /// </summary>
    protected virtual void HandleError(Exception ex)
    {
        HasError = true;
        ErrorMessage = ex.Message;
        
        // TODO: Add logging/telemetry in production
        System.Diagnostics.Debug.WriteLine($"[ERROR] {GetType().Name}: {ex.Message}");
    }

    /// <summary>
    /// Clear error state.
    /// </summary>
    [RelayCommand]
    protected void ClearError()
    {
        HasError = false;
        ErrorMessage = null;
    }
}
