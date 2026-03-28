using System;
using System.Globalization;
using Microsoft.Maui.Controls;
using SpiderSolitaire.Constants;
using SpiderSolitaire.Models;

namespace SpiderSolitaire.Converters
{
    /// <summary>
    /// Convierte Difficulty al color de fondo del botón de dificultad.
    /// </summary>
    public class DifficultyToColorConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType,
                              object? parameter, CultureInfo culture)
        {
            if (value is Difficulty diff)
            {
                return diff switch
                {
                    Difficulty.OneSuit => Color.FromArgb(AppColors.ButtonSecondary),
                    Difficulty.TwoSuits => Color.FromArgb(AppColors.ButtonPrimary),
                    Difficulty.FourSuits => Color.FromArgb(AppColors.ButtonDanger),
                    _ => Color.FromArgb(AppColors.ButtonSecondary)
                };
            }
            return Color.FromArgb(AppColors.ButtonSecondary);
        }

        public object ConvertBack(object? value, Type targetType,
                                  object? parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}