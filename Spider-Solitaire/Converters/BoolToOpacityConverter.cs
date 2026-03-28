// Convierte bool → double de opacidad (true=1.0, false=0.4).
using System;
using System.Globalization;
using Microsoft.Maui.Controls;

namespace SpiderSolitaire.Converters
{
    /// <summary>
    /// Convierte un booleano a un valor de opacidad.
    /// Útil para deshabilitar visualmente botones sin ocultarlos.
    /// </summary>
    public class BoolToOpacityConverter : IValueConverter
    {
        public double TrueOpacity { get; set; } = 1.0;
        public double FalseOpacity { get; set; } = 0.4;

        public object Convert(object? value, Type targetType,
                              object? parameter, CultureInfo culture)
            => value is true ? TrueOpacity : FalseOpacity;

        public object ConvertBack(object? value, Type targetType,
                                  object? parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}