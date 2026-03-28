// Code-behind del Shell: registra rutas adicionales (páginas sin tab).
using SpiderSolitaire.Constants;
using SpiderSolitaire.Views;

namespace SpiderSolitaire
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            // Registrar rutas que no están en el tab bar
            // (se navega programáticamente con GoToAsync)
            Routing.RegisterRoute(AppRoutes.Game, typeof(GamePage));
            Routing.RegisterRoute(AppRoutes.Settings, typeof(GamePage)); // Placeholder
        }
    }
}