// Behavior para iniciar el drag de una carta desde XAML.
using System.Windows.Input;
using Microsoft.Maui.Controls;

namespace SpiderSolitaire.Behaviors
{
    /// <summary>
    /// Behavior que habilita el arrastre de una carta.
    /// Se adjunta al View de cada carta en la UI.
    /// </summary>
    public class CardDragBehavior : Behavior<View>
    {
        // ── Bindable Properties ────────────────────────────────────
        public static readonly BindableProperty ColumnIndexProperty =
            BindableProperty.Create(
                nameof(ColumnIndex), typeof(int), typeof(CardDragBehavior), -1);

        public static readonly BindableProperty CardIndexProperty =
            BindableProperty.Create(
                nameof(CardIndex), typeof(int), typeof(CardDragBehavior), -1);

        public static readonly BindableProperty DragStartedCommandProperty =
            BindableProperty.Create(
                nameof(DragStartedCommand), typeof(ICommand), typeof(CardDragBehavior));

        public static readonly BindableProperty IsDraggableProperty =
            BindableProperty.Create(
                nameof(IsDraggable), typeof(bool), typeof(CardDragBehavior), false,
                propertyChanged: OnIsDraggableChanged);

        public int ColumnIndex { get => (int)GetValue(ColumnIndexProperty); set => SetValue(ColumnIndexProperty, value); }
        public int CardIndex { get => (int)GetValue(CardIndexProperty); set => SetValue(CardIndexProperty, value); }
        public ICommand DragStartedCommand { get => (ICommand)GetValue(DragStartedCommandProperty); set => SetValue(DragStartedCommandProperty, value); }
        public bool IsDraggable { get => (bool)GetValue(IsDraggableProperty); set => SetValue(IsDraggableProperty, value); }

        private DragGestureRecognizer? _dragGesture;
        private View? _attachedView;

        // ── Lifecycle ──────────────────────────────────────────────
        protected override void OnAttachedTo(View bindable)
        {
            base.OnAttachedTo(bindable);
            _attachedView = bindable;

            _dragGesture = new DragGestureRecognizer();
            _dragGesture.DragStarting += OnDragStarting;

            if (IsDraggable)
                bindable.GestureRecognizers.Add(_dragGesture);
        }

        protected override void OnDetachingFrom(View bindable)
        {
            base.OnDetachingFrom(bindable);
            if (_dragGesture != null)
            {
                _dragGesture.DragStarting -= OnDragStarting;
                bindable.GestureRecognizers.Remove(_dragGesture);
            }
        }

        // ── Handlers ───────────────────────────────────────────────
        private void OnDragStarting(object? sender, DragStartingEventArgs e)
        {
            // Pasar datos de la carta que se está arrastrando
            e.Data.Properties["columnIndex"] = ColumnIndex;
            e.Data.Properties["cardIndex"] = CardIndex;

            var args = new ViewModels.DragDropEventArgs
            {
                ColumnIndex = ColumnIndex,
                CardIndex = CardIndex
            };
            DragStartedCommand?.Execute(args);
        }

        private static void OnIsDraggableChanged(
            BindableObject bindable,
            object oldValue,
            object newValue)
        {
            if (bindable is CardDragBehavior behavior && behavior._attachedView != null)
            {
                bool draggable = (bool)newValue;
                if (draggable && behavior._dragGesture != null &&
                    !behavior._attachedView.GestureRecognizers.Contains(behavior._dragGesture))
                {
                    behavior._attachedView.GestureRecognizers.Add(behavior._dragGesture);
                }
                else if (!draggable && behavior._dragGesture != null)
                {
                    behavior._attachedView.GestureRecognizers.Remove(behavior._dragGesture);
                }
            }
        }
    }
}