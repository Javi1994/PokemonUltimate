using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PokemonUltimate.Combat.Actions;
using PokemonUltimate.Combat.Events;
using PokemonUltimate.Combat.Helpers;
using PokemonUltimate.Combat.Logging;
using PokemonUltimate.Combat.Validation;
using PokemonUltimate.Core.Constants;
using PokemonUltimate.Core.Extensions;

namespace PokemonUltimate.Combat.Processors.Phases
{
    /// <summary>
    /// Executes a single turn of battle by orchestrating all phase processors.
    /// Coordinates the execution of all phases in the correct order.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.6: Combat Engine
    /// **Documentation**: See `docs/features/2-combat-system/2.6-combat-engine/architecture.md`
    /// </remarks>
    public class TurnExecutor
    {
        private readonly TurnStartProcessor _turnStartProcessor;
        private readonly ActionCollectionProcessor _actionCollectionProcessor;
        private readonly ActionSortingProcessor _actionSortingProcessor;
        private readonly FaintedPokemonSwitchingProcessor _faintedSwitchingProcessor;
        private readonly EndOfTurnEffectsProcessor _endOfTurnEffectsProcessor;
        private readonly DurationDecrementProcessor _durationDecrementProcessor;
        private readonly TurnEndProcessor _turnEndProcessor;
        private readonly IBattleEventBus _eventBus;
        private readonly BattleQueue _queue;
        private readonly IBattleView _view;
        private readonly IBattleStateValidator _stateValidator;
        private readonly IBattleLogger _logger;
        private readonly TurnOrderResolver _turnOrderResolver;
        private readonly ActionProcessorObserver _actionProcessorObserver;

        /// <summary>
        /// Creates a new TurnExecutor with all required processors.
        /// </summary>
        public TurnExecutor(
            TurnStartProcessor turnStartProcessor,
            ActionCollectionProcessor actionCollectionProcessor,
            ActionSortingProcessor actionSortingProcessor,
            FaintedPokemonSwitchingProcessor faintedSwitchingProcessor,
            EndOfTurnEffectsProcessor endOfTurnEffectsProcessor,
            DurationDecrementProcessor durationDecrementProcessor,
            TurnEndProcessor turnEndProcessor,
            IBattleEventBus eventBus,
            BattleQueue queue,
            IBattleView view,
            IBattleStateValidator stateValidator,
            IBattleLogger logger,
            TurnOrderResolver turnOrderResolver,
            DamageTakenProcessor damageTakenProcessor = null,
            ContactReceivedProcessor contactReceivedProcessor = null,
            WeatherChangeProcessor weatherChangeProcessor = null,
            SwitchInProcessor switchInProcessor = null)
        {
            _turnStartProcessor = turnStartProcessor ?? throw new ArgumentNullException(nameof(turnStartProcessor));
            _actionCollectionProcessor = actionCollectionProcessor ?? throw new ArgumentNullException(nameof(actionCollectionProcessor));
            _actionSortingProcessor = actionSortingProcessor ?? throw new ArgumentNullException(nameof(actionSortingProcessor));
            _faintedSwitchingProcessor = faintedSwitchingProcessor ?? throw new ArgumentNullException(nameof(faintedSwitchingProcessor));
            _endOfTurnEffectsProcessor = endOfTurnEffectsProcessor ?? throw new ArgumentNullException(nameof(endOfTurnEffectsProcessor));
            _durationDecrementProcessor = durationDecrementProcessor ?? throw new ArgumentNullException(nameof(durationDecrementProcessor));
            _turnEndProcessor = turnEndProcessor ?? throw new ArgumentNullException(nameof(turnEndProcessor));
            _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
            _queue = queue ?? throw new ArgumentNullException(nameof(queue));
            _view = view ?? throw new ArgumentNullException(nameof(view));
            _stateValidator = stateValidator ?? throw new ArgumentNullException(nameof(stateValidator));
            _logger = logger ?? new Logging.NullBattleLogger();
            _turnOrderResolver = turnOrderResolver ?? throw new ArgumentNullException(nameof(turnOrderResolver));

            // Create and register ActionProcessorObserver to handle processor calls
            _actionProcessorObserver = new ActionProcessorObserver(
                _queue,
                damageTakenProcessor,
                contactReceivedProcessor,
                weatherChangeProcessor,
                switchInProcessor);
            _queue.AddObserver(_actionProcessorObserver);
        }

        /// <summary>
        /// Executes a single turn of battle.
        /// </summary>
        /// <param name="field">The battlefield. Cannot be null.</param>
        /// <param name="turnNumber">The current turn number (for event publishing).</param>
        /// <exception cref="ArgumentNullException">If field is null.</exception>
        public async Task ExecuteTurn(BattleField field, int turnNumber = 0)
        {
            if (field == null)
                throw new InvalidOperationException("BattleField must be initialized before running turn.");

            // 1. Turn start
            await _turnStartProcessor.ProcessAsync(field, turnNumber, _queue);

            // 2. Collect actions from all active slots
            var collectedActions = await _actionCollectionProcessor.ProcessAsync(field);

            // 3. Sort by turn order (priority, then speed)
            var sortedActions = _actionSortingProcessor.SortActions(collectedActions, field);

            // Publish action collection events for debugging (AFTER sorting to show execution order)
            PublishActionCollectionEvent(sortedActions, turnNumber);

            // 4. Enqueue all actions in sorted order
            _queue.EnqueueRange(sortedActions);

            // 5. Process the queue
            await _queue.ProcessQueue(field, _view);

            // 6. Handle automatic switching for fainted Pokemon
            var switchingActions = await _faintedSwitchingProcessor.ProcessAsync(field);
            if (switchingActions.Count > 0)
            {
                _queue.EnqueueRange(switchingActions);
                await _queue.ProcessQueue(field, _view);
            }

            // 7. End-of-turn effects (status damage, weather damage)
            var endOfTurnActions = await _endOfTurnEffectsProcessor.ProcessAsync(field);
            if (endOfTurnActions.Count > 0)
            {
                _queue.EnqueueRange(endOfTurnActions);
                await _queue.ProcessQueue(field, _view);

                // Check for fainted Pokemon again after end-of-turn effects
                var switchingActions2 = await _faintedSwitchingProcessor.ProcessAsync(field);
                if (switchingActions2.Count > 0)
                {
                    _queue.EnqueueRange(switchingActions2);
                    await _queue.ProcessQueue(field, _view);
                }
            }

            // 8. Decrement weather duration
            // 9. Decrement terrain duration
            // 10. Decrement side condition durations
            _durationDecrementProcessor.Process(field);

            // 11. End-of-turn triggers (abilities and items)
            var turnEndActions = await _turnEndProcessor.ProcessAsync(field);
            if (turnEndActions.Count > 0)
            {
                _queue.EnqueueRange(turnEndActions);
                await _queue.ProcessQueue(field, _view);

                // Check for fainted Pokemon again after triggers
                var switchingActions3 = await _faintedSwitchingProcessor.ProcessAsync(field);
                if (switchingActions3.Count > 0)
                {
                    _queue.EnqueueRange(switchingActions3);
                    await _queue.ProcessQueue(field, _view);
                }
            }

            // 12. Validate battle state after turn
            ValidateBattleState(field);
        }

        /// <summary>
        /// Publishes events for action collection and sorting (for debugging).
        /// </summary>
        private void PublishActionCollectionEvent(List<BattleAction> sortedActions, int turnNumber)
        {
            if (_eventBus == null || sortedActions == null)
                return;

            var actionDetails = new List<string>();

            // Publish event for each action in sorted order
            int executionOrder = 1;
            foreach (var action in sortedActions)
            {
                if (action.User?.Pokemon == null)
                    continue;

                var speed = action.User != null ? _turnOrderResolver.GetEffectiveSpeed(action.User, null) : 0;
                var sideName = action.User.Side.IsPlayer ? "Player" : "Enemy";
                var pokemonName = action.User.Pokemon.DisplayName;
                var priority = action.Priority;

                // Build detailed action description
                string actionDescription = $"{executionOrder}. [{sideName}] {pokemonName}";

                // Add action-specific details
                switch (action)
                {
                    case UseMoveAction moveAction:
                        var localizationProvider = Core.Localization.LocalizationManager.Instance;
                        var moveName = moveAction.Move.GetDisplayName(localizationProvider);
                        var targetName = moveAction.Target?.Pokemon?.DisplayName ?? "Unknown";
                        actionDescription += $" - {action.GetType().Name}: {moveName} → {targetName}";
                        break;
                    case SwitchAction switchAction:
                        var newPokemonName = switchAction.NewPokemon?.DisplayName ?? "Unknown";
                        actionDescription += $" - {action.GetType().Name}: switching to {newPokemonName}";
                        break;
                    default:
                        actionDescription += $" - {action.GetType().Name}";
                        break;
                }

                actionDescription += $" (Priority: {priority}, Speed: {speed:F1})";
                actionDetails.Add(actionDescription);

                _eventBus.PublishEvent(new BattleEvent(
                    BattleEventType.ActionCollected,
                    turnNumber: turnNumber,
                    isPlayerSide: action.User.Side.IsPlayer,
                    pokemon: action.User.Pokemon,
                    data: new BattleEventData
                    {
                        ActionType = action.GetType().Name,
                        Priority = action.Priority,
                        Speed = speed,
                        SlotIndex = action.User.SlotIndex,
                        ActionCount = executionOrder
                    }));
                executionOrder++;
            }

            // Publish event for sorted actions summary
            if (sortedActions.Count > 0)
            {
                _eventBus.PublishEvent(new BattleEvent(
                    BattleEventType.ActionsSorted,
                    turnNumber: turnNumber,
                    isPlayerSide: false,
                    data: new BattleEventData
                    {
                        ActionCount = sortedActions.Count,
                        SortedActionsDetails = actionDetails
                    }));
            }
        }

        /// <summary>
        /// Validates the current battle state and throws an exception if invalid.
        /// </summary>
        private void ValidateBattleState(BattleField field)
        {
            var errors = _stateValidator.ValidateField(field);
            if (errors.Count > 0)
            {
                var errorMessage = "Battle state validation failed:\n" + string.Join("\n", errors);
                _logger.LogError(errorMessage);
                throw new InvalidOperationException(errorMessage);
            }
        }
    }
}
