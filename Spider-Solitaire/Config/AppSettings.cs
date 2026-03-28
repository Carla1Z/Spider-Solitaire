// Configuración global de la aplicación.
// Centraliza flags y valores configurables sin tocar múltiples archivos.
namespace SpiderSolitaire.Config
{
    /// <summary>
    /// Configuración global de la app.
    /// En una app real estos valores vendrían de appsettings.json
    /// o de un servicio de configuración remota.
    /// </summary>
    public class AppSettings
    {
        // ── Juego ──────────────────────────────────────────────────
        public bool SoundEnabled { get; set; } = true;
        public bool AnimationsEnabled { get; set; } = true;
        public bool AutoSave { get; set; } = true;
        public int MaxUndoSteps { get; set; } = 10;

        // ── UI ─────────────────────────────────────────────────────
        public bool ShowTimer { get; set; } = true;
        public bool ShowMoveCount { get; set; } = true;
        public string ThemeName { get; set; } = "CottageCore";

        // ── Debug ──────────────────────────────────────────────────
        public bool EnableDebugLogs { get; set; } = false;
        public int? FixedSeed { get; set; } = null; // null = aleatorio
    }
}