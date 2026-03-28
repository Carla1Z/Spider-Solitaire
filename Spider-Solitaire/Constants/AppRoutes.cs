// Rutas de navegación de Shell centralizadas.
namespace SpiderSolitaire.Constants
{
    /// <summary>
    /// Rutas de navegación Shell.
    /// Centralizar aquí evita strings duplicados en ViewModels y Shell.
    /// </summary>
    public static class AppRoutes
    {
        public const string MainMenu = "//MainMenu";
        public const string Game = "Game";
        public const string Settings = "Settings";
        public const string About = "About";
    }

    /// <summary>
    /// Nombres de parámetros de query para Shell navigation.
    /// </summary>
    public static class RouteParameters
    {
        public const string Difficulty = "difficulty";
        public const string Seed = "seed";
    }
}