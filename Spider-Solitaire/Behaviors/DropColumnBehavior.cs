using System.Linq;
// Behavior para gestionar el Drop en columnas del tablero.
using System.Windows.Input;
using Microsoft.Maui.Controls;

namespace SpiderSolitaire.Behaviors
{
    /// <summary>
    /// Behavior que convierte un Layout en zona de drop para cartas.
    /// 
    /// NOTA: No usamos DataPackageOperation.Move porque es una API
    /// exclusiva de Windows (WinRT). En MAUI multiplataforma simplemente
    /// no seteamos AcceptedOperation y el framework lo maneja por defecto.
    /// </summary>
    public class DropColumnBehavior : Behavior<Layout>
    {
        // ── Bindable Properties ────────────────────────────────────
        public static readonly BindableProperty ColumnIndexProperty =
            BindableProperty.Create(
                nameof(ColumnIndex),
                typeof(int),
                typeof(DropColumnBehavior),
                -1);

        public static readonly BindableProperty DropCommandProperty =
            BindableProperty.Create(
                nameof(DropCommand),
                typeof(ICommand),
                typeof(DropColumnBehavior));

        public static readonly BindableProperty DragOverCommandProperty =
            BindableProperty.Create(
                nameof(DragOverCommand),
                typeof(ICommand),
                typeof(DropColumnBehavior));

        public static readonly BindableProperty DragLeaveCommandProperty =
            BindableProperty.Create(
                nameof(DragLeaveCommand),
                typeof(ICommand),
                typeof(DropColumnBehavior));

        public int ColumnIndex
        {
            get => (int)GetValue(ColumnIndexProperty);
            set => SetValue(ColumnIndexProperty, value);
        }

        public ICommand DropCommand
        {
            get => (ICommand)GetValue(DropCommandProperty);
            set => SetValue(DropCommandProperty, value);
        }

        public ICommand DragOverCommand
        {
            get => (ICommand)GetValue(DragOverCommandProperty);
            set => SetValue(DragOverCommandProperty, value);
        }

        public ICommand DragLeaveCommand
        {
            get => (ICommand)GetValue(DragLeaveCommandProperty);
            set => SetValue(DragLeaveCommandProperty, value);
        }

        // ── Lifecycle ──────────────────────────────────────────────
        protected override void OnAttachedTo(Layout bindable)
        {
            base.OnAttachedTo(bindable);

            var dropGesture = new DropGestureRecognizer();

            // ✅ NO usamos DataPackageOperation.Move (es solo Windows/WinRT)
            // MAUI acepta el drop por defecto sin necesidad de setearlo
            dropGesture.DragOver += OnDragOver;
            dropGesture.DragLeave += OnDragLeave;
            dropGesture.Drop += OnDrop;

            bindable.GestureRecognizers.Add(dropGesture);
        }

        protected override void OnDetachingFrom(Layout bindable)
        {
            base.OnDetachingFrom(bindable);

            // Limpiar todos los gesture recognizers de tipo Drop
            // para evitar memory leaks
            var toRemove = bindable.GestureRecognizers
                .OfType<DropGestureRecognizer>()
                .ToList();

            foreach (var gesture in toRemove)
            {
                gesture.DragOver -= OnDragOver;
                gesture.DragLeave -= OnDragLeave;
                gesture.Drop -= OnDrop;
                bindable.GestureRecognizers.Remove(gesture);
            }
        }

        // ── Handlers ───────────────────────────────────────────────
        private void OnDragOver(object? sender, DragEventArgs e)
        {
            // Sin DataPackageOperation — funciona en todas las plataformas
            DragOverCommand?.Execute(ColumnIndex);
        }

        private void OnDragLeave(object? sender, DragEventArgs e)
        {
            DragLeaveCommand?.Execute(null);
        }

        private void OnDrop(object? sender, DropEventArgs e)
        {
            var args = new ViewModels.DragDropEventArgs
            {
                ColumnIndex = ColumnIndex
            };
            DropCommand?.Execute(args);
        }
    }
}