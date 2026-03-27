using System;
using System.Collections.Generic;
using System.Text;

// Encapsula el estado completo de una partida en un momento dado.
// Usado para guardar/restaurar partidas y para el sistema de Undo.
using System.Collections.Generic;

namespace SpiderSolitaire.Models
{
    /// <summary>
    /// Dificultad del juego según la cantidad de palos utilizados.
    /// </summary>
    public enum Difficulty
    {
        OneSuit = 1,   // Solo Spades — más fácil
        TwoSuits = 2,   // Spades + Hearts
        FourSuits = 4    // Todos los palos — más difícil
    }

    /// <summary>
    /// Resultado de la partida actual.
    /// </summary>
    public enum GameResult
    {
        InProgress,
        Won,
        Lost
    }

    /// <summary>
    /// Snapshot completo del estado del juego.
    /// Permite serializar/deserializar la partida.
    /// </summary>
    public class GameState
    {
        // ── Configuración ──────────────────────────────────────────
        public Difficulty Difficulty { get; set; } = Difficulty.OneSuit;

        // ── Tablero ────────────────────────────────────────────────
        /// <summary>
        /// Las 10 columnas del tablero.
        /// </summary>
        public List<Column> Columns { get; set; } = new();

        /// <summary>
        /// Stock: grupos de cartas que se reparten con el botón "Deal".
        /// Cada Deal reparte 1 carta a cada columna (10 cartas por Deal).
        /// Spider usa 5 grupos = 5 Deals posibles.
        /// </summary>
        public List<List<Card>> StockPiles { get; set; } = new();

        /// <summary>
        /// Secuencias completas ya removidas del tablero (0–8).
        /// </summary>
        public List<List<Card>> CompletedSequences { get; set; } = new();

        // ── Estadísticas ───────────────────────────────────────────
        public int Score { get; set; } = 500;
        public int Moves { get; set; } = 0;
        public int DealsRemaining { get; set; } = 5;
        public GameResult Result { get; set; } = GameResult.InProgress;
        public System.DateTime StartedAt { get; set; } = System.DateTime.UtcNow;
        public System.TimeSpan ElapsedTime { get; set; } = System.TimeSpan.Zero;

        // ── Historial para Undo ────────────────────────────────────
        /// <summary>
        /// Pila de snapshots para soportar Undo.
        /// Cada elemento es un estado anterior serializado.
        /// </summary>
        public Stack<GameSnapshot> UndoHistory { get; set; } = new();

        // ── Helpers ────────────────────────────────────────────────
        public bool IsGameOver => Result != GameResult.InProgress;
        public bool CanDeal => DealsRemaining > 0 && !IsGameOver;
        public bool CanUndo => UndoHistory.Count > 0 && !IsGameOver;
        public int CompletedCount => CompletedSequences.Count;
    }

    /// <summary>
    /// Snapshot ligero del tablero para el sistema de Undo.
    /// Solo guarda lo mínimo necesario para restaurar el estado anterior.
    /// </summary>
    public class GameSnapshot
    {
        public List<List<(string Key, bool IsFaceUp)>> ColumnCards { get; set; } = new();
        public int Score { get; set; }
        public int Moves { get; set; }
        public int DealsLeft { get; set; }
        public int Completed { get; set; }
    }
}
