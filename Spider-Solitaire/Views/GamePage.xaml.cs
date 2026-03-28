// Code-behind de GamePage — mínimo posible.
// Solo conecta el ViewModel y maneja el ciclo de vida de la página.
using SpiderSolitaire.ViewModels;

namespace SpiderSolitaire.Views
{
    public partial class GamePage : ContentPage
    {
        private readonly GameViewModel _viewModel;

        public GamePage(GameViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            BindingContext = _viewModel;
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            // Pausar el timer cuando la página no está visible
            // El timer se reanuda cuando el ViewModel recibe los parámetros de navegación
        }
    }
}