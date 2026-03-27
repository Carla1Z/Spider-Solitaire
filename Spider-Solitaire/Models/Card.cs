using System;
using System.Collections.Generic;
using System.Text;

// Representa una carta individual del mazo.
// Es el objeto de dominio central del juego.
namespace SpiderSolitaire.Models
{
    /// <summary>
    /// Define los palos disponibles en Spider Solitaire.
    /// En la modalidad de 1 palo solo se usa Spades.
    /// </summary>
    public enum Suit
    {
        Spades,
        Hearts,
        Diamonds,
        Clubs
    }

    /// <summary>
    /// Valores posibles de una carta (As=1 hasta Rey=13).
    /// </summary>
    public enum Rank
    {
        Ace = 1, Two, Three, Four, Five,
        Six, Seven, Eight, Nine, Ten,
        Jack, Queen, King
    }

    /// <summary>
    /// Entidad de dominio que representa una carta del juego.
    /// Inmutable en su identidad (Suit + Rank), mutable en su estado visual.
    /// </summary>
    public class Card
    {
        // ── Identidad ──────────────────────────────────────────────
        public Suit Suit { get; }
        public Rank Rank { get; }

        // ── Estado visual ──────────────────────────────────────────
        /// <summary>
        /// Indica si la carta está boca arriba (visible al jugador).
        /// </summary>
        public bool IsFaceUp { get; set; }

        /// <summary>
        /// Indica si la carta está siendo arrastrada en un movimiento.
        /// Útil para efectos visuales de drag & drop.
        /// </summary>
        public bool IsDragging { get; set; }

        /// <summary>
        /// Índice de la columna donde reside actualmente la carta.
        /// -1 si está en el stock o fuera del tablero.
        /// </summary>
        public int ColumnIndex { get; set; } = -1;

        /// <summary>
        /// Posición dentro de la columna (0 = carta más al fondo).
        /// </summary>
        public int PositionInColumn { get; set; } = -1;

        // ── Constructor ────────────────────────────────────────────
        public Card(Suit suit, Rank rank)
        {
            Suit = suit;
            Rank = rank;
            IsFaceUp = false;
        }

        // ── Helpers ────────────────────────────────────────────────
        /// <summary>
        /// Representación legible para debugging.
        /// </summary>
        public override string ToString() =>
            $"{Rank} of {Suit}{(IsFaceUp ? " (face up)" : " (face down)")}";

        /// <summary>
        /// Valor numérico de la carta para comparaciones de secuencia.
        /// </summary>
        public int Value => (int)Rank;

        /// <summary>
        /// Símbolo unicode del palo para renderizado rápido en texto.
        /// </summary>
        public string SuitSymbol => Suit switch
        {
            Suit.Spades => "♠",
            Suit.Hearts => "♥",
            Suit.Diamonds => "♦",
            Suit.Clubs => "♣",
            _ => "?"
        };

        /// <summary>
        /// Etiqueta corta del valor (A, 2-10, J, Q, K).
        /// </summary>
        public string RankLabel => Rank switch
        {
            Rank.Ace => "A",
            Rank.Jack => "J",
            Rank.Queen => "Q",
            Rank.King => "K",
            _ => ((int)Rank).ToString()
        };

        /// <summary>
        /// Clave única para identificar la carta (útil en diccionarios/logs).
        /// </summary>
        public string Key => $"{RankLabel}{SuitSymbol}";
    }
}
