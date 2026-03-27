using System;
using System.Collections.Generic;
using System.Text;

// Representa un movimiento realizado en el juego.
// Sirve para el historial, validación y cálculo de puntaje.
using System.Collections.Generic;

namespace SpiderSolitaire.Models
{
    /// <summary>
    /// Tipo de movimiento ejecutado.
    /// </summary>
    public enum MoveType
    {
        CardMove,       // Mover carta(s) de una columna a otra
        Deal,           // Repartir cartas del stock
        SequenceRemove, // Remover secuencia completa del tablero
        Undo            // Deshacer movimiento anterior
    }

    /// <summary>
    /// Registra un movimiento: origen, destino y cartas involucradas.
    /// Permite calcular puntaje y revertir el movimiento (Undo).
    /// </summary>
    public class Move
    {
        public MoveType Type { get; set; }
        public int SourceColumn { get; set; }
        public int TargetColumn { get; set; }
        public int CardCount { get; set; }

        /// <summary>
        /// Snapshot de las cartas movidas (para Undo).
        /// </summary>
        public List<Card> Cards { get; set; } = new();

        /// <summary>
        /// Carta que se reveló en la columna origen tras el movimiento.
        /// </summary>
        public Card? RevealedCard { get; set; }

        /// <summary>
        /// Puntaje otorgado por este movimiento.
        /// </summary>
        public int ScoreDelta { get; set; }

        public System.DateTime ExecutedAt { get; set; } = System.DateTime.UtcNow;
    }
}
