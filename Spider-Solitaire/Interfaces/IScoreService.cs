// Contrato para el cálculo y gestión del puntaje.
using SpiderSolitaire.Models;

namespace SpiderSolitaire.Interfaces
{
    /// <summary>
    /// Encapsula las reglas de puntaje del juego.
    /// Separado para poder ajustar la lógica de scoring sin tocar GameService.
    /// </summary>
    public interface IScoreService
    {
        /// <summary>
        /// Puntaje inicial de una partida (Spider estándar: 500).
        /// </summary>
        int InitialScore { get; }

        /// <summary>
        /// Puntaje por mover carta(s) correctamente.
        /// </summary>
        int GetMoveScore(Move move);

        /// <summary>
        /// Puntaje por completar una secuencia (K→A mismo palo).
        /// </summary>
        int GetSequenceCompletionScore();

        /// <summary>
        /// Penalización por usar Undo.
        /// </summary>
        int GetUndoPenalty();

        /// <summary>
        /// Penalización por usar Deal del stock.
        /// </summary>
        int GetDealPenalty();
    }
}