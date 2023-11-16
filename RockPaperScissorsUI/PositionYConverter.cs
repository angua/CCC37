using System;
using System.Globalization;
using System.Windows.Data;

namespace RockPaperScissorsUI;

class PositionYConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return (double)parameter * (double)value;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
