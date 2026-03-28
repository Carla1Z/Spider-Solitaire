using System;
using System.Collections.Generic;
using System.Text;

// DTO del estado global del juego.
// Es lo que el ViewModel expone a la View.
using System.Collections.Generic;
using SpiderSolitaire.Models;

namespace SpiderSolitaire.DTOs
{
    /// <summary>
    /// Snapshot del juego listo para bindear en la UI.
    /// </summary>
    public class GameStateDto
    {
        // ── Tablero ────────────────────────────────────────────────
        public List<ColumnDto> Columns { get; set; } = new();
        public int StockPilesRemaining { get; set; }
        public int CompletedSequences { get; set; }

        // ── Estadísticas visibles ──────────────────────────────────
        public int Score { get; set; }
        public int Moves { get; set; }
        public string ElapsedTime { get; set; } = "00:00";
        public string Difficulty { get; set; } = "One Suit";

        // ── Estado de controles ────────────────────────────────────
        public bool CanDeal { get; set; }
        public bool CanUndo { get; set; }
        public bool IsGameOver { get; set; }
        public bool HasWon { get; set; }

        // ── Resultado ─────────────────────────────────────────────
        public GameResult Result { get; set; } = GameResult.InProgress;
    }
}
