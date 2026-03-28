// Paleta de colores "Cottage Core" centralizada.
// Usar constantes en lugar de hard-code en XAML para cambios globales fáciles.
namespace SpiderSolitaire.Constants
{
    /// <summary>
    /// Paleta de colores cottage core: tonos suaves de naturaleza.
    /// Referenciados desde XAML vía StaticResource y desde código C#.
    ///
    /// Decisión: usar strings hex aquí y definir los Color resources
    /// en App.xaml para que XAML pueda bindear correctamente.
    /// </summary>
    public static class AppColors
    {
        // ── Fondos ─────────────────────────────────────────────────
        public const string BackgroundPrimary = "#2D4A2D"; // Verde bosque oscuro
        public const string BackgroundSecondary = "#3A5C3A"; // Verde medio
        public const string TableFelt = "#4A7C59"; // Verde tapete

        // ── Cartas ─────────────────────────────────────────────────
        public const string CardFace = "#FAF3E8"; // Beige crema cálido
        public const string CardBack = "#7B9E6B"; // Verde salvia
        public const string CardBorder = "#C4A882"; // Marrón cálido
        public const string CardShadow = "#1A2E1A"; // Sombra verde oscura

        // ── Texto ──────────────────────────────────────────────────
        public const string TextPrimary = "#3B2A1A"; // Marrón oscuro cálido
        public const string TextSecondary = "#6B5040"; // Marrón medio
        public const string TextOnDark = "#F0E8D8"; // Beige claro sobre fondos oscuros
        public const string TextRed = "#C0392B"; // Rojo hearts/diamonds (suavizado)
        public const string TextBlack = "#2C1810"; // Casi negro cálido

        // ── UI Elements ────────────────────────────────────────────
        public const string ButtonPrimary = "#8B7355"; // Marrón cálido
        public const string ButtonSecondary = "#6B8F5E"; // Verde oliva
        public const string ButtonAccent = "#C4956A"; // Dorado suave
        public const string ButtonDanger = "#B87070"; // Rojo suave

        // ── Highlights ─────────────────────────────────────────────
        public const string HighlightValid = "#90C88A"; // Verde menta (drop válido)
        public const string HighlightInvalid = "#C88A8A"; // Rosa rojizo (drop inválido)
        public const string HighlightSelect = "#E8C97A"; // Dorado (selección)

        // ── Decorativos ────────────────────────────────────────────
        public const string AccentGold = "#D4A853"; // Dorado cottage
        public const string AccentRose = "#D4956A"; // Rosa teja
        public const string AccentMoss = "#7A9E6A"; // Verde musgo
        public const string AccentBark = "#8B6914"; // Corteza de árbol

        // ── Overlay / Modal ────────────────────────────────────────
        public const string OverlayDark = "#CC1A2E1A"; // Verde oscuro semitransparente
        public const string OverlayLight = "#99FAF3E8"; // Beige semitransparente
    }
}