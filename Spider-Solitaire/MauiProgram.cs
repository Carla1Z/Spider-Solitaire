// Punto de configuración principal de .NET MAUI.
// Registra fonts, handlers, servicios y configura la app.
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Controls.Hosting;
using Microsoft.Maui.Hosting;
using SpiderSolitaire.Config;

namespace SpiderSolitaire
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();

            builder
                .UseMauiApp<App>()
                .ConfigureFonts(ConfigureFonts)
                .ConfigureMauiHandlers(ConfigureHandlers);

            // ── Registrar todos los servicios via extensión ────────
            // Toda la lógica de DI está en DependencyInjectionConfig
            // para mantener este archivo limpio
            builder.Services.AddAppServices();

            // ── SQLite-net ─────────────────────────────────────────
            // El DatabaseContext se registra como Singleton en DI,
            // SQLite-net-pcl no requiere configuración adicional aquí

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }

        // ── Tipografías Cottage Core ───────────────────────────────
        private static void ConfigureFonts(IFontCollection fonts)
        {
            // ── Fuentes del sistema incluidas en MAUI ──────────────
            fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
            fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");

            // ── Fuentes personalizadas cottage core ────────────────
            // IMPORTANTE: Agregar estos archivos TTF a:
            // Resources/Fonts/
            // y asegurarse de que tengan Build Action = MauiFont

            // Playfair Display — tipografía serif elegante y cálida
            fonts.AddFont("PlayfairDisplay-Regular.ttf", "PlayfairDisplay");
            fonts.AddFont("PlayfairDisplay-Bold.ttf", "PlayfairDisplay-Bold");
            fonts.AddFont("PlayfairDisplay-Italic.ttf", "PlayfairDisplay-Italic");

            // Lato — sans-serif limpia para texto de interfaz
            fonts.AddFont("Lato-Regular.ttf", "Lato");
            fonts.AddFont("Lato-Bold.ttf", "Lato-Bold");
            fonts.AddFont("Lato-Light.ttf", "Lato-Light");

            // Dancing Script — cursiva decorativa para títulos
            fonts.AddFont("DancingScript-Regular.ttf", "DancingScript");
            fonts.AddFont("DancingScript-Bold.ttf", "DancingScript-Bold");
        }

        // ── Handlers personalizados ────────────────────────────────
        private static void ConfigureHandlers(
            IMauiHandlersCollection handlers)
        {
            // Aquí se pueden registrar handlers nativos personalizados
            // para controles específicos de plataforma.
            // Por ejemplo, para quitar el borde de Entry en Android:
            // handlers.AddHandler<Entry, CustomEntryHandler>();
        }
    }
}