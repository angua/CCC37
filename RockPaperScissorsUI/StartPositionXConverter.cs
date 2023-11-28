using System;
using System.Globalization;
using System.Windows.Data;

namespace RockPaperScissorsUI;

class StartPositionXConverter : IMultiValueConverter
{
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        // StartPositionX, FighterWidth, FighterHeight, DistanceX, DistanceY
        // StartPositionX * DistanceX + 1/2 * FighterWidth

        return System.Convert.ToDouble(values[0]) * System.Convert.ToDouble(values[3]) + 0.5 * System.Convert.ToDouble(values[1]);
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
