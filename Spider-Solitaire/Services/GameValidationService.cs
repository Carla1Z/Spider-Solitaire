// Servicio dedicado exclusivamente a validar movimientos.
// Extraído de GameService para cumplir Single Responsibility Principle.
using System.Collections.Generic;
using System.Linq;
using SpiderSolitaire.Models;

namespace SpiderSolitaire.Services
{
    /// <summary>
    /// Contiene toda la lógica de validación de movimientos del juego.
    /// GameService delega aquí para no crecer demasiado.
    /// </summary>
    public class GameValidationService
    {
        /// <summary>
        /// Verifica si las cartas desde <paramref name="cardIndex"/> en la columna
        /// origen pueden moverse a la columna destino.
        ///
        /// Reglas de Spider Solitaire:
        /// 1. Las cartas a mover deben estar todas boca arriba.
        /// 2. Las cartas a mover deben formar una secuencia descendente
        ///    (no necesariamente del mismo palo para mover, pero sí para completar).
        /// 3. La carta inferior del grupo a mover debe ser exactamente
        ///    1 menos que la carta superior de la columna destino.
        /// 4. Si la columna destino está vacía, cualquier carta puede ir.
        /// </summary>
        public bool IsValidMove(
            Column source,
            int cardIndex,
            Column target)
        {
            // Validaciones básicas
            if (source.Index == target.Index) return false;
            if (cardIndex < 0 || cardIndex >= source.Cards.Count) return false;

            var cardsToMove = source.Cards.Skip(cardIndex).ToList();

            // Todas las cartas a mover deben estar boca arriba
            if (cardsToMove.Any(c => !c.IsFaceUp)) return false;

            // Las cartas a mover deben formar una secuencia descendente
            if (!IsDescendingSequence(cardsToMove)) return false;

            // Columna destino vacía: siempre válido
            if (target.IsEmpty) return true;

            // La carta inferior del grupo debe ser 1 menos que la top del destino
            var bottomCardToMove = cardsToMove[0];
            var targetTop = target.TopCard!;

            return targetTop.IsFaceUp &&
                   targetTop.Value == bottomCardToMove.Value + 1;
        }

        /// <summary>
        /// Verifica si una lista de cartas forma una secuencia estrictamente
        /// descendente (cada carta tiene valor = anterior - 1).
        /// No requiere mismo palo para poder moverlas (solo para completar).
        /// </summary>
        public bool IsDescendingSequence(List<Card> cards)
        {
            if (cards.Count <= 1) return true;

            for (int i = 0; i < cards.Count - 1; i++)
            {
                if (cards[i].Value != cards[i + 1].Value + 1)
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Determina si hay al menos un movimiento válido en el tablero.
        /// Recorre todas las combinaciones origen-destino posibles.
        /// </summary>
        public bool HasAvailableMoves(List<Column> columns)
        {
            for (int src = 0; src < columns.Count; src++)
            {
                if (columns[src].IsEmpty) continue;

                // El inicio de la secuencia válida en la columna origen
                int seqStart = columns[src].GetValidSequenceStart();

                for (int tgt = 0; tgt < columns.Count; tgt++)
                {
                    if (src == tgt) continue;
                    if (IsValidMove(columns[src], seqStart, columns[tgt]))
                        return true;
                }
            }
            return false;
        }
    }
}