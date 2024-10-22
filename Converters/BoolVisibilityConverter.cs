using System.Globalization;
using System.Windows.Data;

namespace WorkLifeBalance.Converters;

public class BoolVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        => value is bool enabled
            ? (object)(enabled ? Visibility.Visible : Visibility.Collapsed)
            : throw new Exception("Value is not of type bool");

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => value is Visibility visible
            ? (object)(visible == Visibility.Visible)
            : throw new Exception("Value is not of type Visibility");
}
