using System;
using System.Collections.Generic;
using System.Text;

// Representa una de las 10 columnas del tablero de Spider Solitaire.
// Gestiona la pila de cartas y expone operaciones de dominio.
using System.Collections.Generic;
using System.Linq;

namespace SpiderSolitaire.Models
{
    /// <summary>
    /// Una columna del tablero. Contiene una pila de cartas
    /// donde solo las cartas boca arriba son jugables.
    /// </summary>
    public class Column
    {
        // ── Estado ─────────────────────────────────────────────────
        /// <summary>
        /// Lista ordenada de cartas: índice 0 = carta más al fondo (primera en llegar).
        /// La última carta de la lista es la carta superior (top).
        /// </summary>
        public List<Card> Cards { get; private set; } = new();

        /// <summary>
        /// Índice de esta columna en el tablero (0–9).
        /// </summary>
        public int Index { get; set; }

        // ── Propiedades derivadas ──────────────────────────────────
        /// <summary>
        /// Carta visible en la cima de la columna.
        /// Null si la columna está vacía.
        /// </summary>
        public Card? TopCard => Cards.LastOrDefault();

        /// <summary>
        /// True si la columna no tiene ninguna carta.
        /// </summary>
        public bool IsEmpty => Cards.Count == 0;

        /// <summary>
        /// Cantidad total de cartas en la columna.
        /// </summary>
        public int Count => Cards.Count;

        // ── Operaciones de dominio ─────────────────────────────────
        /// <summary>
        /// Agrega una carta a la cima de la columna.
        /// Actualiza los metadatos de posición de la carta.
        /// </summary>
        public void Push(Card card)
        {
            card.ColumnIndex = Index;
            card.PositionInColumn = Cards.Count;
            Cards.Add(card);
        }

        /// <summary>
        /// Agrega múltiples cartas a la cima (en orden de la lista).
        /// </summary>
        public void PushRange(IEnumerable<Card> cards)
        {
            foreach (var card in cards)
                Push(card);
        }

        /// <summary>
        /// Quita y retorna la carta de la cima.
        /// </summary>
        public Card? Pop()
        {
            if (IsEmpty) return null;
            var card = Cards[^1];
            Cards.RemoveAt(Cards.Count - 1);
            card.ColumnIndex = -1;
            card.PositionInColumn = -1;
            return card;
        }

        /// <summary>
        /// Obtiene las cartas desde una posición hasta la cima (inclusive).
        /// Usado para mover secuencias.
        /// </summary>
        public List<Card> GetCardsFrom(int fromIndex)
        {
            if (fromIndex < 0 || fromIndex >= Cards.Count)
                return new List<Card>();
            return Cards.Skip(fromIndex).ToList();
        }

        /// <summary>
        /// Remueve las cartas desde una posición hasta la cima.
        /// </summary>
        public void RemoveCardsFrom(int fromIndex)
        {
            if (fromIndex < 0 || fromIndex >= Cards.Count) return;
            Cards.RemoveRange(fromIndex, Cards.Count - fromIndex);

            // Recalcular posiciones de las cartas restantes
            for (int i = 0; i < Cards.Count; i++)
                Cards[i].PositionInColumn = i;
        }

        /// <summary>
        /// Revela la carta superior si está boca abajo.
        /// Se llama automáticamente después de cada movimiento.
        /// </summary>
        public void FlipTopCard()
        {
            if (TopCard != null && !TopCard.IsFaceUp)
                TopCard.IsFaceUp = true;
        }

        /// <summary>
        /// Verifica si existe una secuencia completa del mismo palo en la cima
        /// (K→Q→J→...→A del mismo palo = 13 cartas).
        /// </summary>
        public bool HasCompleteSequence()
        {
            if (Cards.Count < 13) return false;

            var topThirteen = Cards.TakeLast(13).ToList();
            var suit = topThirteen[0].Suit;

            for (int i = 0; i < topThirteen.Count; i++)
            {
                var card = topThirteen[i];
                // Deben ser del mismo palo, estar boca arriba y en secuencia descendente
                if (card.Suit != suit || !card.IsFaceUp)
                    return false;
                if ((int)card.Rank != 13 - i) // K=13, Q=12, ..., A=1
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Devuelve el índice en Cards de la primera carta de
        /// la secuencia válida (misma suit, descendente) desde la cima.
        /// </summary>
        public int GetValidSequenceStart()
        {
            if (IsEmpty) return -1;

            int startIndex = Cards.Count - 1;

            for (int i = Cards.Count - 2; i >= 0; i--)
            {
                var upper = Cards[i];
                var lower = Cards[i + 1];

                // La secuencia se rompe si: carta boca abajo,
                // distinto palo, o no es consecutiva descendente
                if (!upper.IsFaceUp || !lower.IsFaceUp)
                    break;
                if (upper.Suit != lower.Suit)
                    break;
                if (upper.Value != lower.Value + 1)
                    break;

                startIndex = i;
            }

            return startIndex;
        }
    }
}
