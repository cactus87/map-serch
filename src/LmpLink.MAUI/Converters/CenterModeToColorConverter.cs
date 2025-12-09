#nullable enable

using System.Globalization;

namespace LmpLink.MAUI.Converters;

/// <summary>
/// Converter for Center Mode toggle buttons.
/// Returns primary color if the mode matches, secondary color otherwise.
/// </summary>
public class CenterModeToColorConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not CenterMode currentMode || parameter is not string modeString)
        {
            return Color.FromArgb("#6B7280"); // Default gray
        }

        var paramMode = Enum.Parse<CenterMode>(modeString);
        
        return currentMode == paramMode
            ? Color.FromArgb("#3B82F6") // Primary blue (selected)
            : Color.FromArgb("#6B7280"); // Gray (unselected)
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
