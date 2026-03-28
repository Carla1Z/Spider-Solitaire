// Control personalizado para renderizar una carta.
// Es un ContentView con su propio BindableProperties para máxima reutilización.
using Microsoft.Maui.Controls;
using SpiderSolitaire.Constants;
using SpiderSolitaire.DTOs;

namespace SpiderSolitaire.Controls
{
    /// <summary>
    /// Control visual de una carta del juego.
    /// Expone BindableProperties para bindear CardDto desde XAML.
    ///
    /// Decisión: usar ContentView en lugar de DataTemplate puro
    /// para poder agregar lógica visual específica de la carta
    /// (animaciones, estados de selección, etc.).
    /// </summary>
    public partial class CardView : ContentView
    {
        // ── Bindable Properties ────────────────────────────────────
        public static readonly BindableProperty CardDataProperty =
            BindableProperty.Create(
                nameof(CardData),
                typeof(CardDto),
                typeof(CardView),
                null,
                propertyChanged: OnCardDataChanged);

        public static readonly BindableProperty IsHighlightedProperty =
            BindableProperty.Create(
                nameof(IsHighlighted),
                typeof(bool),
                typeof(CardView),
                false,
                propertyChanged: OnHighlightChanged);

        public CardDto? CardData
        {
            get => (CardDto?)GetValue(CardDataProperty);
            set => SetValue(CardDataProperty, value);
        }

        public bool IsHighlighted
        {
            get => (bool)GetValue(IsHighlightedProperty);
            set => SetValue(IsHighlightedProperty, value);
        }

        // ── Constructor ────────────────────────────────────────────
        public CardView()
        {
            InitializeComponent();
        }

        // ── Property Changed Handlers ──────────────────────────────
        private static void OnCardDataChanged(
            BindableObject bindable,
            object oldValue,
            object newValue)
        {
            if (bindable is CardView view)
                view.UpdateCardVisuals();
        }

        private static void OnHighlightChanged(
            BindableObject bindable,
            object oldValue,
            object newValue)
        {
            if (bindable is CardView view)
                view.UpdateHighlight((bool)newValue);
        }

        // ── Visual Updates ─────────────────────────────────────────
        private void UpdateCardVisuals()
        {
            if (CardData == null) return;

            if (!CardData.IsFaceUp)
            {
                // Carta boca abajo: mostrar dorso
                CardFaceContent.IsVisible = false;
                CardBackContent.IsVisible = true;
                return;
            }

            // Carta boca arriba: mostrar valores
            CardFaceContent.IsVisible = true;
            CardBackContent.IsVisible = false;

            RankTopLabel.Text = CardData.RankLabel;
            SuitTopLabel.Text = CardData.SuitSymbol;
            RankBottomLabel.Text = CardData.RankLabel;
            SuitBottomLabel.Text = CardData.SuitSymbol;
            CenterSuitLabel.Text = CardData.SuitSymbol;

            // Color según palo (rojo para Hearts/Diamonds)
            var textColor = CardData.IsRed
                ? Color.FromArgb(AppColors.TextRed)
                : Color.FromArgb(AppColors.TextBlack);

            RankTopLabel.TextColor = textColor;
            SuitTopLabel.TextColor = textColor;
            RankBottomLabel.TextColor = textColor;
            SuitBottomLabel.TextColor = textColor;
            CenterSuitLabel.TextColor = textColor;

            // Opacidad reducida si está siendo arrastrada
            Opacity = CardData.IsDragging ? 0.6 : 1.0;
        }

        private void UpdateHighlight(bool highlighted)
        {
            CardBorder.Stroke = highlighted
                ? Color.FromArgb(AppColors.HighlightSelect)
                : Color.FromArgb(AppColors.CardBorder);

            CardBorder.StrokeThickness = highlighted ? 2.5 : 1.0;
        }
    }
}