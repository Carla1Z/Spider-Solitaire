// Mapper central: convierte objetos de dominio (Models) a DTOs para la UI.
// Separa la capa de presentación del dominio — la View nunca toca Models.
using System.Collections.Generic;
using System.Linq;
using SpiderSolitaire.DTOs;
using SpiderSolitaire.Models;

namespace SpiderSolitaire.Mappers
{
    /// <summary>
    /// Convierte entidades de dominio a DTOs listos para bindear en la UI.
    /// Registrado como Singleton en DI porque no tiene estado.
    ///
    /// Decisión de diseño: usamos mapper manual en lugar de AutoMapper
    /// para evitar dependencias externas y tener control total del mapeo.
    /// </summary>
    public class GameMapper
    {
        // ── GameState → GameStateDto ───────────────────────────────
        public GameStateDto ToDto(GameState state)
        {
            return new GameStateDto
            {
                Columns = state.Columns.Select(MapColumn).ToList(),
                StockPilesRemaining = state.StockPiles.Count,
                CompletedSequences = state.CompletedSequences.Count,
                Score = state.Score,
                Moves = state.Moves,
                ElapsedTime = FormatElapsed(state.ElapsedTime),
                Difficulty = MapDifficultyLabel(state.Difficulty),
                CanDeal = state.CanDeal,
                CanUndo = state.CanUndo,
                IsGameOver = state.IsGameOver,
                HasWon = state.Result == GameResult.Won,
                Result = state.Result
            };
        }

        // ── Column → ColumnDto ─────────────────────────────────────
        public ColumnDto MapColumn(Column column)
        {
            // Determinar desde qué índice la secuencia es válida (arrastrable)
            int validSeqStart = column.GetValidSequenceStart();

            var cardDtos = column.Cards
                .Select((card, index) => MapCard(card, index, validSeqStart))
                .ToList();

            return new ColumnDto
            {
                Index = column.Index,
                Cards = cardDtos,
                IsEmpty = column.IsEmpty,
                FaceDownCount = column.Cards.Count(c => !c.IsFaceUp)
            };
        }

        // ── Card → CardDto ─────────────────────────────────────────
        public CardDto MapCard(Card card, int indexInColumn, int validSeqStart)
        {
            return new CardDto
            {
                Key = card.Key,
                RankLabel = card.RankLabel,
                SuitSymbol = card.SuitSymbol,
                SuitName = card.Suit.ToString(),
                RankValue = card.Value,
                IsFaceUp = card.IsFaceUp,
                IsDragging = card.IsDragging,
                ColumnIndex = card.ColumnIndex,
                PositionInColumn = card.PositionInColumn,
                // Una carta es arrastrable si está en la secuencia válida
                // y está boca arriba
                IsDraggable = card.IsFaceUp && indexInColumn >= validSeqStart
            };
        }

        // ── Helpers ────────────────────────────────────────────────
        private static string MapDifficultyLabel(Difficulty difficulty) => difficulty switch
        {
            Difficulty.OneSuit => "Un Palo 🌿",
            Difficulty.TwoSuits => "Dos Palos 🌸",
            Difficulty.FourSuits => "Cuatro Palos 🍂",
            _ => "Un Palo 🌿"
        };

        private static string FormatElapsed(System.TimeSpan elapsed)
            => elapsed.ToString(@"mm\:ss");
    }
}