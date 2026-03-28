// Implementa las reglas de puntaje estándar de Spider Solitaire.
using SpiderSolitaire.Interfaces;
using SpiderSolitaire.Models;

namespace SpiderSolitaire.Services
{
    /// <summary>
    /// Reglas de puntaje de Spider Solitaire:
    /// - Inicio: 500 puntos
    /// - Movimiento válido: -1 punto
    /// - Secuencia completa: +100 puntos
    /// - Undo: -5 puntos
    /// - Deal: -1 punto por carta (10 cartas = -10)
    /// </summary>
    public class ScoreService : IScoreService
    {
        public int InitialScore => 500;

        public int GetMoveScore(Move move) => move.Type switch
        {
            MoveType.CardMove => -1,
            MoveType.Deal => GetDealPenalty(),
            MoveType.SequenceRemove => GetSequenceCompletionScore(),
            MoveType.Undo => GetUndoPenalty(),
            _ => 0
        };

        // Cada Deal reparte 10 cartas, penaliza 1 por carta
        public int GetDealPenalty() => -10;

        // Completar una secuencia K→A del mismo palo vale 100 puntos
        public int GetSequenceCompletionScore() => 100;

        // Undo penaliza para desincentivar el uso excesivo
        public int GetUndoPenalty() => -5;
    }
}