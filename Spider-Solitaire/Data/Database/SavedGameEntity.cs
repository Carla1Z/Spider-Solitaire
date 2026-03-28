// Entidad de base de datos para persistir el estado del juego.
// Serializa GameState a JSON para almacenamiento simple y flexible.
using SQLite;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace SpiderSolitaire.Data.Database
{
    /// <summary>
    /// Tabla SQLite que almacena partidas guardadas.
    /// Usamos serialización JSON para el estado complejo del juego —
    /// más flexible que normalizar todo en tablas relacionales.
    ///
    /// Para un juego de cartas donde el estado cambia frecuentemente,
    /// JSON en SQLite es un buen balance entre simplicidad y rendimiento.
    /// </summary>
    [Table("saved_games")]
    public class SavedGameEntity
    {
        [PrimaryKey, AutoIncrement]
        [Column("id")]
        public int Id { get; set; }

        /// <summary>
        /// Identificador único de la partida (GUID).
        /// Permite distinguir partidas si en el futuro se soportan múltiples slots.
        /// </summary>
        [Column("game_id"), Unique, NotNull]
        public string GameId { get; set; } = System.Guid.NewGuid().ToString();

        /// <summary>
        /// Estado completo del juego serializado como JSON.
        /// Incluye columnas, stock, puntaje, historial de undo, etc.
        /// </summary>
        [Column("state_json"), NotNull]
        public string StateJson { get; set; } = string.Empty;

        /// <summary>
        /// Dificultad como entero (1, 2 o 4 palos).
        /// Guardado por separado para queries rápidas sin deserializar.
        /// </summary>
        [Column("difficulty")]
        public int Difficulty { get; set; }

        /// <summary>
        /// Puntaje actual — guardado por separado para mostrar
        /// en pantalla de selección sin deserializar el JSON completo.
        /// </summary>
        [Column("score")]
        public int Score { get; set; }

        [Column("created_at")]
        public System.DateTime CreatedAt { get; set; } = System.DateTime.UtcNow;

        [Column("updated_at")]
        public System.DateTime UpdatedAt { get; set; } = System.DateTime.UtcNow;
    }
}