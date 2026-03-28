// Abstracción de navegación para no acoplar ViewModels a Shell/Navigation.
using System.Threading.Tasks;

namespace SpiderSolitaire.Interfaces
{
    /// <summary>
    /// Servicio de navegación desacoplado de la implementación de MAUI Shell.
    /// Los ViewModels usan esta interfaz para navegar sin importar Shell.
    /// </summary>
    public interface INavigationService
    {
        Task NavigateToAsync(string route);
        Task NavigateBackAsync();
        Task NavigateToGameAsync(DTOs.NewGameRequestDto request);
    }
}