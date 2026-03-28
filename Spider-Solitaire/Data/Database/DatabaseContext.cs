// Contexto de base de datos SQLite-net.
// Gestiona la conexión y asegura que las tablas existan al iniciar.
using System.Threading.Tasks;
using SQLite;

namespace SpiderSolitaire.Data.Database
{
    /// <summary>
    /// Wrapper sobre SQLiteAsyncConnection.
    /// Patrón Singleton gestionado por DI (registrado como Singleton en MauiProgram).
    /// Inicializa las tablas lazy (solo cuando se necesitan por primera vez).
    /// </summary>
    public class DatabaseContext
    {
        // ── Conexión ───────────────────────────────────────────────
        private SQLiteAsyncConnection? _connection;
        private bool _initialized = false;

        // ── Inicialización lazy ────────────────────────────────────
        /// <summary>
        /// Retorna la conexión lista para usar.
        /// Crea las tablas en la primera llamada.
        /// </summary>
        public async Task<SQLiteAsyncConnection> GetConnectionAsync()
        {
            if (!_initialized)
                await InitializeAsync();

            return _connection!;
        }

        private async Task InitializeAsync()
        {
            if (_initialized) return;

            _connection = new SQLiteAsyncConnection(
                DatabaseConfig.DatabasePath,
                DatabaseConfig.Flags);

            // Crear tablas si no existen
            await _connection.CreateTableAsync<SavedGameEntity>();

            _initialized = true;
        }
    }
}