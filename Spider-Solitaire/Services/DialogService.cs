// Implementación de IDialogService usando la API de alertas de MAUI.
using System.Threading.Tasks;
using SpiderSolitaire.Interfaces;

namespace SpiderSolitaire.Services
{
    /// <summary>
    /// Muestra alertas nativas de MAUI a través de Application.Current.MainPage.
    /// </summary>
    public class DialogService : IDialogService
    {
        private static Microsoft.Maui.Controls.Page? CurrentPage
            => Application.Current?.MainPage;

        public Task ShowAlertAsync(string title, string message, string cancel = "OK")
            => CurrentPage?.DisplayAlert(title, message, cancel)
               ?? Task.CompletedTask;

        public async Task<bool> ShowConfirmAsync(
            string title,
            string message,
            string accept = "Yes",
            string cancel = "No")
        {
            if (CurrentPage == null) return false;
            return await CurrentPage.DisplayAlert(title, message, accept, cancel);
        }

        public async Task<string?> ShowActionSheetAsync(
            string title,
            string cancel,
            params string[] buttons)
        {
            if (CurrentPage == null) return null;
            return await CurrentPage.DisplayActionSheet(title, cancel, null, buttons);
        }
    }
}