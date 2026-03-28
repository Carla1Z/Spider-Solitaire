// ViewModel principal del juego. Orquesta toda la interacción UI ↔ GameService.
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using SpiderSolitaire.DTOs;
using SpiderSolitaire.Interfaces;
using SpiderSolitaire.Models;

namespace SpiderSolitaire.ViewModels
{
    /// <summary>
    /// ViewModel del tablero de juego.
    /// Expone el estado del juego como propiedades observables
    /// y maneja los comandos de interacción del usuario.
    ///
    /// Principio: este VM no contiene lógica de juego,
    /// solo coordina entre la UI y IGameService.
    /// </summary>
    [QueryProperty(nameof(DifficultyParam), "difficulty")]
    [QueryProperty(nameof(SeedParam), "seed")]
    public class GameViewModel : BaseViewModel
    {
        // ── Dependencias ───────────────────────────────────────────
        private readonly IGameService _gameService;
        private readonly IDialogService _dialog;
        private readonly INavigationService _navigation;
        private readonly ITimerService _timer;

        // ── Query Parameters (Shell Navigation) ────────────────────
        private string _difficultyParam = "1";
        private string _seedParam = "";

        public string DifficultyParam
        {
            get => _difficultyParam;
            set
            {
                _difficultyParam = value;
                // Inicializar cuando llegan los parámetros de navegación
                _ = InitializeGameAsync();
            }
        }

        public string SeedParam
        {
            get => _seedParam;
            set => _seedParam = value;
        }

        // ── Estado del juego (bindeable) ───────────────────────────
        private ObservableCollection<ColumnDto> _columns = new();
        private int _score;
        private int _moves;
        private int _stockRemaining;
        private int _completedSequences;
        private string _elapsedTime = "00:00";
        private bool _canDeal;
        private bool _canUndo;
        private bool _isGameOver;
        private bool _hasWon;
        private string _difficultyLabel = "Un Palo";

        // ── Drag & Drop state ──────────────────────────────────────
        private int _dragSourceColumn = -1;
        private int _dragCardIndex = -1;
        private int _highlightedColumn = -1;

        // ── Propiedades públicas ───────────────────────────────────
        public ObservableCollection<ColumnDto> Columns
        {
            get => _columns;
            set => SetProperty(ref _columns, value);
        }

        public int Score
        {
            get => _score;
            set => SetProperty(ref _score, value);
        }

        public int Moves
        {
            get => _moves;
            set => SetProperty(ref _moves, value);
        }

        public int StockRemaining
        {
            get => _stockRemaining;
            set => SetProperty(ref _stockRemaining, value);
        }

        public int CompletedSequences
        {
            get => _completedSequences;
            set => SetProperty(ref _completedSequences, value);
        }

        public string ElapsedTime
        {
            get => _elapsedTime;
            set => SetProperty(ref _elapsedTime, value);
        }

        public bool CanDeal
        {
            get => _canDeal;
            set => SetProperty(ref _canDeal, value);
        }

        public bool CanUndo
        {
            get => _canUndo;
            set => SetProperty(ref _canUndo, value);
        }

        public bool IsGameOver
        {
            get => _isGameOver;
            set => SetProperty(ref _isGameOver, value);
        }

        public bool HasWon
        {
            get => _hasWon;
            set => SetProperty(ref _hasWon, value);
        }

        public string DifficultyLabel
        {
            get => _difficultyLabel;
            set => SetProperty(ref _difficultyLabel, value);
        }

        public int HighlightedColumn
        {
            get => _highlightedColumn;
            set => SetProperty(ref _highlightedColumn, value);
        }

        // ── Comandos ───────────────────────────────────────────────
        public ICommand DealCommand { get; }
        public ICommand UndoCommand { get; }
        public ICommand NewGameCommand { get; }
        public ICommand RestartCommand { get; }
        public ICommand BackToMenuCommand { get; }
        public ICommand CardTappedCommand { get; }

        // Comandos de Drag & Drop
        public ICommand DragStartedCommand { get; }
        public ICommand DropCommand { get; }
        public ICommand DragOverCommand { get; }
        public ICommand DragLeaveCommand { get; }

        // ── Constructor ────────────────────────────────────────────
        public GameViewModel(
            IGameService gameService,
            IDialogService dialog,
            INavigationService navigation,
            ITimerService timer)
        {
            _gameService = gameService;
            _dialog = dialog;
            _navigation = navigation;
            _timer = timer;

            Title = "Spider Solitaire 🌿";

            // Suscribir al tick del timer
            _timer.Tick += OnTimerTick;

            // Inicializar comandos
            DealCommand = new Command(async () => await OnDealAsync(), () => CanDeal && !IsBusy);
            UndoCommand = new Command(async () => await OnUndoAsync(), () => CanUndo && !IsBusy);
            NewGameCommand = new Command(async () => await OnNewGameAsync());
            RestartCommand = new Command(async () => await OnRestartAsync());
            BackToMenuCommand = new Command(async () => await OnBackToMenuAsync());
            CardTappedCommand = new Command<CardTapEventArgs>(OnCardTapped);

            DragStartedCommand = new Command<DragDropEventArgs>(OnDragStarted);
            DropCommand = new Command<DragDropEventArgs>(async args => await OnDropAsync(args));
            DragOverCommand = new Command<int>(OnDragOver);
            DragLeaveCommand = new Command(OnDragLeave);
        }

        // ── Inicialización ─────────────────────────────────────────
        public async Task InitializeGameAsync()
        {
            await ExecuteAsync(async () =>
            {
                int difficulty = int.TryParse(_difficultyParam, out int d) ? d : 1;
                int? seed = int.TryParse(_seedParam, out int s) ? s : null;

                var request = new NewGameRequestDto
                {
                    Difficulty = (Difficulty)difficulty,
                    Seed = seed
                };

                var state = await _gameService.StartNewGameAsync(request);
                ApplyState(state);
                _timer.Start();
            });
        }

        // ── Handlers de comandos ───────────────────────────────────
        private async Task OnDealAsync()
        {
            await ExecuteAsync(async () =>
            {
                var state = await _gameService.DealNextRoundAsync();
                if (state != null) ApplyState(state);
                RefreshCommandStates();
            });
        }

        private async Task OnUndoAsync()
        {
            await ExecuteAsync(async () =>
            {
                var state = await _gameService.UndoLastMoveAsync();
                if (state != null) ApplyState(state);
                RefreshCommandStates();
            });
        }

        private async Task OnNewGameAsync()
        {
            bool confirm = await _dialog.ShowConfirmAsync(
                "Nueva Partida",
                "¿Abandonar la partida actual?",
                "Sí", "No");

            if (!confirm) return;

            _timer.Reset();
            await InitializeGameAsync();
        }

        private async Task OnRestartAsync()
        {
            bool confirm = await _dialog.ShowConfirmAsync(
                "Reiniciar",
                "¿Reiniciar la partida con el mismo mazo?",
                "Sí", "No");

            if (!confirm) return;

            await ExecuteAsync(async () =>
            {
                _timer.Reset();
                var state = await _gameService.RestartGameAsync();
                ApplyState(state);
                _timer.Start();
            });
        }

        private async Task OnBackToMenuAsync()
        {
            _timer.Pause();
            await _navigation.NavigateBackAsync();
        }

        // ── Drag & Drop ────────────────────────────────────────────
        private void OnDragStarted(DragDropEventArgs args)
        {
            _dragSourceColumn = args.ColumnIndex;
            _dragCardIndex = args.CardIndex;
        }

        private async Task OnDropAsync(DragDropEventArgs args)
        {
            if (_dragSourceColumn == -1 || _dragCardIndex == -1) return;

            HighlightedColumn = -1;

            await ExecuteAsync(async () =>
            {
                var state = await _gameService.TryMoveCardsAsync(
                    _dragSourceColumn,
                    _dragCardIndex,
                    args.ColumnIndex);

                if (state != null)
                {
                    ApplyState(state);
                    RefreshCommandStates();

                    // Manejar fin de juego
                    if (state.IsGameOver)
                        await HandleGameOverAsync(state);
                }
            });

            _dragSourceColumn = -1;
            _dragCardIndex = -1;
        }

        private void OnDragOver(int columnIndex)
        {
            // Resaltar la columna destino solo si el movimiento sería válido
            if (_dragSourceColumn >= 0 && _dragCardIndex >= 0)
            {
                bool valid = _gameService.IsValidMove(
                    _dragSourceColumn,
                    _dragCardIndex,
                    columnIndex);

                HighlightedColumn = valid ? columnIndex : -1;
            }
        }

        private void OnDragLeave() => HighlightedColumn = -1;

        /// <summary>
        /// Tap en carta: selección para mover sin drag & drop.
        /// Primera carta tapeada = origen, segunda = destino.
        /// </summary>
        private void OnCardTapped(CardTapEventArgs args)
        {
            if (_dragSourceColumn == -1)
            {
                // Primera selección: marcar como origen
                _dragSourceColumn = args.ColumnIndex;
                _dragCardIndex = args.CardIndex;
                // Aquí podrías resaltar las cartas seleccionadas en la UI
            }
            else
            {
                // Segunda selección: intentar mover
                _ = OnDropAsync(new DragDropEventArgs
                {
                    ColumnIndex = args.ColumnIndex
                });
            }
        }

        // ── Aplicar estado ─────────────────────────────────────────
        /// <summary>
        /// Actualiza todas las propiedades bindeadas desde el DTO.
        /// Se llama después de cada operación del GameService.
        /// </summary>
        private void ApplyState(GameStateDto state)
        {
            Score = state.Score;
            Moves = state.Moves;
            StockRemaining = state.StockPilesRemaining;
            CompletedSequences = state.CompletedSequences;
            CanDeal = state.CanDeal;
            CanUndo = state.CanUndo;
            IsGameOver = state.IsGameOver;
            HasWon = state.HasWon;
            DifficultyLabel = state.Difficulty;
            ElapsedTime = state.ElapsedTime;

            // Reemplazar la colección de columnas
            Columns = new ObservableCollection<ColumnDto>(state.Columns);
        }

        private void RefreshCommandStates()
        {
            (DealCommand as Command)?.ChangeCanExecute();
            (UndoCommand as Command)?.ChangeCanExecute();
        }

        private async Task HandleGameOverAsync(GameStateDto state)
        {
            _timer.Pause();

            if (state.HasWon)
            {
                await _dialog.ShowAlertAsync(
                    "🌸 ¡Victoria!",
                    $"Completaste el juego con {state.Score} puntos en {state.ElapsedTime}. ¡Bien hecho!",
                    "Gracias");
            }
            else
            {
                bool retry = await _dialog.ShowConfirmAsync(
                    "🍂 Fin del Juego",
                    "No hay más movimientos disponibles. ¿Intentar de nuevo?",
                    "Nueva Partida", "Menú");

                if (retry)
                    await OnNewGameAsync();
                else
                    await OnBackToMenuAsync();
            }
        }

        private void OnTimerTick(object? sender, System.TimeSpan elapsed)
        {
            ElapsedTime = elapsed.ToString(@"mm\:ss");
        }
    }

    // ── Event Args para comandos tipados ──────────────────────────
    public class DragDropEventArgs
    {
        public int ColumnIndex { get; set; }
        public int CardIndex { get; set; }
    }

    public class CardTapEventArgs
    {
        public int ColumnIndex { get; set; }
        public int CardIndex { get; set; }
    }
}