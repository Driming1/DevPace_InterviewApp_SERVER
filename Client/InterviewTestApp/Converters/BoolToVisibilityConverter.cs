using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace InterviewTestApp.Converters;

public class BoolToVisibilityConverter : IValueConverter 
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if(value is bool boolValue)
        {
            return boolValue ? Visibility.Visible : Visibility.Collapsed;
        }

        throw new InvalidOperationException($"Invalid type of {nameof(value)}");
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new InvalidOperationException("Use OneWay binding mode");
    }
}