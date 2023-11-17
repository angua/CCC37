using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace RockPaperScissorsUI
{
    class FighterColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value switch
            {
                'R' => "#494d53",
                'P' => "#00f3f6",
                'S' => "#0066ff",
                'Y' => "#ff6000",
                'L' => "#1adf00",
                _ => "#000000"

            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
