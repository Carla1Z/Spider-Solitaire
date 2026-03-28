using System;
using System.Collections.Generic;
using System.Text;

// DTO para transferir datos de una carta hacia la capa de presentación.
// Desacopla el modelo de dominio de la UI — la View nunca toca Card directamente.
namespace SpiderSolitaire.DTOs
{
    /// <summary>
    /// Data Transfer Object de una carta.
    /// Contiene solo la información que la UI necesita para renderizar.
    /// </summary>
    public class CardDto
    {
        // ── Identidad ──────────────────────────────────────────────
        public string Key { get; set; } = string.Empty; // Ej: "A♠"
        public string RankLabel { get; set; } = string.Empty; // Ej: "A", "10", "K"
        public string SuitSymbol { get; set; } = string.Empty; // Ej: "♠"
        public string SuitName { get; set; } = string.Empty; // Ej: "Spades"
        public int RankValue { get; set; }                 // 1–13

        // ── Estado visual ──────────────────────────────────────────
        public bool IsFaceUp { get; set; }
        public bool IsDragging { get; set; }

        // ── Posición en el tablero ─────────────────────────────────
        public int ColumnIndex { get; set; }
        public int PositionInColumn { get; set; }

        // ── Propiedades de color para la UI ───────────────────────
        /// <summary>
        /// True si el palo es rojo (Hearts/Diamonds).
        /// La UI lo usa para elegir el color del texto de la carta.
        /// </summary>
        public bool IsRed => SuitName is "Hearts" or "Diamonds";

        /// <summary>
        /// True si esta carta puede ser cabeza de una secuencia arrastrable.
        /// Se calcula en el mapper según el estado de la columna.
        /// </summary>
        public bool IsDraggable { get; set; }
    }
}
