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
using PokemonUltimate.Core.Instances;

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
        }

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

            _logger.LogInfo("Battle started");

            int turnCount = 0;

            while (Outcome == BattleOutcome.Ongoing && turnCount < BattleConstants.MaxTurns)
            {
                _logger.LogBattleEvent("TurnStart", $"Turn {turnCount + 1} starting");
                await RunTurn();
                turnCount++;

                // Check outcome after each turn
                Outcome = BattleArbiter.CheckOutcome(Field);
                _logger.LogBattleEvent("TurnEnd", $"Turn {turnCount} completed. Outcome: {Outcome}");
            }

            // Generate result
            var result = new BattleResult
            {
                Outcome = Outcome,
                TurnsTaken = turnCount
            };

            _logger.LogInfo($"Battle ended after {turnCount} turns. Outcome: {Outcome}");

            // TODO: Calculate MVP, defeated enemies, EXP, loot (Phase 2.7+)

            return result;
        }

        /// <summary>
        /// Executes a single turn of battle.
        /// </summary>
        public async Task RunTurn()
        {
            if (Field == null)
                throw new InvalidOperationException("CombatEngine must be initialized before running turn.");

            // 1. Collect actions from all active slots
            var pendingActions = new List<BattleAction>();
            foreach (var slot in Field.GetAllActiveSlots())
            {
                if (slot.ActionProvider != null)
                {
                    var action = await slot.ActionProvider.GetAction(Field, slot);
                    if (action != null)
                    {
                        pendingActions.Add(action);
                        _logger.LogDebug($"Collected action: {action.GetType().Name} from slot {slot.SlotIndex}");
                    }
                }
            }

            // 2. Sort by turn order (priority, then speed)
            var sortedActions = _turnOrderResolver.SortActions(pendingActions, Field);

            // 3. Enqueue all actions in sorted order
            Queue.EnqueueRange(sortedActions);

            // 4. Process the queue
            await Queue.ProcessQueue(Field, _view);

            // 5. End-of-turn effects (status damage, weather damage)
            var endOfTurnActions = _endOfTurnProcessor.ProcessEffects(Field);
            if (endOfTurnActions.Count > 0)
            {
                Queue.EnqueueRange(endOfTurnActions);
                await Queue.ProcessQueue(Field, _view);
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
            }

            // 10. Validate battle state after turn (in debug builds or when enabled)
            ValidateBattleState();
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

