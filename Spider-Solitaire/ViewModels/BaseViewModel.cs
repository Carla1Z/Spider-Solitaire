// ViewModel base con INotifyPropertyChanged y comandos comunes.
// Todos los ViewModels heredan de aquí para evitar código repetido.
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SpiderSolitaire.ViewModels
{
    /// <summary>
    /// Base de todos los ViewModels.
    /// Provee: INotifyPropertyChanged, IsBusy, Title, y helpers de SetProperty.
    /// Usamos CommunityToolkit.Mvvm patterns sin la dependencia del paquete
    /// para mayor control (se puede migrar fácilmente si se desea).
    /// </summary>
    public abstract class BaseViewModel : INotifyPropertyChanged
    {
        // ── INotifyPropertyChanged ─────────────────────────────────
        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        /// <summary>
        /// Asigna valor a un campo y notifica si cambió.
        /// Retorna true si el valor cambió efectivamente.
        /// </summary>
        protected bool SetProperty<T>(
            ref T backingField,
            T value,
            [CallerMemberName] string? propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(backingField, value))
                return false;

            backingField = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        // ── Estado común ───────────────────────────────────────────
        private bool _isBusy;
        private string _title = string.Empty;

        public bool IsBusy
        {
            get => _isBusy;
            set => SetProperty(ref _isBusy, value);
        }

        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        // ── Helper para comandos async seguros ─────────────────────
        /// <summary>
        /// Ejecuta una tarea async mostrando IsBusy durante la ejecución.
        /// Captura excepciones para evitar crashes silenciosos.
        /// </summary>
        protected async Task ExecuteAsync(
            System.Func<Task> task,
            System.Action<System.Exception>? onError = null)
        {
            if (IsBusy) return;

            IsBusy = true;
            try
            {
                await task();
            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[ViewModel] Error: {ex.Message}");
                onError?.Invoke(ex);
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}