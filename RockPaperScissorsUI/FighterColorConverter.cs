﻿using System;
using System.Globalization;
using System.Windows.Data;

namespace RockPaperScissorsUI;

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
            'X' => "#fcd500",
            _ => "#000000"
        };
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}