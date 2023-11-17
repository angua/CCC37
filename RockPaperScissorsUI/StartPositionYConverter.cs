using System;
using System.Globalization;
using System.Windows.Data;

namespace RockPaperScissorsUI;

class StartPositionYConverter : IMultiValueConverter
{
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        // StartPositionY, FighterWidth, FighterHeight, DistanceX, DistanceY
        // StartPositionY * DistanceY + FighterHeight

        return System.Convert.ToDouble(values[0]) * System.Convert.ToDouble(values[4]) + System.Convert.ToDouble(values[2]);
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
