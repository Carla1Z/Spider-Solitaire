using System;
using System.Collections.Generic;
using System.Text;

// Responsable de crear y mezclar los mazos del juego.
// Spider Solitaire usa 2 mazos de 52 cartas = 104 cartas totales.
using System;
using System.Collections.Generic;

namespace SpiderSolitaire.Models
{
    /// <summary>
    /// Fábrica de mazos. Crea mazos configurados según la dificultad.
    /// Separado del dominio para cumplir Single Responsibility.
    /// </summary>
    public static class DeckFactory
    {
        /// <summary>
        /// Genera y mezcla 104 cartas según la dificultad elegida.
        /// - 1 palo:  104 cartas de Spades
        /// - 2 palos: 52 Spades + 52 Hearts
        /// - 4 palos: 2 mazos completos estándar
        /// </summary>
        public static List<Card> CreateShuffledDeck(Difficulty difficulty, int? seed = null)
        {
            var rng = seed.HasValue ? new Random(seed.Value) : new Random();
            var deck = BuildDeck(difficulty);
            Shuffle(deck, rng);
            return deck;
        }

        // ── Privados ───────────────────────────────────────────────
        private static List<Card> BuildDeck(Difficulty difficulty)
        {
            var cards = new List<Card>(104);

            // Determinamos qué palos usar según la dificultad
            var suits = difficulty switch
            {
                Difficulty.OneSuit => new[] { Suit.Spades },
                Difficulty.TwoSuits => new[] { Suit.Spades, Suit.Hearts },
                Difficulty.FourSuits => new[] { Suit.Spades, Suit.Hearts, Suit.Diamonds, Suit.Clubs },
                _ => new[] { Suit.Spades }
            };

            // Calculamos cuántas veces repetir cada palo para llegar a 104 cartas
            // 1 palo: 8 repeticiones × 13 = 104
            // 2 palos: 4 repeticiones × 13 × 2 = 104
            // 4 palos: 2 repeticiones × 13 × 4 = 104
            int repetitions = 8 / suits.Length;

            foreach (var suit in suits)
                for (int r = 0; r < repetitions; r++)
                    foreach (Rank rank in Enum.GetValues<Rank>())
                        cards.Add(new Card(suit, rank));

            return cards;
        }

        private static void Shuffle<T>(List<T> list, Random rng)
        {
            // Fisher-Yates shuffle — distribución uniforme garantizada
            for (int i = list.Count - 1; i > 0; i--)
            {
                int j = rng.Next(i + 1);
                (list[i], list[j]) = (list[j], list[i]);
            }
        }
    }
}
