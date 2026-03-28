// ViewModel de la pantalla principal / menú.
using System.Threading.Tasks;
using System.Windows.Input;
using SpiderSolitaire.DTOs;
using SpiderSolitaire.Interfaces;
using SpiderSolitaire.Models;

namespace SpiderSolitaire.ViewModels
{
    /// <summary>
    /// Maneja la lógica del menú principal:
    /// - Selección de dificultad
    /// - Iniciar nueva partida
    /// - Continuar partida guardada
    /// </summary>
    public class MainMenuViewModel : BaseViewModel
    {
        // ── Dependencias ───────────────────────────────────────────
        private readonly INavigationService _navigation;
        private readonly IGameRepository _repository;
        private readonly IDialogService _dialog;

        // ── Estado ─────────────────────────────────────────────────
        private Difficulty _selectedDifficulty = Difficulty.OneSuit;
        private bool _hasSavedGame;

        public Difficulty SelectedDifficulty
        {
            get => _selectedDifficulty;
            set => SetProperty(ref _selectedDifficulty, value);
        }

        public bool HasSavedGame
        {
            get => _hasSavedGame;
            set => SetProperty(ref _hasSavedGame, value);
        }

        // ── Opciones de dificultad para la UI ─────────────────────
        public DifficultyOption[] DifficultyOptions { get; } =
        {
            new(Difficulty.OneSuit,   "🌿 Un Palo",    "Ideal para principiantes"),
            new(Difficulty.TwoSuits,  "🌸 Dos Palos",  "Desafío moderado"),
            new(Difficulty.FourSuits, "🍂 Cuatro Palos","Para expertos")
        };

        // ── Comandos ───────────────────────────────────────────────
        public ICommand NewGameCommand { get; }
        public ICommand ContinueGameCommand { get; }
        public ICommand SelectDifficultyCommand { get; }

        // ── Constructor ────────────────────────────────────────────
        public MainMenuViewModel(
            INavigationService navigation,
            IGameRepository repository,
            IDialogService dialog)
        {
            _navigation = navigation;
            _repository = repository;
            _dialog = dialog;

            Title = "Spider Solitaire";

            NewGameCommand = new Command(async () => await OnNewGameAsync());
            ContinueGameCommand = new Command(async () => await OnContinueAsync(),
                                                  () => HasSavedGame);
            SelectDifficultyCommand = new Command<Difficulty>(d => SelectedDifficulty = d);
        }

        // ── Inicialización ─────────────────────────────────────────
        public async Task InitializeAsync()
        {
            await ExecuteAsync(async () =>
            {
                HasSavedGame = await _repository.HasSavedGameAsync();
                // Refrescar el estado del comando ContinueGame
                (ContinueGameCommand as Command)?.ChangeCanExecute();
            });
        }

        // ── Handlers de comandos ───────────────────────────────────
        private async Task OnNewGameAsync()
        {
            await ExecuteAsync(async () =>
            {
                // Si hay partida guardada, confirmar antes de sobrescribir
                if (HasSavedGame)
                {
                    bool confirm = await _dialog.ShowConfirmAsync(
                        "Nueva Partida",
                        "¿Abandonar la partida actual y comenzar una nueva?",
                        "Sí, nueva partida",
                        "Cancelar");

                    if (!confirm) return;
                }

                var request = new NewGameRequestDto
                {
                    Difficulty = SelectedDifficulty
                };
                await _navigation.NavigateToGameAsync(request);
            });
        }

        private async Task OnContinueAsync()
        {
            await ExecuteAsync(async () =>
            {
                var request = new NewGameRequestDto
                {
                    Difficulty = SelectedDifficulty
                };
                await _navigation.NavigateToGameAsync(request);
            });
        }
    }

    /// <summary>
    /// Helper para mostrar opciones de dificultad en la UI.
    /// </summary>
    public record DifficultyOption(
        Difficulty Value,
        string Label,
        string Description);
}