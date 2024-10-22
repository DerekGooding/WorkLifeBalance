using System.Globalization;
using System.Windows.Data;

namespace WorkLifeBalance.Converters;

public class TimeOnlyStringConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        => value is TimeOnly time
            ? (object)time.ToString("HH:mm:ss")
            : throw new Exception("Value is not of type TimeOnly");

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is string time)
        {
            string[] timeData = time.Split(':' , StringSplitOptions.RemoveEmptyEntries);
            Console.WriteLine(timeData.Length);
            foreach(string timer in timeData)
            {
                Console.WriteLine(timer);
            }
            return new TimeOnly
                (
                    int.Parse(timeData[0]),
                    int.Parse(timeData[1]),
                    int.Parse(timeData[2])
                );
        }
        throw new Exception("Value is not of type string");
    }
}
