using System.Globalization;
using System.Windows.Data;

namespace WorkLifeBalance.Converters;

public class DateOnlyStringConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        => value is DateOnly date
            ? (object)date.ToString("MM/dd/yyyy")
            : throw new Exception("Value is not of type DateOnly");

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if(value is string date)
        {
            string[] dateData = date.Split('-', StringSplitOptions.RemoveEmptyEntries);
            return new DateOnly
                (
                    int.Parse(dateData[2]),
                    int.Parse(dateData[0]),
                    int.Parse(dateData[1])
                );
        }
        throw new Exception("Value is not of type string");
    }
}
