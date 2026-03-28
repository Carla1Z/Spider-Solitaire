// Helpers estáticos para evaluar el estado del juego.
using System.Collections.Generic;
using System.Linq;
using SpiderSolitaire.Models;

namespace SpiderSolitaire.Helpers
{
    /// <summary>
    /// Utilidades para analizar el estado del tablero sin modificarlo.
    /// Usados por GameService y posiblemente por una IA de hints futura.
    /// </summary>
    public static class GameStateHelper
    {
        /// <summary>
        /// Cuenta cuántas cartas están boca arriba en todo el tablero.
        /// </summary>
        public static int CountFaceUpCards(List<Column> columns)
            => columns.Sum(c => c.Cards.Count(card => card.IsFaceUp));

        /// <summary>
        /// Retorna el porcentaje de progreso del juego (0.0 - 1.0).
        /// Basado en secuencias completadas / total posible.
        /// </summary>
        public static double GetProgressPercent(int completedSequences)
            => completedSequences / (double)Constants.GameConstants.TotalSequences;

        /// <summary>
        /// Determina si el tablero está "bloqueado" (no hay movimientos posibles).
        /// </summary>
        public static bool IsBoardLocked(List<Column> columns)
        {
            // Verificar si alguna columna tiene cartas movibles
            foreach (var col in columns)
            {
                if (col.IsEmpty) continue;
                int seqStart = col.GetValidSequenceStart();
                var topCard = col.Cards[seqStart];

                // Verificar si la carta puede ir a alguna columna
                foreach (var target in columns)
                {
                    if (target.Index == col.Index) continue;
                    if (target.IsEmpty) return false; // Siempre hay un lugar vacío

                    if (target.TopCard != null &&
                        target.TopCard.IsFaceUp &&
                        target.TopCard.Value == topCard.Value + 1)
                        return false;
                }
            }
            return true;
        }
    }
}