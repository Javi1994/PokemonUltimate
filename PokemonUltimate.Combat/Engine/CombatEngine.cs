using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PokemonUltimate.Combat.Actions;
using PokemonUltimate.Combat.Constants;
using PokemonUltimate.Combat.Damage;
using PokemonUltimate.Combat.Engine;
using PokemonUltimate.Combat.Events;
using PokemonUltimate.Combat.Factories;
using PokemonUltimate.Combat.Helpers;
using PokemonUltimate.Combat.Logging;
using PokemonUltimate.Combat.Providers;
using PokemonUltimate.Combat.Validation;
using PokemonUltimate.Core.Constants;
using PokemonUltimate.Core.Extensions;
using PokemonUltimate.Core.Instances;
using PokemonUltimate.Core.Localization;

namespace PokemonUltimate.Combat
{
    /// <summary>
    /// Main controller for battle execution.
    /// Orchestrates the full battle loop, turn execution, and outcome detection.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.6: Combat Engine
    /// **Documentation**: See `docs/features/2-combat-system/2.6-combat-engine/architecture.md`
    /// </remarks>
    public class CombatEngine
    {
        private readonly IBattleFieldFactory _battleFieldFactory;
        private readonly IBattleQueueFactory _battleQueueFactory;
        private readonly IRandomProvider _randomProvider;
        private readonly IEndOfTurnProcessor _endOfTurnProcessor;
        private readonly IBattleTriggerProcessor _battleTriggerProcessor;
        private readonly TurnOrderResolver _turnOrderResolver;
        private readonly AccuracyChecker _accuracyChecker;
        private readonly IDamagePipeline _damagePipeline;
        private readonly Effects.MoveEffectProcessorRegistry _effectProcessorRegistry;
        private readonly IBattleStateValidator _stateValidator;
        private readonly IBattleLogger _logger;
        private readonly Events.BattleEventBus _eventBus;

        private IBattleView _view;
        private IActionProvider _playerProvider;
        private IActionProvider _enemyProvider;

        /// <summary>
        /// The battlefield for this battle.
        /// </summary>
        public BattleField Field { get; private set; }

        /// <summary>
        /// The action queue for processing battle actions.
        /// </summary>
        public BattleQueue Queue { get; private set; }

        /// <summary>
        /// The current outcome of the battle.
        /// </summary>
        public BattleOutcome Outcome { get; private set; }

        /// <summary>
        /// Creates a new CombatEngine with required dependencies.
        /// </summary>
        /// <param name="battleFieldFactory">Factory for creating BattleField instances. Cannot be null.</param>
        /// <param name="battleQueueFactory">Factory for creating BattleQueue instances. Cannot be null.</param>
        /// <param name="randomProvider">Random provider for random operations. Cannot be null.</param>
        /// <param name="endOfTurnProcessor">Processor for end-of-turn effects. Cannot be null.</param>
        /// <param name="battleTriggerProcessor">Processor for battle triggers. Cannot be null.</param>
        /// <param name="accuracyChecker">Accuracy checker for move accuracy. If null, creates a temporary one.</param>
        /// <param name="damagePipeline">Damage pipeline for damage calculation. If null, creates a temporary one.</param>
        /// <param name="effectProcessorRegistry">Effect processor registry. If null, creates a temporary one.</param>
        /// <param name="stateValidator">Battle state validator. If null, creates a default one.</param>
        /// <param name="logger">Battle logger. If null, creates a default one.</param>
        /// <exception cref="ArgumentNullException">If any required parameter is null.</exception>
        public CombatEngine(
            IBattleFieldFactory battleFieldFactory,
            IBattleQueueFactory battleQueueFactory,
            IRandomProvider randomProvider,
            IEndOfTurnProcessor endOfTurnProcessor,
            IBattleTriggerProcessor battleTriggerProcessor,
            AccuracyChecker accuracyChecker = null,
            IDamagePipeline damagePipeline = null,
            Effects.MoveEffectProcessorRegistry effectProcessorRegistry = null,
            IBattleStateValidator stateValidator = null,
            IBattleLogger logger = null)
        {
            _battleFieldFactory = battleFieldFactory ?? throw new ArgumentNullException(nameof(battleFieldFactory));
            _battleQueueFactory = battleQueueFactory ?? throw new ArgumentNullException(nameof(battleQueueFactory));
            _randomProvider = randomProvider ?? throw new ArgumentNullException(nameof(randomProvider));
            _endOfTurnProcessor = endOfTurnProcessor ?? throw new ArgumentNullException(nameof(endOfTurnProcessor));
            _battleTriggerProcessor = battleTriggerProcessor ?? throw new ArgumentNullException(nameof(battleTriggerProcessor));

            // Create TurnOrderResolver with RandomProvider
            _turnOrderResolver = new TurnOrderResolver(_randomProvider);

            // Create AccuracyChecker if not provided (temporary until full DI refactoring)
            _accuracyChecker = accuracyChecker ?? new AccuracyChecker(_randomProvider);

            // Create DamagePipeline if not provided (temporary until full DI refactoring)
            _damagePipeline = damagePipeline ?? new DamagePipeline(_randomProvider);

            // Create MoveEffectProcessorRegistry if not provided (temporary until full DI refactoring)
            var damageContextFactory = new DamageContextFactory();
            _effectProcessorRegistry = effectProcessorRegistry ?? new Effects.MoveEffectProcessorRegistry(_randomProvider, damageContextFactory);

            // Create BattleStateValidator if not provided
            _stateValidator = stateValidator ?? new BattleStateValidator();

            // Create BattleLogger if not provided
            _logger = logger ?? new BattleLogger("CombatEngine");

            // Create unified event bus for all battle events (triggers and statistics/logging)
            _eventBus = new Events.BattleEventBus();
        }

        /// <summary>
        /// Gets the unified event bus for this battle.
        /// Handles both BattleTrigger events (abilities/items) and BattleEvent events (statistics/logging).
        /// </summary>
        public Events.BattleEventBus EventBus => _eventBus;

        /// <summary>
        /// Initializes the combat engine with parties and action providers.
        /// </summary>
        /// <param name="rules">Battle configuration. Cannot be null.</param>
        /// <param name="playerParty">Player's Pokemon party. Cannot be null.</param>
        /// <param name="enemyParty">Enemy's Pokemon party. Cannot be null.</param>
        /// <param name="playerProvider">Provider for player actions. Cannot be null.</param>
        /// <param name="enemyProvider">Provider for enemy actions. Cannot be null.</param>
        /// <param name="view">Battle view for visual feedback. Cannot be null.</param>
        /// <exception cref="ArgumentNullException">If any parameter is null.</exception>
        public void Initialize(
            BattleRules rules,
            IReadOnlyList<PokemonInstance> playerParty,
            IReadOnlyList<PokemonInstance> enemyParty,
            IActionProvider playerProvider,
            IActionProvider enemyProvider,
            IBattleView view)
        {
            if (rules == null)
                throw new ArgumentNullException(nameof(rules));
            if (playerParty == null)
                throw new ArgumentNullException(nameof(playerParty), ErrorMessages.PartyCannotBeNull);
            if (enemyParty == null)
                throw new ArgumentNullException(nameof(enemyParty), ErrorMessages.PartyCannotBeNull);
            if (playerProvider == null)
                throw new ArgumentNullException(nameof(playerProvider));
            if (enemyProvider == null)
                throw new ArgumentNullException(nameof(enemyProvider));
            if (view == null)
                throw new ArgumentNullException(nameof(view));

            _playerProvider = playerProvider;
            _enemyProvider = enemyProvider;
            _view = view;

            // Create BattleField using factory
            Field = _battleFieldFactory.Create(rules, playerParty, enemyParty);

            // Assign action providers to slots
            foreach (var slot in Field.PlayerSide.Slots)
            {
                slot.ActionProvider = _playerProvider;
            }

            foreach (var slot in Field.EnemySide.Slots)
            {
                slot.ActionProvider = _enemyProvider;
            }

            // Create BattleQueue using factory
            Queue = _battleQueueFactory.Create();
            Outcome = BattleOutcome.Ongoing;

            // Validate initial battle state
            ValidateBattleState();
        }

        /// <summary>
        /// Runs the complete battle until a conclusion is reached.
        /// </summary>
        /// <returns>The detailed battle result.</returns>
        public async Task<BattleResult> RunBattle()
        {
            if (Field == null)
                throw new InvalidOperationException("CombatEngine must be initialized before running battle.");

            // Determine battle mode name
            string battleMode = DetermineBattleMode(Field.Rules);

            // Collect initial Pokemon information
            var initialPlayerPokemon = Field.PlayerSide.Slots
                .Where(s => !s.IsEmpty && s.Pokemon != null)
                .Select(s => s.Pokemon)
                .ToList();
            var initialEnemyPokemon = Field.EnemySide.Slots
                .Where(s => !s.IsEmpty && s.Pokemon != null)
                .Select(s => s.Pokemon)
                .ToList();

            // Publish battle started event with setup information
            _eventBus.PublishEvent(new Events.BattleEvent(
                Events.BattleEventType.BattleStarted,
                turnNumber: 0,
                isPlayerSide: false,
                pokemon: initialPlayerPokemon.FirstOrDefault(), // First Pokemon as primary
                data: new Events.BattleEventData
                {
                    PlayerSlots = Field.Rules.PlayerSlots,
                    EnemySlots = Field.Rules.EnemySlots,
                    PlayerPartySize = Field.PlayerSide?.Party?.Count ?? 0,
                    EnemyPartySize = Field.EnemySide?.Party?.Count ?? 0,
                    BattleMode = battleMode,
                    // Store initial Pokemon counts in slots
                    RemainingPokemon = initialPlayerPokemon.Count, // Reuse field for initial active count
                    TotalPokemon = initialEnemyPokemon.Count // Reuse field for initial enemy active count
                }));

            // Publish additional events for initial Pokemon in each slot
            foreach (var slot in Field.PlayerSide.Slots)
            {
                if (!slot.IsEmpty && slot.Pokemon != null)
                {
                    _eventBus.PublishEvent(new Events.BattleEvent(
                        Events.BattleEventType.PokemonSwitched,
                        turnNumber: 0,
                        isPlayerSide: true,
                        pokemon: slot.Pokemon,
                        data: new Events.BattleEventData
                        {
                            SwitchedIn = slot.Pokemon,
                            SlotIndex = slot.SlotIndex
                        }));
                }
            }

            foreach (var slot in Field.EnemySide.Slots)
            {
                if (!slot.IsEmpty && slot.Pokemon != null)
                {
                    _eventBus.PublishEvent(new Events.BattleEvent(
                        Events.BattleEventType.PokemonSwitched,
                        turnNumber: 0,
                        isPlayerSide: false,
                        pokemon: slot.Pokemon,
                        data: new Events.BattleEventData
                        {
                            SwitchedIn = slot.Pokemon,
                            SlotIndex = slot.SlotIndex
                        }));
                }
            }

            int turnCount = 0;

            while (Outcome == BattleOutcome.Ongoing && turnCount < BattleConstants.MaxTurns)
            {
                int currentTurn = turnCount + 1;

                // Publish turn started event (logging handled by EventBasedBattleLogger)
                _eventBus.PublishEvent(new Events.BattleEvent(
                    Events.BattleEventType.TurnStarted,
                    turnNumber: currentTurn,
                    isPlayerSide: false,
                    data: new Events.BattleEventData()));

                await RunTurn(currentTurn);
                turnCount++;

                // Check outcome after each turn
                Outcome = BattleArbiter.CheckOutcome(Field);

                // Publish turn ended event (logging handled by EventBasedBattleLogger)
                _eventBus.PublishEvent(new Events.BattleEvent(
                    Events.BattleEventType.TurnEnded,
                    turnNumber: turnCount,
                    isPlayerSide: false,
                    data: new Events.BattleEventData()));
            }

            // Generate result
            var result = new BattleResult
            {
                Outcome = Outcome,
                TurnsTaken = turnCount
            };

            // Calculate team statistics for battle ended event
            int playerFainted = Field?.PlayerSide?.Party?.Count(p => p.IsFainted) ?? 0;
            int playerTotal = Field?.PlayerSide?.Party?.Count ?? 0;
            int enemyFainted = Field?.EnemySide?.Party?.Count(p => p.IsFainted) ?? 0;
            int enemyTotal = Field?.EnemySide?.Party?.Count ?? 0;

            // Publish battle ended event (logging handled by EventBasedBattleLogger)
            _eventBus.PublishEvent(new Events.BattleEvent(
                Events.BattleEventType.BattleEnded,
                turnNumber: turnCount,
                isPlayerSide: false,
                data: new Events.BattleEventData
                {
                    Outcome = Outcome,
                    TotalTurns = turnCount,
                    PlayerFainted = playerFainted,
                    PlayerTotal = playerTotal,
                    EnemyFainted = enemyFainted,
                    EnemyTotal = enemyTotal
                }));

            // TODO: Calculate MVP, defeated enemies, EXP, loot (Phase 2.7+)

            return result;
        }

        /// <summary>
        /// Executes a single turn of battle.
        /// </summary>
        /// <param name="turnNumber">The current turn number (for event publishing).</param>
        public async Task RunTurn(int turnNumber = 0)
        {
            if (Field == null)
                throw new InvalidOperationException("CombatEngine must be initialized before running turn.");

            // 1. Collect actions from all active slots (FASE DE SELECCIÓN)
            // En Pokémon, ambos jugadores eligen simultáneamente antes de ejecutar
            var pendingActions = new List<BattleAction>();
            foreach (var slot in Field.GetAllActiveSlots())
            {
                if (slot.ActionProvider != null)
                {
                    var action = await slot.ActionProvider.GetAction(Field, slot);
                    if (action != null)
                    {
                        pendingActions.Add(action);
                        // Logging handled by EventBasedBattleLogger via events
                    }
                }
            }

            // 2. Sort by turn order (priority, then speed) (FASE DE ORDENAMIENTO)
            // Ahora se ordenan todas las acciones por prioridad y velocidad
            var sortedActions = _turnOrderResolver.SortActions(pendingActions, Field);

            // Publish action collection events for debugging (AFTER sorting to show execution order)
            PublishActionCollectionEvent(sortedActions, turnNumber);

            // 3. Enqueue all actions in sorted order (FASE DE EJECUCIÓN)
            Queue.EnqueueRange(sortedActions);

            // 4. Process the queue
            await Queue.ProcessQueue(Field, _view);

            // 4.5. Handle automatic switching for fainted Pokemon (team battles)
            // Note: Turn number tracking would need to be passed - for now using 0
            await HandleFaintedPokemonSwitching();

            // 5. End-of-turn effects (status damage, weather damage)
            var endOfTurnActions = _endOfTurnProcessor.ProcessEffects(Field);
            if (endOfTurnActions.Count > 0)
            {
                Queue.EnqueueRange(endOfTurnActions);
                await Queue.ProcessQueue(Field, _view);

                // Check for fainted Pokemon again after end-of-turn effects
                await HandleFaintedPokemonSwitching();
            }

            // 6. Decrement weather duration
            Field.DecrementWeatherDuration();

            // 7. Decrement terrain duration
            Field.DecrementTerrainDuration();

            // 8. Decrement side condition durations
            Field.PlayerSide.DecrementAllSideConditionDurations();
            Field.EnemySide.DecrementAllSideConditionDurations();

            // 9. End-of-turn triggers (abilities and items)
            var triggerActions = _battleTriggerProcessor.ProcessTrigger(BattleTrigger.OnTurnEnd, Field);
            if (triggerActions.Count > 0)
            {
                Queue.EnqueueRange(triggerActions);
                await Queue.ProcessQueue(Field, _view);

                // Check for fainted Pokemon again after triggers
                await HandleFaintedPokemonSwitching();
            }

            // 10. Validate battle state after turn (in debug builds or when enabled)
            ValidateBattleState();
        }

        /// <summary>
        /// Handles automatic switching for fainted Pokemon in team battles.
        /// Checks all slots for fainted Pokemon and forces automatic switching if available.
        /// </summary>
        private async Task HandleFaintedPokemonSwitching()
        {
            if (Field == null)
                return;

            var switchActions = new List<BattleAction>();
            var currentTurn = 0; // Will be set by caller if tracking turns

            // Check all slots (not just active ones) for fainted Pokemon
            foreach (var slot in Field.PlayerSide.Slots.Concat(Field.EnemySide.Slots))
            {
                // Check if slot has a fainted Pokemon
                bool hasFaintedPokemon = slot.Pokemon != null && slot.Pokemon.IsFainted;

                // Check if slot is empty but party has active Pokemon available
                bool isEmptyButHasAvailablePokemon = slot.IsEmpty &&
                    slot.Side != null &&
                    slot.Side.Party != null &&
                    slot.Side.Party.Any(p => !p.IsFainted && !slot.Side.Slots.Any(s => s.Pokemon == p));

                if (hasFaintedPokemon || isEmptyButHasAvailablePokemon)
                {
                    // Try to get a switch action from the action provider
                    if (slot.ActionProvider != null)
                    {
                        try
                        {
                            var action = await slot.ActionProvider.GetAction(Field, slot);
                            if (action != null && action is SwitchAction switchAction)
                            {
                                switchActions.Add(switchAction);
                                // Logging handled by DetailedBattleLoggerObserver when action executes
                            }
                            else if (action == null && hasFaintedPokemon)
                            {
                                // No Pokemon available to switch - this is expected when party is exhausted
                                // Logging handled by EventBasedBattleLogger via events
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError($"Error getting switch action for fainted Pokemon: {ex.Message}");
                        }
                    }
                }
            }

            // Execute all switch actions
            if (switchActions.Count > 0)
            {
                // Logging handled by EventBasedBattleLogger via events
                Queue.EnqueueRange(switchActions);
                await Queue.ProcessQueue(Field, _view);
            }
        }

        private int _currentTurnNumber = 0;

        /// <summary>
        /// Publishes events for action collection and sorting (for debugging).
        /// Publishes events AFTER sorting to show the final execution order.
        /// </summary>
        private void PublishActionCollectionEvent(List<BattleAction> sortedActions, int turnNumber)
        {
            if (_eventBus == null || sortedActions == null)
                return;

            var actionDetails = new List<string>();

            // Publish event for each action in sorted order (shows execution order)
            int executionOrder = 1;
            foreach (var action in sortedActions)
            {
                if (action.User?.Pokemon == null)
                    continue;

                var speed = action.User != null ? _turnOrderResolver.GetEffectiveSpeed(action.User, Field) : 0;
                var sideName = action.User.Side.IsPlayer ? "Player" : "Enemy";
                var pokemonName = action.User.Pokemon.DisplayName;
                var priority = action.Priority;

                // Build detailed action description
                string actionDescription = $"{executionOrder}. [{sideName}] {pokemonName}";

                // Add action-specific details
                switch (action)
                {
                    case UseMoveAction moveAction:
                        // Use localized move name
                        var localizationProvider = LocalizationManager.Instance;
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

                _eventBus.PublishEvent(new Events.BattleEvent(
                    Events.BattleEventType.ActionCollected,
                    turnNumber: turnNumber,
                    isPlayerSide: action.User.Side.IsPlayer,
                    pokemon: action.User.Pokemon,
                    data: new Events.BattleEventData
                    {
                        ActionType = action.GetType().Name,
                        Priority = action.Priority,
                        Speed = speed,
                        SlotIndex = action.User.SlotIndex,
                        ActionCount = executionOrder // Use ActionCount to store execution order
                    }));
                executionOrder++;
            }

            // Publish event for sorted actions summary
            if (sortedActions.Count > 0)
            {
                _eventBus.PublishEvent(new Events.BattleEvent(
                    Events.BattleEventType.ActionsSorted,
                    turnNumber: turnNumber,
                    isPlayerSide: false,
                    data: new Events.BattleEventData
                    {
                        ActionCount = sortedActions.Count,
                        SortedActionsDetails = actionDetails
                    }));
            }
        }

        /// <summary>
        /// Determines the battle mode name based on slot configuration.
        /// </summary>
        private string DetermineBattleMode(BattleRules rules)
        {
            if (rules == null)
                return "Custom";

            // Standard modes
            if (rules.PlayerSlots == 1 && rules.EnemySlots == 1)
                return rules.IsBossBattle ? "Raid (1vBoss)" : "Singles";
            if (rules.PlayerSlots == 2 && rules.EnemySlots == 2)
                return "Doubles";
            if (rules.PlayerSlots == 3 && rules.EnemySlots == 3)
                return "Triples";

            // Horde modes
            if (rules.PlayerSlots == 1 && rules.EnemySlots == 2)
                return "Horde (1v2)";
            if (rules.PlayerSlots == 1 && rules.EnemySlots == 3)
                return "Horde (1v3)";
            if (rules.PlayerSlots == 1 && rules.EnemySlots == 5)
                return "Horde (1v5)";

            // Raid modes
            if (rules.IsBossBattle)
            {
                if (rules.PlayerSlots == 1 && rules.EnemySlots == 1)
                    return "Raid (1vBoss)";
                if (rules.PlayerSlots == 2 && rules.EnemySlots == 1)
                    return "Raid (2vBoss)";
            }

            // Custom mode
            return $"Custom ({rules.PlayerSlots}v{rules.EnemySlots})";
        }

        /// <summary>
        /// Validates the current battle state and throws an exception if invalid.
        /// </summary>
        /// <exception cref="InvalidOperationException">If battle state is invalid.</exception>
        private void ValidateBattleState()
        {
            var errors = _stateValidator.ValidateField(Field);
            if (errors.Count > 0)
            {
                var errorMessage = "Battle state validation failed:\n" + string.Join("\n", errors);
                _logger.LogError(errorMessage);
                throw new InvalidOperationException(errorMessage);
            }
        }
    }
}

