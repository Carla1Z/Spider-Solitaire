using System;
using System.Globalization;
using Microsoft.Maui.Controls;

namespace SpiderSolitaire.Converters

{
    /// <summary>
    /// Convierte HasWon (bool) al título del resultado del juego.
    /// </summary>
    public class WinLoseTitleConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType,
                              object? parameter, CultureInfo culture)
            => value is true ? "¡Victoria!" : "Fin del juego";

        public object ConvertBack(object? value, Type targetType,
                                  object? parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}