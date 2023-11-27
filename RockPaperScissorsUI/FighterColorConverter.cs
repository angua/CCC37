using System;
using System.Globalization;
using System.Windows.Data;

namespace RockPaperScissorsUI;

class FighterColorConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value switch
        {
            "R" => "#66686d",
            "P" => "#02d8db",
            "S" => "#0066ff",
            "Y" => "#ff6000",
            "L" => "#03af2b",
            "X" => "#fcd500",
            "Z" => "#ab58ff",
            _ => "#000000"
        };
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
