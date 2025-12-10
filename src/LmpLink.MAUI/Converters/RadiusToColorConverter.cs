namespace LmpLink.MAUI.Converters;

/// <summary>
/// Converts current radius to button background color.
/// Returns primary color if radius matches, secondary color otherwise.
/// </summary>
public class RadiusToColorConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, System.Globalization.CultureInfo culture)
    {
        if (value is not double currentRadius || parameter is not string paramStr)
            return Color.FromArgb("#4B5563"); // Default gray
            
        if (!double.TryParse(paramStr, out var targetRadius))
            return Color.FromArgb("#4B5563");

        // Selected: primary blue, Unselected: gray
        return Math.Abs(currentRadius - targetRadius) < 0.01
            ? Color.FromArgb("#3B82F6")  // PrimaryCTA blue
            : Color.FromArgb("#4B5563"); // Gray
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, System.Globalization.CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
