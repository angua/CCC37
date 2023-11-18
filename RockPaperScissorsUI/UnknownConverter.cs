using System;
using System.Globalization;
using System.Windows.Data;

namespace RockPaperScissorsUI;

class UnknownConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if ((bool) value == true)
        {
            return "#fdff5c";
        }
        return "#FFFFFF";
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
