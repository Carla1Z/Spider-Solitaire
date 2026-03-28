// Servicio principal del juego. Orquesta toda la lógica de negocio.
// Depende de abstracciones (interfaces) para cumplir Dependency Inversion.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SpiderSolitaire.DTOs;
using SpiderSolitaire.Interfaces;
using SpiderSolitaire.Mappers;
using SpiderSolitaire.Models;

namespace SpiderSolitaire.Services
{
    /// <summary>
    /// Implementación principal de IGameService.
    /// Orquesta: creación de partida, movimientos, deal, undo y detección de victoria.
    /// </summary>
    public class GameService : IGameService
    {
        // ── Dependencias ───────────────────────────────────────────
        private readonly IDeckService _deckService;
        private readonly IScoreService _scoreService;
        private readonly IGameRepository _repository;
        private readonly GameValidationService _validator;
        private readonly GameMapper _mapper;

        // ── Estado interno ─────────────────────────────────────────
        private GameState _state = new();

        // ── Constructor ────────────────────────────────────────────
        public GameService(
            IDeckService deckService,
            IScoreService scoreService,
            IGameRepository repository,
            GameValidationService validator,
            GameMapper mapper)
        {
            _deckService = deckService;
            _scoreService = scoreService;
            _repository = repository;
            _validator = validator;
            _mapper = mapper;
        }

        // ── IGameService: Ciclo de vida ────────────────────────────
        public async Task<GameStateDto> StartNewGameAsync(NewGameRequestDto request)
        {
            _state = BuildInitialState(request.Difficulty, request.Seed);
            await _repository.SaveGameAsync(_state);
            return _mapper.ToDto(_state);
        }

        public async Task<GameStateDto> RestartGameAsync()
        {
            var difficulty = _state.Difficulty;
            _state = BuildInitialState(difficulty, null);
            await _repository.SaveGameAsync(_state);
            return _mapper.ToDto(_state);
        }

        public GameStateDto GetCurrentState() => _mapper.ToDto(_state);

        // ── IGameService: Movimientos ──────────────────────────────
        public async Task<GameStateDto?> TryMoveCardsAsync(
            int sourceColumnIndex,
            int cardIndexInColumn,
            int targetColumnIndex)
        {
            if (_state.IsGameOver) return null;

            var source = _state.Columns[sourceColumnIndex];
            var target = _state.Columns[targetColumnIndex];

            if (!_validator.IsValidMove(source, cardIndexInColumn, target))
                return null;

            // ── Guardar snapshot para Undo ─────────────────────────
            PushSnapshot();

            // ── Ejecutar movimiento ────────────────────────────────
            var cardsToMove = source.GetCardsFrom(cardIndexInColumn);
            source.RemoveCardsFrom(cardIndexInColumn);
            source.FlipTopCard(); // Revelar carta que quedó expuesta

            target.PushRange(cardsToMove);

            // ── Registrar movimiento y actualizar puntaje ──────────
            var move = new Move
            {
                Type = MoveType.CardMove,
                SourceColumn = sourceColumnIndex,
                TargetColumn = targetColumnIndex,
                CardCount = cardsToMove.Count,
                Cards = cardsToMove
            };
            move.ScoreDelta = _scoreService.GetMoveScore(move);
            _state.Score += move.ScoreDelta;
            _state.Moves++;

            // ── Verificar secuencia completa en destino ────────────
            CheckAndRemoveCompletedSequence(target);

            // ── Verificar condición de victoria ────────────────────
            CheckWinCondition();

            // ── Verificar si no hay movimientos disponibles ────────
            if (!_state.IsGameOver && !HasAvailableMoves() && !_state.CanDeal)
                _state.Result = GameResult.Lost;

            await _repository.SaveGameAsync(_state);
            return _mapper.ToDto(_state);
        }

        public async Task<GameStateDto?> DealNextRoundAsync()
        {
            if (!_state.CanDeal) return null;

            // ── Guardar snapshot para Undo ─────────────────────────
            PushSnapshot();

            // ── Repartir 1 carta de la próxima pila a cada columna ─
            var pile = _state.StockPiles[^1];
            _state.StockPiles.RemoveAt(_state.StockPiles.Count - 1);

            for (int i = 0; i < _state.Columns.Count; i++)
            {
                var card = pile[i];
                card.IsFaceUp = true;
                _state.Columns[i].Push(card);
            }

            _state.DealsRemaining--;

            // ── Penalización de puntaje ────────────────────────────
            _state.Score += _scoreService.GetDealPenalty();
            _state.Moves++;

            // ── Verificar secuencias completadas ───────────────────
            foreach (var column in _state.Columns)
                CheckAndRemoveCompletedSequence(column);

            CheckWinCondition();

            await _repository.SaveGameAsync(_state);
            return _mapper.ToDto(_state);
        }

        public async Task<GameStateDto?> UndoLastMoveAsync()
        {
            if (!_state.CanUndo) return null;

            var snapshot = _state.UndoHistory.Pop();
            RestoreSnapshot(snapshot);

            _state.Score += _scoreService.GetUndoPenalty();
            _state.Moves++;

            await _repository.SaveGameAsync(_state);
            return _mapper.ToDto(_state);
        }

        // ── IGameService: Validación ───────────────────────────────
        public bool IsValidMove(
            int sourceColumnIndex,
            int cardIndexInColumn,
            int targetColumnIndex)
        {
            if (sourceColumnIndex < 0 || sourceColumnIndex >= _state.Columns.Count) return false;
            if (targetColumnIndex < 0 || targetColumnIndex >= _state.Columns.Count) return false;

            return _validator.IsValidMove(
                _state.Columns[sourceColumnIndex],
                cardIndexInColumn,
                _state.Columns[targetColumnIndex]);
        }

        public bool HasAvailableMoves()
            => _validator.HasAvailableMoves(_state.Columns);

        // ── Privados: Construcción inicial ─────────────────────────
        /// <summary>
        /// Construye el estado inicial de una partida nueva.
        /// Spider Solitaire estándar:
        /// - 10 columnas
        /// - Columnas 0–3: 6 cartas (última boca arriba)
        /// - Columnas 4–9: 5 cartas (última boca arriba)
        /// - 50 cartas restantes en 5 grupos de 10 para los Deals
        /// </summary>
        private GameState BuildInitialState(Difficulty difficulty, int? seed)
        {
            var deck = _deckService.CreateShuffledDeck(difficulty, seed);
            var state = new GameState
            {
                Difficulty = difficulty,
                Score = _scoreService.InitialScore,
                DealsRemaining = 5,
                Result = GameResult.InProgress,
                StartedAt = DateTime.UtcNow
            };

            // Crear 10 columnas
            for (int i = 0; i < 10; i++)
                state.Columns.Add(new Column { Index = i });

            // Distribuir cartas iniciales
            int deckIndex = 0;

            // Columnas 0–3 reciben 6 cartas
            for (int col = 0; col < 4; col++)
                for (int j = 0; j < 6; j++)
                    DealCardToColumn(state.Columns[col], deck[deckIndex++], isLast: j == 5);

            // Columnas 4–9 reciben 5 cartas
            for (int col = 4; col < 10; col++)
                for (int j = 0; j < 5; j++)
                    DealCardToColumn(state.Columns[col], deck[deckIndex++], isLast: j == 4);

            // Las 50 cartas restantes se agrupan en 5 pilas de 10 (para Deal)
            for (int pile = 0; pile < 5; pile++)
            {
                var group = new List<Card>();
                for (int j = 0; j < 10; j++)
                    group.Add(deck[deckIndex++]);
                state.StockPiles.Add(group);
            }

            return state;
        }

        private static void DealCardToColumn(Column column, Card card, bool isLast)
        {
            card.IsFaceUp = isLast; // Solo la última carta está boca arriba
            column.Push(card);
        }

        // ── Privados: Lógica del juego ─────────────────────────────
        /// <summary>
        /// Verifica si hay una secuencia K→A del mismo palo en la cima
        /// de la columna y la remueve si existe.
        /// </summary>
        private void CheckAndRemoveCompletedSequence(Column column)
        {
            if (!column.HasCompleteSequence()) return;

            // Tomar las últimas 13 cartas (la secuencia completa)
            var sequence = column.GetCardsFrom(column.Count - 13);
            column.RemoveCardsFrom(column.Count - 13);
            column.FlipTopCard();

            _state.CompletedSequences.Add(sequence);
            _state.Score += _scoreService.GetSequenceCompletionScore();
        }

        /// <summary>
        /// El jugador gana cuando completa las 8 secuencias
        /// (2 mazos × 4 palos = 8 secuencias posibles).
        /// </summary>
        private void CheckWinCondition()
        {
            if (_state.CompletedSequences.Count >= 8)
                _state.Result = GameResult.Won;
        }

        // ── Privados: Undo ─────────────────────────────────────────
        private void PushSnapshot()
        {
            // Limitar historial a 10 movimientos para no usar demasiada memoria
            if (_state.UndoHistory.Count >= 10)
            {
                // Convertir a lista, quitar el más antiguo y reconvertir
                var list = _state.UndoHistory.ToList();
                list.RemoveAt(list.Count - 1);
                _state.UndoHistory = new Stack<GameSnapshot>(list.AsEnumerable().Reverse());
            }

            var snapshot = new GameSnapshot
            {
                Score = _state.Score,
                Moves = _state.Moves,
                DealsLeft = _state.DealsRemaining,
                Completed = _state.CompletedSequences.Count
            };

            // Serializar el estado de las columnas
            foreach (var col in _state.Columns)
            {
                var colData = col.Cards
                    .Select(c => (c.Key, c.IsFaceUp))
                    .ToList();
                snapshot.ColumnCards.Add(colData);
            }

            _state.UndoHistory.Push(snapshot);
        }

        private void RestoreSnapshot(GameSnapshot snapshot)
        {
            // Reconstruir columnas desde el snapshot
            // Nota: en una implementación completa se reconstruirían
            // los objetos Card completos. Aquí simplificamos recreando
            // desde las claves guardadas.
            _state.Score = snapshot.Score;
            _state.Moves = snapshot.Moves;
            _state.DealsRemaining = snapshot.DealsLeft;
            _state.Result = GameResult.InProgress;

            // Reconstruir columnas desde el snapshot usando un lookup del mazo
            var allCards = GetAllCardsInPlay();
            var cardLookup = allCards.ToDictionary(c => c.Key + c.ColumnIndex + c.PositionInColumn);

            for (int i = 0; i < _state.Columns.Count && i < snapshot.ColumnCards.Count; i++)
            {
                _state.Columns[i].Cards.Clear();
                foreach (var (key, isFaceUp) in snapshot.ColumnCards[i])
                {
                    // Buscar carta por su clave (RankLabel + SuitSymbol)
                    var card = FindCardByKey(allCards, key);
                    if (card != null)
                    {
                        card.IsFaceUp = isFaceUp;
                        _state.Columns[i].Push(card);
                    }
                }
            }
        }

        /// <summary>
        /// Obtiene todas las cartas actualmente en juego (columnas + stock).
        /// </summary>
        private List<Card> GetAllCardsInPlay()
        {
            var all = new List<Card>();
            foreach (var col in _state.Columns)
                all.AddRange(col.Cards);
            foreach (var pile in _state.StockPiles)
                all.AddRange(pile);
            return all;
        }

        private static Card? FindCardByKey(List<Card> cards, string key)
            => cards.FirstOrDefault(c => c.Key == key);
    }
}