#nullable enable

using System.Globalization;

namespace LmpLink.MAUI.Converters;

/// <summary>
/// Converter for CenterMode to Korean text.
/// </summary>
public class CenterModeToKoreanConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not CenterMode mode)
        {
            return "알 수 없음";
        }

        return mode switch
        {
            CenterMode.User => "이용자 중심",
            CenterMode.Assistant => "활동지원사 중심",
            _ => "알 수 없음"
        };
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
