// Helper para calcular dimensiones y posiciones de cartas en pantalla.
using SpiderSolitaire.Constants;

namespace SpiderSolitaire.Helpers
{
    /// <summary>
    /// Calcula el layout de cartas según el tamaño de pantalla disponible.
    /// Centralizar estos cálculos facilita soporte de múltiples resoluciones.
    /// </summary>
    public static class CardLayoutHelper
    {
        /// <summary>
        /// Calcula el ancho óptimo de una carta según el ancho total del tablero.
        /// Spider tiene 10 columnas con pequeños márgenes entre ellas.
        /// </summary>
        public static double GetCardWidth(double boardWidth)
        {
            // 10 columnas + 11 espacios de margen (2dp cada uno)
            const double margins = 11 * 2.0;
            return (boardWidth - margins) / GameConstants.ColumnCount;
        }

        /// <summary>
        /// Calcula el alto de la carta manteniendo el aspect ratio estándar.
        /// </summary>
        public static double GetCardHeight(double cardWidth)
            => cardWidth / GameConstants.CardAspectRatio;

        /// <summary>
        /// Calcula el offset vertical entre cartas superpuestas en una columna.
        /// Las cartas boca abajo se solapan más para ahorrar espacio.
        /// </summary>
        public static double GetCardOffset(double cardHeight, bool isFaceUp)
            => isFaceUp
                ? cardHeight * GameConstants.CardOverlap * 1.4  // Más visible si está boca arriba
                : cardHeight * GameConstants.CardOverlap * 0.5; // Más compacta si está boca abajo

        /// <summary>
        /// Calcula la altura total que ocupará una columna según su contenido.
        /// Útil para hacer la ScrollView de la columna del tamaño correcto.
        /// </summary>
        public static double GetColumnHeight(
            int faceDownCount,
            int faceUpCount,
            double cardHeight)
        {
            double faceDownHeight = faceDownCount * GetCardOffset(cardHeight, false);
            double faceUpHeight = faceUpCount * GetCardOffset(cardHeight, true);
            return faceDownHeight + faceUpHeight + cardHeight; // +1 carta completa al final
        }
    }
}