using System.ComponentModel;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Shapes;
using SpiderSolitaire.DTOs;
using SpiderSolitaire.ViewModels;

namespace SpiderSolitaire.Views
{
    public partial class GamePage : ContentPage
    {
        private readonly GameViewModel _viewModel;
        private int _selectedColumn = -1;
        private int _selectedCard = -1;

        public GamePage(GameViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            BindingContext = _viewModel;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            _viewModel.PropertyChanged += OnViewModelPropertyChanged;
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            _viewModel.PropertyChanged -= OnViewModelPropertyChanged;
        }

        // ── Reacción a cambios del ViewModel ───────────────────────
        private void OnViewModelPropertyChanged(
            object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(GameViewModel.Columns))
                MainThread.BeginInvokeOnMainThread(RebuildBoard);
        }

        // ── Construcción del tablero ───────────────────────────────
        private void RebuildBoard()
        {
            if (_viewModel.Columns == null ||
                _viewModel.Columns.Count == 0) return;

            BoardGrid.Children.Clear();
            BoardGrid.ColumnDefinitions.Clear();

            int count = _viewModel.Columns.Count;
            for (int i = 0; i < count; i++)
                BoardGrid.ColumnDefinitions.Add(
                    new ColumnDefinition(GridLength.Star));

            for (int i = 0; i < count; i++)
            {
                var view = BuildColumnView(_viewModel.Columns[i]);
                Grid.SetColumn(view, i);
                BoardGrid.Children.Add(view);
            }
        }

        // ── Columna ────────────────────────────────────────────────
        private View BuildColumnView(ColumnDto col)
        {
            var stack = new VerticalStackLayout
            {
                Spacing = 0,
                VerticalOptions = LayoutOptions.Start,
                HorizontalOptions = LayoutOptions.Fill,
                MinimumHeightRequest = 100
            };

            // Resaltado si es columna seleccionada como destino
            var highlight = new BoxView
            {
                Color = Color.FromArgb("#4490C88A"),
                IsVisible = false,
                HeightRequest = 4,
                HorizontalOptions = LayoutOptions.Fill
            };
            stack.Children.Add(highlight);

            if (col.IsEmpty)
            {
                // Placeholder visual + tap para mover carta seleccionada
                var placeholder = BuildEmptyColumnPlaceholder(col.Index);
                stack.Children.Add(placeholder);
            }
            else
            {
                for (int i = 0; i < col.Cards.Count; i++)
                {
                    var cardView = BuildCardView(
                        col.Cards[i], col.Index, i,
                        isLast: i == col.Cards.Count - 1);
                    stack.Children.Add(cardView);
                }
            }

            // Tap en zona vacía inferior de la columna (para mover hacia aquí)
            var columnTap = new TapGestureRecognizer();
            int colIdx = col.Index;
            columnTap.Tapped += (s, e) => OnColumnTapped(colIdx);
            stack.GestureRecognizers.Add(columnTap);

            return stack;
        }

        // ── Placeholder columna vacía ──────────────────────────────
        private View BuildEmptyColumnPlaceholder(int colIndex)
        {
            var border = new Border
            {
                BackgroundColor = Colors.Transparent,
                Stroke = Color.FromArgb("#7A9E6A"),
                StrokeThickness = 1.5,
                StrokeShape = new RoundRectangle { CornerRadius = 8 },
                HeightRequest = 90,
                Opacity = 0.5,
                Content = new Label
                {
                    Text = "🌿",
                    FontSize = 20,
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center
                }
            };

            // Tap en columna vacía = destino del movimiento
            var tap = new TapGestureRecognizer();
            tap.Tapped += (s, e) => OnColumnTapped(colIndex);
            border.GestureRecognizers.Add(tap);

            return border;
        }

        // ── Carta ──────────────────────────────────────────────────
        private View BuildCardView(
            CardDto card, int colIndex, int cardIndex, bool isLast)
        {
            // Altura: cartas intermedias se solapan, la última muestra completa
            double height = isLast ? 88 : (card.IsFaceUp ? 26 : 16);

            var wrapper = new Grid
            {
                HeightRequest = height,
                HorizontalOptions = LayoutOptions.Fill,
                Margin = new Thickness(1, 0)
            };

            // Fondo de la carta
            var cardBorder = new Border
            {
                StrokeShape = new RoundRectangle { CornerRadius = 5 },
                StrokeThickness = 1,
                HeightRequest = 88,
                VerticalOptions = LayoutOptions.Start,
                HorizontalOptions = LayoutOptions.Fill,
                BackgroundColor = card.IsFaceUp
                    ? Color.FromArgb("#FAF3E8")
                    : Color.FromArgb("#7B9E6B"),
                Stroke = Color.FromArgb("#C4A882")
            };

            if (card.IsFaceUp)
            {
                cardBorder.Content = BuildCardFaceContent(card);
            }
            else
            {
                // Dorso de la carta
                cardBorder.Content = new Label
                {
                    Text = "🌿",
                    FontSize = 16,
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center,
                    Opacity = 0.5
                };
            }

            wrapper.Children.Add(cardBorder);

            // ── Gestures ───────────────────────────────────────────
            // Usamos TapGestureRecognizer en el wrapper directamente
            // (más confiable en Android que en Border/ContentView)
            if (card.IsFaceUp)
            {
                int capturedCol = colIndex;
                int capturedCard = cardIndex;

                var tap = new TapGestureRecognizer();
                tap.Tapped += (s, e) =>
                {
                    System.Diagnostics.Debug.WriteLine(
                        $"[GamePage] Card tapped: col={capturedCol}, card={capturedCard}");
                    OnCardTapped(capturedCol, capturedCard);
                };

                // Agregar al wrapper Y al border para máxima superficie de tap
                wrapper.GestureRecognizers.Add(tap);
                cardBorder.GestureRecognizers.Add(tap);
            }

            return wrapper;
        }

        // ── Contenido visual de carta boca arriba ─────────────────
        private static View BuildCardFaceContent(CardDto card)
        {
            bool isRed = card.SuitName is "Hearts" or "Diamonds";
            var textColor = isRed
                ? Color.FromArgb("#C0392B")
                : Color.FromArgb("#2C1810");

            return new Grid
            {
                Padding = new Thickness(4, 3),
                Children =
                {
                    // Esquina superior izquierda
                    new VerticalStackLayout
                    {
                        VerticalOptions   = LayoutOptions.Start,
                        HorizontalOptions = LayoutOptions.Start,
                        Spacing           = -2,
                        Children =
                        {
                            new Label
                            {
                                Text      = card.RankLabel,
                                FontSize  = 13,
                                FontAttributes = FontAttributes.Bold,
                                TextColor = textColor,
                                LineBreakMode = LineBreakMode.NoWrap
                            },
                            new Label
                            {
                                Text      = card.SuitSymbol,
                                FontSize  = 11,
                                TextColor = textColor,
                                LineBreakMode = LineBreakMode.NoWrap
                            }
                        }
                    },
                    // Centro
                    new Label
                    {
                        Text              = card.SuitSymbol,
                        FontSize          = 20,
                        TextColor         = textColor,
                        HorizontalOptions = LayoutOptions.Center,
                        VerticalOptions   = LayoutOptions.Center
                    }
                }
            };
        }

        // ── Lógica de selección y movimiento con tap ───────────────
        /// <summary>
        /// Sistema de tap en dos pasos:
        /// 1er tap: selecciona la carta origen (la resalta)
        /// 2do tap: intenta moverla a la columna tapeada
        /// </summary>
        private void OnCardTapped(int colIndex, int cardIndex)
        {
            if (_selectedColumn == -1)
            {
                // Primera selección — marcar origen
                _selectedColumn = colIndex;
                _selectedCard = cardIndex;
                HighlightSelected(colIndex, cardIndex);

                System.Diagnostics.Debug.WriteLine(
                    $"[GamePage] Selected: col={colIndex}, card={cardIndex}");
            }
            else
            {
                // Segunda selección — intentar mover
                TryExecuteMove(colIndex);
            }
        }

        private void OnColumnTapped(int colIndex)
        {
            if (_selectedColumn == -1) return;

            // Si tapeó la misma columna, cancelar selección
            if (_selectedColumn == colIndex)
            {
                ClearSelection();
                return;
            }

            TryExecuteMove(colIndex);
        }

        private void TryExecuteMove(int targetColumn)
        {
            if (_selectedColumn == -1 || _selectedCard == -1)
            {
                ClearSelection();
                return;
            }

            int srcCol = _selectedColumn;
            int srcCard = _selectedCard;

            // Limpiar selección ANTES de ejecutar para evitar doble tap
            _selectedColumn = -1;
            _selectedCard = -1;

            System.Diagnostics.Debug.WriteLine(
                $"[GamePage] Executing move: {srcCol}[{srcCard}] → {targetColumn}");

            // Llamada directa al método del ViewModel — sin doble comando
            _viewModel.ExecuteMoveFromView(srcCol, srcCard, targetColumn);
        }

        private void ClearSelection()
        {
            _selectedColumn = -1;
            _selectedCard = -1;
            // Reconstruir para quitar highlights
            RebuildBoard();
        }

        private void HighlightSelected(int colIndex, int cardIndex)
        {
            // Reconstruir el tablero resaltando la carta seleccionada
            // La reconstrucción es barata porque son vistas nativas simples
            RebuildBoard();

            // Buscar y resaltar la carta en el grid reconstruido
            if (colIndex < BoardGrid.Children.Count)
            {
                var colView = BoardGrid.Children[colIndex];
                if (colView is VerticalStackLayout stack)
                {
                    // +1 por el highlight BoxView al inicio
                    int viewIndex = cardIndex + 1;
                    if (viewIndex < stack.Children.Count &&
                        stack.Children[viewIndex] is Grid wrapper &&
                        wrapper.Children.Count > 0 &&
                        wrapper.Children[0] is Border border)
                    {
                        border.Stroke = Color.FromArgb("#E8C97A");
                        border.StrokeThickness = 2.5;
                    }
                }
            }
        }
    }
}