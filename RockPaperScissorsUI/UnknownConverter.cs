using System;
using System.Globalization;
using System.Windows.Data;

namespace RockPaperScissorsUI;

class UnknownConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is UnknownOrAvailable unknownOrAvailable)
        {

            return unknownOrAvailable switch
            {

                UnknownOrAvailable.Available => "#fdff5c",
                UnknownOrAvailable.Unknown => "#eba6ff",
                UnknownOrAvailable.None => "#FFFFFF",
                _ => "#FFFFFF"
            };
        }
        return "#FFFFFF";
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
