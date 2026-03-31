// Métodos de extensión para Card que no pertenecen al dominio central.
using System.Collections.Generic;
using System.Linq;
using SpiderSolitaire.Constants;   // ← MOVIDO AQUÍ ARRIBA
using SpiderSolitaire.Models;

namespace SpiderSolitaire.Extensions
{
    /// <summary>
    /// Extensiones de utilidad para trabajar con cartas y colecciones de cartas.
    /// </summary>
    public static class CardExtensions
    {
        /// <summary>
        /// Verifica si una lista de cartas forma una secuencia válida del mismo palo.
        /// </summary>
        public static bool IsSameSuitSequence(this List<Card> cards)
        {
            if (cards.Count == 0) return false;
            if (cards.Count == 1) return true;

            var suit = cards[0].Suit;
            for (int i = 0; i < cards.Count - 1; i++)
            {
                if (cards[i].Suit != suit) return false;
                if (!cards[i].IsFaceUp) return false;
                if (cards[i].Value != cards[i + 1].Value + 1) return false;
            }
            return cards[^1].Suit == suit && cards[^1].IsFaceUp;
        }

        /// <summary>
        /// Verifica si una lista forma secuencia descendente
        /// (independiente del palo — para validar movimientos).
        /// </summary>
        public static bool IsDescendingSequence(this List<Card> cards)
        {
            for (int i = 0; i < cards.Count - 1; i++)
                if (cards[i].Value != cards[i + 1].Value + 1)
                    return false;
            return true;
        }

        /// <summary>
        /// Retorna el color hex según el palo de la carta.
        /// </summary>
        public static string GetDisplayColor(this Card card)
            => card.Suit is Suit.Hearts or Suit.Diamonds
                ? AppColors.TextRed
                : AppColors.TextBlack;

        /// <summary>
        /// Devuelve true si source puede apilarse sobre target en Spider.
        /// </summary>
        public static bool CanStackOn(this Card source, Card target)
            => target.IsFaceUp && target.Value == source.Value + 1;
    }
}