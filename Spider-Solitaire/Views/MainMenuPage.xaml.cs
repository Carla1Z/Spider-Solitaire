// Code-behind mínimo: solo inicializa el ViewModel.
// Toda la lógica vive en MainMenuViewModel.
using SpiderSolitaire.ViewModels;

namespace SpiderSolitaire.Views
{
    public partial class MainMenuPage : ContentPage
    {
        private readonly MainMenuViewModel _viewModel;

        public MainMenuPage(MainMenuViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            BindingContext = _viewModel;
        }

        // Inicializar cuando la página aparece en pantalla
        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await _viewModel.InitializeAsync();
        }
    }
}