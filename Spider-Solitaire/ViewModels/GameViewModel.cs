using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Maui.ApplicationModel;
using SpiderSolitaire.DTOs;
using SpiderSolitaire.Interfaces;
using SpiderSolitaire.Models;

namespace SpiderSolitaire.ViewModels
{
    /// <summary>
    /// ViewModel del tablero de juego.
    /// QueryProperty recibe strings — la conversión a tipos ocurre aquí adentro.
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

        // ── Query Parameters ───────────────────────────────────────
        // Ambos se reciben como string desde Shell navigation
        private string _difficultyParam = "1";
        private string _seedParam = "";
        private bool _paramsReceived = false;

        public string DifficultyParam
        {
            get => _difficultyParam;
            set
            {
                _difficultyParam = value ?? "1";
                System.Diagnostics.Debug.WriteLine(
                    $"[GameVM] DifficultyParam received: {_difficultyParam}");
                TryStartGame();
            }
        }

        public string SeedParam
        {
            get => _seedParam;
            set
            {
                _seedParam = value ?? "";
                System.Diagnostics.Debug.WriteLine(
                    $"[GameVM] SeedParam received: {_seedParam}");
            }
        }

        // ── Estado del juego ───────────────────────────────────────
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

            _timer.Tick += OnTimerTick;

            DealCommand = new Command(OnDeal, () => CanDeal && !IsBusy);
            UndoCommand = new Command(OnUndo, () => CanUndo && !IsBusy);
            NewGameCommand = new Command(OnNewGame);
            RestartCommand = new Command(OnRestart);
            BackToMenuCommand = new Command(OnBackToMenu);
            CardTappedCommand = new Command<CardTapEventArgs>(OnCardTapped);

            DragStartedCommand = new Command<DragDropEventArgs>(OnDragStarted);
            DropCommand = new Command<DragDropEventArgs>(OnDrop);
            DragOverCommand = new Command<int>(OnDragOver);
            DragLeaveCommand = new Command(OnDragLeave);
        }

        // ── Inicialización ─────────────────────────────────────────
        /// <summary>
        /// Shell puede enviar los QueryProperty en cualquier orden.
        /// TryStartGame se llama solo cuando DifficultyParam ya llegó
        /// (SeedParam es opcional, puede llegar vacío).
        /// </summary>
        private void TryStartGame()
        {
            if (_paramsReceived) return;
            _paramsReceived = true;

            MainThread.BeginInvokeOnMainThread(async () =>
            {
                await InitializeGameAsync();
            });
        }

        public async Task InitializeGameAsync()
        {
            // Resetear flag para permitir reiniciar
            _paramsReceived = false;

            try
            {
                IsBusy = true;

                int difficulty = int.TryParse(_difficultyParam, out int d) ? d : 1;
                int? seed = int.TryParse(_seedParam, out int s) ? s : null;

                // Mapear int → Difficulty enum de forma segura
                var difficultyEnum = difficulty switch
                {
                    1 => Difficulty.OneSuit,
                    2 => Difficulty.TwoSuits,
                    4 => Difficulty.FourSuits,
                    _ => Difficulty.OneSuit
                };

                System.Diagnostics.Debug.WriteLine(
                    $"[GameVM] Starting game — difficulty: {difficultyEnum}, seed: {seed}");

                var request = new NewGameRequestDto
                {
                    Difficulty = difficultyEnum,
                    Seed = seed
                };

                var state = await _gameService.StartNewGameAsync(request);
                ApplyState(state);
                _timer.Start();

                System.Diagnostics.Debug.WriteLine(
                    $"[GameVM] Game started — columns: {state.Columns.Count}");
            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(
                    $"[GameVM] InitializeGameAsync error: {ex.Message}\n{ex.StackTrace}");
            }
            finally
            {
                IsBusy = false;
            }
        }

        // ── Handlers de comandos ───────────────────────────────────
        private void OnDeal()
        {
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                try
                {
                    IsBusy = true;
                    var state = await _gameService.DealNextRoundAsync();
                    if (state != null) ApplyState(state);
                    RefreshCommandStates();
                }
                catch (System.Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(
                        $"[GameVM] OnDeal error: {ex.Message}");
                }
                finally { IsBusy = false; }
            });
        }

        private void OnUndo()
        {
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                try
                {
                    IsBusy = true;
                    var state = await _gameService.UndoLastMoveAsync();
                    if (state != null) ApplyState(state);
                    RefreshCommandStates();
                }
                catch (System.Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(
                        $"[GameVM] OnUndo error: {ex.Message}");
                }
                finally { IsBusy = false; }
            });
        }

        private void OnNewGame()
        {
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                try
                {
                    bool confirm = await _dialog.ShowConfirmAsync(
                        "Nueva Partida",
                        "¿Abandonar la partida actual?",
                        "Sí", "No");

                    if (!confirm) return;

                    _timer.Reset();
                    _paramsReceived = false;
                    await InitializeGameAsync();
                }
                catch (System.Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(
                        $"[GameVM] OnNewGame error: {ex.Message}");
                }
            });
        }

        private void OnRestart()
        {
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                try
                {
                    bool confirm = await _dialog.ShowConfirmAsync(
                        "Reiniciar",
                        "¿Reiniciar la partida con el mismo mazo?",
                        "Sí", "No");

                    if (!confirm) return;

                    IsBusy = true;
                    _timer.Reset();
                    var state = await _gameService.RestartGameAsync();
                    ApplyState(state);
                    _timer.Start();
                }
                catch (System.Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(
                        $"[GameVM] OnRestart error: {ex.Message}");
                }
                finally { IsBusy = false; }
            });
        }

        private void OnBackToMenu()
        {
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                _timer.Pause();
                await _navigation.NavigateBackAsync();
            });
        }

        // ── Drag & Drop ────────────────────────────────────────────
        private void OnDragStarted(DragDropEventArgs args)
        {
            _dragSourceColumn = args.ColumnIndex;
            _dragCardIndex = args.CardIndex;
        }

        private void OnDrop(DragDropEventArgs args)
        {
            if (_dragSourceColumn == -1 || _dragCardIndex == -1) return;

            HighlightedColumn = -1;
            int srcCol = _dragSourceColumn;
            int cardIdx = _dragCardIndex;
            int tgtCol = args.ColumnIndex;

            _dragSourceColumn = -1;
            _dragCardIndex = -1;

            MainThread.BeginInvokeOnMainThread(async () =>
            {
                try
                {
                    IsBusy = true;
                    var state = await _gameService.TryMoveCardsAsync(
                        srcCol, cardIdx, tgtCol);

                    if (state != null)
                    {
                        ApplyState(state);
                        RefreshCommandStates();

                        if (state.IsGameOver)
                            await HandleGameOverAsync(state);
                    }
                }
                catch (System.Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(
                        $"[GameVM] OnDrop error: {ex.Message}");
                }
                finally { IsBusy = false; }
            });
        }

        private void OnDragOver(int columnIndex)
        {
            if (_dragSourceColumn >= 0 && _dragCardIndex >= 0)
            {
                bool valid = _gameService.IsValidMove(
                    _dragSourceColumn, _dragCardIndex, columnIndex);
                HighlightedColumn = valid ? columnIndex : -1;
            }
        }

        private void OnDragLeave() => HighlightedColumn = -1;

        private void OnCardTapped(CardTapEventArgs args)
        {
            if (_dragSourceColumn == -1)
            {
                _dragSourceColumn = args.ColumnIndex;
                _dragCardIndex = args.CardIndex;
            }
            else
            {
                var dropArgs = new DragDropEventArgs
                {
                    ColumnIndex = args.ColumnIndex
                };
                OnDrop(dropArgs);
            }
        }

        /// <summary>
        /// Método directo para mover cartas desde el code-behind.
        /// Evita la doble llamada DragStarted+Drop que puede tener
        /// condiciones de carrera en Android.
        /// </summary>
        public void ExecuteMoveFromView(
            int sourceColumn, int sourceCardIndex, int targetColumn)
        {
            System.Diagnostics.Debug.WriteLine(
                $"[GameVM] ExecuteMove: {sourceColumn}[{sourceCardIndex}] → {targetColumn}");

            MainThread.BeginInvokeOnMainThread(async () =>
            {
                try
                {
                    IsBusy = true;
                    var state = await _gameService.TryMoveCardsAsync(
                        sourceColumn, sourceCardIndex, targetColumn);

                    if (state != null)
                    {
                        ApplyState(state);
                        RefreshCommandStates();

                        if (state.IsGameOver)
                            await HandleGameOverAsync(state);
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine(
                            "[GameVM] Move invalid or rejected by service");
                    }
                }
                catch (System.Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(
                        $"[GameVM] ExecuteMove error: {ex.Message}");
                }
                finally
                {
                    IsBusy = false;
                }
            });
        }

        // ── Aplicar estado ─────────────────────────────────────────
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
                    $"Completaste el juego con {state.Score} puntos en {state.ElapsedTime}.",
                    "Gracias");
            }
            else
            {
                bool retry = await _dialog.ShowConfirmAsync(
                    "🍂 Fin del Juego",
                    "No hay más movimientos. ¿Intentar de nuevo?",
                    "Nueva Partida", "Menú");

                if (retry)
                    OnNewGame();
                else
                    OnBackToMenu();
            }
        }

        private void OnTimerTick(object? sender, System.TimeSpan elapsed)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                ElapsedTime = elapsed.ToString(@"mm\:ss");
            });
        }
    }

    // ── Event Args ─────────────────────────────────────────────────
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