// Extensión de IServiceCollection que registra todos los servicios.
// Mantiene MauiProgram.cs limpio — toda la DI está aquí centralizada.
using Microsoft.Extensions.DependencyInjection;
using SpiderSolitaire.Data.Database;
using SpiderSolitaire.Data.Repositories;
using SpiderSolitaire.Interfaces;
using SpiderSolitaire.Mappers;
using SpiderSolitaire.Services;
using SpiderSolitaire.ViewModels;
using SpiderSolitaire.Views;

namespace SpiderSolitaire.Config
{
    /// <summary>
    /// Registra todos los servicios, repositorios, ViewModels y Views
    /// en el contenedor de DI de .NET MAUI.
    ///
    /// Decisiones de lifetime:
    /// - Singleton:  servicios stateless o que mantienen estado global (DB, Navigation)
    /// - Transient:  ViewModels (nueva instancia por navegación)
    /// - Scoped:     no aplica en MAUI (sin request scope)
    /// </summary>
    public static class DependencyInjectionConfig
    {
        public static IServiceCollection AddAppServices(
            this IServiceCollection services)
        {
            RegisterInfrastructure(services);
            RegisterServices(services);
            RegisterViewModels(services);
            RegisterViews(services);
            RegisterConfig(services);
            return services;
        }

        // ── Infraestructura ────────────────────────────────────────
        private static void RegisterInfrastructure(IServiceCollection services)
        {
            // Base de datos — Singleton porque SQLite no es thread-safe
            // si se usan múltiples conexiones simultáneas
            services.AddSingleton<DatabaseContext>();
            services.AddSingleton<IGameRepository, GameRepository>();
        }

        // ── Servicios de dominio ───────────────────────────────────
        private static void RegisterServices(IServiceCollection services)
        {
            // Singleton: stateless, sin dependencias volátiles
            services.AddSingleton<IDeckService, DeckService>();
            services.AddSingleton<IScoreService, ScoreService>();
            services.AddSingleton<GameValidationService>();
            services.AddSingleton<GameMapper>();

            // GameService: Singleton para mantener el estado de la partida
            // entre navegaciones (volver al menú y continuar)
            services.AddSingleton<IGameService, GameService>();

            // UI Services — Singleton
            services.AddSingleton<INavigationService, NavigationService>();
            services.AddSingleton<IDialogService, DialogService>();

            // Timer — Transient porque cada partida tiene su propio timer
            services.AddTransient<ITimerService>(sp =>
                new TimerService(
                    Microsoft.Maui.Controls.Application.Current!.Dispatcher));
        }

        // ── ViewModels ─────────────────────────────────────────────
        private static void RegisterViewModels(IServiceCollection services)
        {
            // Transient: cada vez que se navega se crea una instancia nueva
            services.AddTransient<MainMenuViewModel>();
            services.AddTransient<GameViewModel>();
        }

        // ── Views (Pages) ──────────────────────────────────────────
        private static void RegisterViews(IServiceCollection services)
        {
            // Las páginas también son Transient en MAUI Shell
            services.AddTransient<MainMenuPage>();
            services.AddTransient<GamePage>();
        }

        // ── Configuración ──────────────────────────────────────────
        private static void RegisterConfig(IServiceCollection services)
        {
            services.AddSingleton<AppSettings>();
        }
    }
}