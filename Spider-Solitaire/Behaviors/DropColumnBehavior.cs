// Behavior para gestionar el Drop en columnas del tablero.
// Permite adjuntar lógica de Drag & Drop a cualquier View desde XAML.
using System.Windows.Input;
using Microsoft.Maui.Controls;

namespace SpiderSolitaire.Behaviors
{
    /// <summary>
    /// Behavior que convierte un Layout en zona de drop para cartas.
    /// Uso en XAML:
    ///   &lt;StackLayout&gt;
    ///     &lt;StackLayout.Behaviors&gt;
    ///       &lt;behaviors:DropColumnBehavior
    ///           ColumnIndex="{Binding Index}"
    ///           DropCommand="{Binding Source={RelativeSource ...}, Path=DropCommand}"/&gt;
    ///     &lt;/StackLayout.Behaviors&gt;
    ///   &lt;/StackLayout&gt;
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

        public int ColumnIndex { get => (int)GetValue(ColumnIndexProperty); set => SetValue(ColumnIndexProperty, value); }
        public ICommand DropCommand { get => (ICommand)GetValue(DropCommandProperty); set => SetValue(DropCommandProperty, value); }
        public ICommand DragOverCommand { get => (ICommand)GetValue(DragOverCommandProperty); set => SetValue(DragOverCommandProperty, value); }
        public ICommand DragLeaveCommand { get => (ICommand)GetValue(DragLeaveCommandProperty); set => SetValue(DragLeaveCommandProperty, value); }

        // ── Lifecycle ──────────────────────────────────────────────
        protected override void OnAttachedTo(Layout bindable)
        {
            base.OnAttachedTo(bindable);

            var dropGesture = new DropGestureRecognizer();
            dropGesture.DragOver += OnDragOver;
            dropGesture.DragLeave += OnDragLeave;
            dropGesture.Drop += OnDrop;

            bindable.GestureRecognizers.Add(dropGesture);
        }

        protected override void OnDetachingFrom(Layout bindable)
        {
            base.OnDetachingFrom(bindable);
            // Limpiar gestures si es necesario
        }

        // ── Handlers ───────────────────────────────────────────────
        private void OnDragOver(object? sender, DragEventArgs e)
        {
            e.AcceptedOperation = DataPackageOperation.Move;
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