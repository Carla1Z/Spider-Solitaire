using System.Threading.Tasks;
using SpiderSolitaire.DTOs;
using SpiderSolitaire.Interfaces;
using SpiderSolitaire.Constants;

namespace SpiderSolitaire.Services
{
    /// <summary>
    /// Navega usando Shell.Current.GoToAsync con query string en la URL.
    /// IMPORTANTE: pasar parámetros como query string "?key=value"
    /// es el método más confiable en MAUI — evita el cast error
    /// que ocurre al usar Dictionary con objetos tipados.
    /// </summary>
    public class NavigationService : INavigationService
    {
        public Task NavigateToAsync(string route)
            => Shell.Current.GoToAsync(route);

        public Task NavigateBackAsync()
            => Shell.Current.GoToAsync("..");

        public Task NavigateToGameAsync(NewGameRequestDto request)
        {
            // ✅ Pasar como query string directamente en la URL
            // Shell parsea estos valores como string y [QueryProperty]
            // los recibe sin problemas de casting
            int difficultyValue = (int)request.Difficulty;
            string seedValue = request.Seed?.ToString() ?? string.Empty;

            string route = $"{AppRoutes.Game}?difficulty={difficultyValue}&seed={seedValue}";

            System.Diagnostics.Debug.WriteLine(
                $"[NavigationService] Navigating to: {route}");

            return Shell.Current.GoToAsync(route);
        }
    }
}