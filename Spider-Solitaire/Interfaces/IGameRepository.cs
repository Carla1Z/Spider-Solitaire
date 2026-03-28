// Contrato para persistencia del estado del juego.
using System.Threading.Tasks;
using SpiderSolitaire.Models;

namespace SpiderSolitaire.Interfaces
{
    /// <summary>
    /// Define las operaciones de almacenamiento de partidas.
    /// Implementado sobre SQLite en Data/Repositories.
    /// </summary>
    public interface IGameRepository
    {
        Task SaveGameAsync(GameState state);
        Task<GameState?> LoadLastGameAsync();
        Task DeleteSavedGameAsync();
        Task<bool> HasSavedGameAsync();
    }
}