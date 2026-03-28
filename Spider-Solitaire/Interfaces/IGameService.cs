// Contrato principal del servicio de juego.
// El ViewModel depende solo de esta abstracción — nunca de la implementación.
using System.Threading.Tasks;
using SpiderSolitaire.DTOs;
using SpiderSolitaire.Models;

namespace SpiderSolitaire.Interfaces
{
    /// <summary>
    /// Define todas las operaciones posibles sobre una partida de Spider Solitaire.
    /// Permite mockear el servicio en tests unitarios sin tocar la lógica.
    /// </summary>
    public interface IGameService
    {
        // ── Ciclo de vida ──────────────────────────────────────────
        /// <summary>
        /// Inicia una partida nueva con la configuración dada.
        /// </summary>
        Task<GameStateDto> StartNewGameAsync(NewGameRequestDto request);

        /// <summary>
        /// Reinicia la partida actual manteniendo la dificultad.
        /// </summary>
        Task<GameStateDto> RestartGameAsync();

        /// <summary>
        /// Devuelve el estado actual del juego mapeado a DTO.
        /// </summary>
        GameStateDto GetCurrentState();

        // ── Movimientos ────────────────────────────────────────────
        /// <summary>
        /// Intenta mover cartas desde una columna origen a una destino.
        /// Retorna el nuevo estado o null si el movimiento es inválido.
        /// </summary>
        Task<GameStateDto?> TryMoveCardsAsync(
            int sourceColumnIndex,
            int cardIndexInColumn,
            int targetColumnIndex);

        /// <summary>
        /// Reparte una nueva ronda de cartas del stock (1 por columna).
        /// </summary>
        Task<GameStateDto?> DealNextRoundAsync();

        /// <summary>
        /// Deshace el último movimiento.
        /// </summary>
        Task<GameStateDto?> UndoLastMoveAsync();

        // ── Validación ─────────────────────────────────────────────
        /// <summary>
        /// Verifica si un movimiento es válido sin ejecutarlo.
        /// Útil para resaltar columnas válidas durante drag & drop.
        /// </summary>
        bool IsValidMove(int sourceColumnIndex, int cardIndexInColumn, int targetColumnIndex);

        /// <summary>
        /// Verifica si hay movimientos disponibles en el tablero actual.
        /// Si no hay y no quedan stocks, el juego termina.
        /// </summary>
        bool HasAvailableMoves();
    }
}