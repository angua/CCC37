using System;
using System.Globalization;
using System.Windows.Data;

namespace RockPaperScissorsUI;

class EndPositionXConverter : IMultiValueConverter
{
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        // EndPositionX, FighterWidth, FighterHeight, DistanceX, DistanceY
        // EndPositionX * DistanceX + 1/2 * FighterWidth

        return System.Convert.ToDouble(values[0]) * System.Convert.ToDouble(values[3]) + 0.5 * System.Convert.ToDouble(values[1]);
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
