// Alias explícito para SQLite-net — resuelve ambigüedades con
// System.Diagnostics.CodeAnalysis y System.ComponentModel.DataAnnotations.Schema
// que .NET 10 incluye automáticamente via ImplicitUsings.
using Sqlite = SQLite;

namespace SpiderSolitaire.Data.Database
{
    /// <summary>
    /// Tabla SQLite que almacena partidas guardadas.
    /// Usamos serialización JSON para el estado complejo del juego —
    /// más flexible que normalizar todo en tablas relacionales.
    ///
    /// NOTA: todos los atributos usan el alias [Sqlite.X] para evitar
    /// ambigüedades con atributos homónimos del framework base.
    /// </summary>
    [Sqlite.Table("saved_games")]
    public class SavedGameEntity
    {
        [Sqlite.PrimaryKey]
        [Sqlite.AutoIncrement]
        [Sqlite.Column("id")]
        public int Id { get; set; }

        /// <summary>
        /// Identificador único de la partida (GUID).
        /// </summary>
        [Sqlite.Column("game_id")]
        [Sqlite.Unique]
        [Sqlite.NotNull]
        public string GameId { get; set; } = System.Guid.NewGuid().ToString();

        /// <summary>
        /// Estado completo del juego serializado como JSON.
        /// </summary>
        [Sqlite.Column("state_json")]
        [Sqlite.NotNull]
        public string StateJson { get; set; } = string.Empty;

        /// <summary>
        /// Dificultad guardada por separado para queries rápidas
        /// sin deserializar el JSON completo.
        /// </summary>
        [Sqlite.Column("difficulty")]
        public int Difficulty { get; set; }

        /// <summary>
        /// Puntaje guardado por separado para mostrarlo en la UI
        /// sin deserializar el JSON completo.
        /// </summary>
        [Sqlite.Column("score")]
        public int Score { get; set; }

        [Sqlite.Column("created_at")]
        public System.DateTime CreatedAt { get; set; } = System.DateTime.UtcNow;

        [Sqlite.Column("updated_at")]
        public System.DateTime UpdatedAt { get; set; } = System.DateTime.UtcNow;
    }
}