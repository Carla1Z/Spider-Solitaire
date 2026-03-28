using System;
using System.Globalization;
using Microsoft.Maui.Controls;

namespace SpiderSolitaire.Converters
{
    /// <summary>
    /// Convierte HasWon (bool) al emoji correspondiente al resultado.
    /// </summary>
    public class WinLoseEmojiConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType,
                              object? parameter, CultureInfo culture)
            => value is true ? "🌸✨" : "🍂";

        public object ConvertBack(object? value, Type targetType,
                                  object? parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}