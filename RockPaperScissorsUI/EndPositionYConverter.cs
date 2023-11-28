using System;
using System.Globalization;
using System.Windows.Data;

namespace RockPaperScissorsUI;

class EndPositionYConverter : IMultiValueConverter
{
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        // EndPositionY, FighterWidth, FighterHeight, DistanceX, DistanceY
        // EndPositionY * DistanceY

        return System.Convert.ToDouble(values[0]) * System.Convert.ToDouble(values[4]);
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
