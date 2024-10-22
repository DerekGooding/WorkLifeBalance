using System.Globalization;
using System.Windows.Data;

namespace WorkLifeBalance.Converters;

public class IntStringConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        => value is int intValue ? intValue.ToString() : (object)string.Empty;

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => int.TryParse(value as string, out int result) ? result : (object)0;
}
