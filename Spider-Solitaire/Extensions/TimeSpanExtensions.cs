// Extensiones para formatear TimeSpan en la UI del juego.
using System;

namespace SpiderSolitaire.Extensions
{
    /// <summary>
    /// Formatos de tiempo para mostrar en la UI del juego.
    /// </summary>
    public static class TimeSpanExtensions
    {
        /// <summary>
        /// Formato mm:ss para el cronómetro del juego.
        /// </summary>
        public static string ToGameFormat(this TimeSpan ts)
            => ts.ToString(@"mm\:ss");

        /// <summary>
        /// Formato hh:mm:ss para partidas largas.
        /// </summary>
        public static string ToLongFormat(this TimeSpan ts)
            => ts.TotalHours >= 1
                ? ts.ToString(@"hh\:mm\:ss")
                : ts.ToString(@"mm\:ss");
    }
}