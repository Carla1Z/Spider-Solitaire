// Abstracción para mostrar diálogos y alertas desde los ViewModels.
// Testeable y desacoplado de DisplayAlert de MAUI.
using System.Threading.Tasks;

namespace SpiderSolitaire.Interfaces
{
    /// <summary>
    /// Servicio para mostrar diálogos modales desde los ViewModels
    /// sin depender directamente de Application.Current o Page.
    /// </summary>
    public interface IDialogService
    {
        Task ShowAlertAsync(string title, string message, string cancel = "OK");

        Task<bool> ShowConfirmAsync(
            string title,
            string message,
            string accept = "Yes",
            string cancel = "No");

        Task<string?> ShowActionSheetAsync(
            string title,
            string cancel,
            params string[] buttons);
    }
}