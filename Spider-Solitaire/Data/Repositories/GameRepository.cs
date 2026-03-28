// Implementación del repositorio de partidas sobre SQLite.
// Serializa/deserializa GameState usando System.Text.Json.
using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using SpiderSolitaire.Data.Database;
using SpiderSolitaire.Interfaces;
using SpiderSolitaire.Models;

namespace SpiderSolitaire.Data.Repositories
{
    /// <summary>
    /// Persiste y recupera partidas de la base de datos SQLite local.
    /// Implementa IGameRepository para desacoplar la capa de datos
    /// del resto de la aplicación.
    /// </summary>
    public class GameRepository : IGameRepository
    {
        // ── Dependencias ───────────────────────────────────────────
        private readonly DatabaseContext _context;

        // ── Opciones de serialización JSON ─────────────────────────
        private static readonly JsonSerializerOptions _jsonOptions = new()
        {
            // Incluir campos privados si fuera necesario
            IncludeFields = false,
            WriteIndented = false, // Compacto para DB
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            // Manejar referencias circulares por si acaso
            ReferenceHandler = ReferenceHandler.IgnoreCycles,
            Converters =
            {
                // Converter para Stack<GameSnapshot> que no serializa bien por defecto
                new JsonStringEnumConverter()
            }
        };

        // ── Constructor ────────────────────────────────────────────
        public GameRepository(DatabaseContext context)
        {
            _context = context;
        }

        // ── IGameRepository ────────────────────────────────────────
        public async Task SaveGameAsync(GameState state)
        {
            var db = await _context.GetConnectionAsync();

            var json = SerializeState(state);
            var entity = new SavedGameEntity
            {
                GameId = "current",   // Solo guardamos 1 partida activa
                StateJson = json,
                Difficulty = (int)state.Difficulty,
                Score = state.Score,
                UpdatedAt = DateTime.UtcNow
            };

            // InsertOrReplace: actualiza si ya existe, inserta si no
            await db.InsertOrReplaceAsync(entity);
        }

        public async Task<GameState?> LoadLastGameAsync()
        {
            var db = await _context.GetConnectionAsync();
            var entity = await db.Table<SavedGameEntity>()
                                 .Where(e => e.GameId == "current")
                                 .FirstOrDefaultAsync();

            if (entity == null) return null;

            return DeserializeState(entity.StateJson);
        }

        public async Task DeleteSavedGameAsync()
        {
            var db = await _context.GetConnectionAsync();
            await db.Table<SavedGameEntity>()
                    .DeleteAsync(e => e.GameId == "current");
        }

        public async Task<bool> HasSavedGameAsync()
        {
            var db = await _context.GetConnectionAsync();
            var count = await db.Table<SavedGameEntity>()
                                .CountAsync(e => e.GameId == "current");
            return count > 0;
        }

        // ── Privados: Serialización ────────────────────────────────
        private static string SerializeState(GameState state)
        {
            try
            {
                return JsonSerializer.Serialize(state, _jsonOptions);
            }
            catch (Exception ex)
            {
                // Log del error — en producción usaríamos ILogger
                System.Diagnostics.Debug.WriteLine($"[GameRepository] Serialize error: {ex.Message}");
                return "{}";
            }
        }

        private static GameState? DeserializeState(string json)
        {
            if (string.IsNullOrWhiteSpace(json)) return null;

            try
            {
                return JsonSerializer.Deserialize<GameState>(json, _jsonOptions);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[GameRepository] Deserialize error: {ex.Message}");
                return null;
            }
        }
    }
}