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

        protected override void OnAppearing()
        {
            base.OnAppearing();

            // ✅ Disparar sin await para no bloquear el hilo de UI
            // MainThread.BeginInvokeOnMainThread garantiza que la
            // actualización de propiedades ocurra en el hilo correcto
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                await _viewModel.InitializeAsync();
            });
        }
    }
}