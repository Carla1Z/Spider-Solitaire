// Constantes del dominio del juego Spider Solitaire.
namespace SpiderSolitaire.Constants
{
    /// <summary>
    /// Valores fijos del juego que no deben cambiar en runtime.
    /// </summary>
    public static class GameConstants
    {
        // ── Tablero ────────────────────────────────────────────────
        public const int ColumnCount = 10;   // Columnas en el tablero
        public const int TotalCards = 104;  // 2 mazos × 52
        public const int CardsPerDeal = 10;   // 1 por columna
        public const int MaxDeals = 5;    // Deals disponibles por partida
        public const int SequenceLength = 13;   // K→A = 13 cartas
        public const int TotalSequences = 8;    // Para ganar (2 mazos × 4 palos)
        public const int InitialColsLarge = 4;    // Columnas con 6 cartas al inicio
        public const int InitialColsSmall = 6;    // Columnas con 5 cartas al inicio
        public const int InitialCardsLarge = 6;    // Cartas en columnas grandes
        public const int InitialCardsSmall = 5;    // Cartas en columnas pequeñas

        // ── Puntaje ────────────────────────────────────────────────
        public const int InitialScore = 500;
        public const int MoveScore = -1;
        public const int SequenceScore = 100;
        public const int DealPenalty = -10;
        public const int UndoPenalty = -5;

        // ── UI ─────────────────────────────────────────────────────
        public const int MaxUndoHistory = 10;   // Límite de undos
        public const double CardAspectRatio = 0.69; // Ancho/alto estándar de carta
        public const double CardOverlap = 0.25; // Porcentaje de solapamiento visible

        // ── Animaciones ────────────────────────────────────────────
        public const uint AnimationDuration = 200; // ms
        public const uint DealAnimationDuration = 400; // ms
    }
}