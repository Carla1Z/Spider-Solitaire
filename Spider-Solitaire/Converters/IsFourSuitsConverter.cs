using System;
using System.Globalization;
using Microsoft.Maui.Controls;
using SpiderSolitaire.Models;

namespace SpiderSolitaire.Converters
{
    public class IsFourSuitsConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType,
                              object? parameter, CultureInfo culture)
            => value is Difficulty d && d == Difficulty.FourSuits;

        public object ConvertBack(object? value, Type targetType,
                                  object? parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}