// Punto de entrada de la aplicación MAUI.
// Mantiene mínimo: solo configura el Shell inicial.
namespace SpiderSolitaire
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            // Shell como página principal — gestiona toda la navegación
            MainPage = new AppShell();
        }

        // Manejo del ciclo de vida de la app
        protected override void OnStart()
        {
            base.OnStart();
            System.Diagnostics.Debug.WriteLine("[App] Started");
        }

        protected override void OnSleep()
        {
            base.OnSleep();
            // La app pasa a segundo plano — el GameService guardó automáticamente
            System.Diagnostics.Debug.WriteLine("[App] Sleep");
        }

        protected override void OnResume()
        {
            base.OnResume();
            System.Diagnostics.Debug.WriteLine("[App] Resumed");
        }
    }
}