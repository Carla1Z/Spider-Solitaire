// Nombres de fuentes tipográficas registradas en la app.
namespace SpiderSolitaire.Constants
{
    /// <summary>
    /// Nombres de las fuentes registradas en MauiProgram.cs.
    /// Usar estas constantes evita typos en XAML.
    ///
    /// Fuentes cottage core recomendadas:
    /// - Playfair Display: elegante, serif, cálida
    /// - Lato: sans-serif limpia para textos pequeños
    /// - Dancing Script: cursiva decorativa para títulos
    /// </summary>
    public static class AppFonts
    {
        public const string Title = "PlayfairDisplay";       // Títulos principales
        public const string TitleBold = "PlayfairDisplay-Bold";  // Títulos en negrita
        public const string Body = "Lato";                  // Texto general
        public const string BodyBold = "Lato-Bold";             // Texto en negrita
        public const string Decorative = "DancingScript";         // Títulos decorativos
        public const string CardRank = "PlayfairDisplay-Bold";  // Valor en cartas
        public const string Fallback = "OpenSansRegular";       // Fallback incluido en MAUI
    }
}