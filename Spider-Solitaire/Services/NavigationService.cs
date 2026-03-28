// Implementación de INavigationService usando Shell de MAUI.
using System.Threading.Tasks;
using SpiderSolitaire.DTOs;
using SpiderSolitaire.Interfaces;
using SpiderSolitaire.Constants;

namespace SpiderSolitaire.Services
{
    /// <summary>
    /// Navega usando Shell.Current.GoToAsync.
    /// Registrado como Singleton para mantener el estado de navegación.
    /// </summary>
    public class NavigationService : INavigationService
    {
        public Task NavigateToAsync(string route)
            => Shell.Current.GoToAsync(route);

        public Task NavigateBackAsync()
            => Shell.Current.GoToAsync("..");

        public Task NavigateToGameAsync(NewGameRequestDto request)
        {
            var parameters = new Dictionary<string, object>
            {
                { RouteParameters.Difficulty, (int)request.Difficulty },
                { RouteParameters.Seed,       request.Seed?.ToString() ?? "" }
            };
            return Shell.Current.GoToAsync(AppRoutes.Game, parameters);
        }
    }
}