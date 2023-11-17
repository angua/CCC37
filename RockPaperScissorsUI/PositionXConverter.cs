using System;
using System.Globalization;
using System.Windows.Data;

namespace RockPaperScissorsUI;

class PositionXConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return (double)parameter * System.Convert.ToDouble(value);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
