using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Maui.ApplicationModel;
using SpiderSolitaire.DTOs;
using SpiderSolitaire.Interfaces;
using SpiderSolitaire.Models;

namespace SpiderSolitaire.ViewModels
{
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

        // ── Opciones de dificultad ─────────────────────────────────
        public List<DifficultyOption> DifficultyOptions { get; } = new()
        {
            new(Difficulty.OneSuit,   "🌿 Un Palo",     "Ideal para principiantes"),
            new(Difficulty.TwoSuits,  "🌸 Dos Palos",   "Desafío moderado"),
            new(Difficulty.FourSuits, "🍂 Cuatro Palos", "Para expertos")
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

            // ✅ Command simple y síncrono para SelectDifficulty —
            // no necesita async, solo actualiza una propiedad
            SelectDifficultyCommand = new Command<string>(OnSelectDifficulty);

            // ✅ Los comandos async se envuelven en MainThread
            NewGameCommand = new Command(OnNewGame);
            ContinueGameCommand = new Command(OnContinueGame,
                                              () => HasSavedGame);
        }

        // ── Inicialización ─────────────────────────────────────────
        public async Task InitializeAsync()
        {
            try
            {
                HasSavedGame = await _repository.HasSavedGameAsync();
                (ContinueGameCommand as Command)?.ChangeCanExecute();
            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(
                    $"[MainMenuVM] InitializeAsync error: {ex.Message}");
            }
        }

        // ── Handlers ──────────────────────────────────────────────
        // ✅ Recibe string desde XAML CommandParameter y convierte a Difficulty
        private void OnSelectDifficulty(string? param)
        {
            if (param == null) return;

            // Mapear string al enum — CommandParameter en XAML es siempre string
            SelectedDifficulty = param switch
            {
                "0" => Difficulty.OneSuit,
                "1" => Difficulty.TwoSuits,
                "2" => Difficulty.FourSuits,
                _ => Difficulty.OneSuit
            };

            System.Diagnostics.Debug.WriteLine(
                $"[MainMenuVM] Difficulty selected: {SelectedDifficulty}");
        }

        private void OnNewGame()
        {
            // ✅ Toda la lógica async dentro de MainThread para garantizar
            // que Shell.GoToAsync se ejecute en el hilo correcto en Android
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                try
                {
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

                    System.Diagnostics.Debug.WriteLine(
                        $"[MainMenuVM] Navigating to game, difficulty: {request.Difficulty}");

                    await _navigation.NavigateToGameAsync(request);
                }
                catch (System.Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(
                        $"[MainMenuVM] OnNewGame error: {ex.Message}\n{ex.StackTrace}");
                }
            });
        }

        private void OnContinueGame()
        {
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                try
                {
                    var request = new NewGameRequestDto
                    {
                        Difficulty = SelectedDifficulty
                    };
                    await _navigation.NavigateToGameAsync(request);
                }
                catch (System.Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(
                        $"[MainMenuVM] OnContinueGame error: {ex.Message}");
                }
            });
        }
    }

    public record DifficultyOption(
        Difficulty Value,
        string Label,
        string Description);
}