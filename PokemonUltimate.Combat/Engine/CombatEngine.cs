using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PokemonUltimate.Combat.Actions;
using PokemonUltimate.Combat.Constants;
using PokemonUltimate.Combat.Damage;
using PokemonUltimate.Combat.Events;
using PokemonUltimate.Combat.Factories;
using PokemonUltimate.Combat.Helpers;
using PokemonUltimate.Combat.Logging;
using PokemonUltimate.Combat.Processors.Phases;
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
        private readonly IBattleStateValidator _stateValidator;
        private readonly IBattleLogger _logger;

        // New processors and services
        private readonly BattleInitializer _battleInitializer;
        private readonly BattleStartProcessor _battleStartProcessor;
        private readonly BattleEndProcessor _battleEndProcessor;
        private BattleLoop _battleLoop;
        private TurnExecutor _turnExecutor;

        private IBattleView _view;
        private IActionProvider _playerProvider;
        private IActionProvider _enemyProvider;
        private Events.BattleEventBus _eventBus;

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
            AccuracyChecker accuracyChecker = null,
            IDamagePipeline damagePipeline = null,
            Effects.MoveEffectProcessorRegistry effectProcessorRegistry = null,
            IBattleStateValidator stateValidator = null,
            IBattleLogger logger = null)
        {
            _battleFieldFactory = battleFieldFactory ?? throw new ArgumentNullException(nameof(battleFieldFactory));
            _battleQueueFactory = battleQueueFactory ?? throw new ArgumentNullException(nameof(battleQueueFactory));
            _randomProvider = randomProvider ?? throw new ArgumentNullException(nameof(randomProvider));

            // Create dependencies
            _stateValidator = stateValidator ?? new BattleStateValidator();
            _logger = logger ?? new BattleLogger("CombatEngine");
            _eventBus = new Events.BattleEventBus();

            // Create processors (will be initialized with Queue/View in Initialize)
            _battleInitializer = new Processors.Phases.BattleInitializer(_battleFieldFactory, _battleQueueFactory, _stateValidator);
            _battleStartProcessor = new Processors.Phases.BattleStartProcessor(_eventBus);
            _battleEndProcessor = new Processors.Phases.BattleEndProcessor(_eventBus);

            // Processors will be created in Initialize after Queue and View are available
            _turnExecutor = null;
            _battleLoop = null;

            // Keep unused dependencies for compatibility (used elsewhere in the system)
            _ = accuracyChecker ?? new AccuracyChecker(_randomProvider);
            _ = damagePipeline ?? new DamagePipeline(_randomProvider);
            var damageContextFactory = new DamageContextFactory();
            _ = effectProcessorRegistry ?? new Effects.MoveEffectProcessorRegistry(_randomProvider, damageContextFactory);
        }

        /// <summary>
        /// Gets the event bus for this battle.
        /// Handles BattleEvent events for statistics/logging.
        /// Processors handle abilities/items directly without using the event bus.
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

            // Use BattleInitializer
            (Field, Queue) = _battleInitializer.Initialize(
                rules, playerParty, enemyParty, playerProvider, enemyProvider);

            Outcome = BattleOutcome.Ongoing;

            // Create processors that need Queue/View (now available)
            var turnOrderResolver = new TurnOrderResolver(_randomProvider);
            var turnStartProcessor = new Processors.Phases.TurnStartProcessor(_eventBus);
            var actionCollectionProcessor = new Processors.Phases.ActionCollectionProcessor(_randomProvider);
            var actionSortingProcessor = new Processors.Phases.ActionSortingProcessor(turnOrderResolver);
            var faintedSwitchingProcessor = new Processors.Phases.FaintedPokemonSwitchingProcessor(_randomProvider, _logger);

            var damageContextFactory = new DamageContextFactory();
            var endOfTurnEffectsProcessor = new Processors.Phases.EndOfTurnEffectsProcessor(damageContextFactory);
            var durationDecrementProcessor = new Processors.Phases.DurationDecrementProcessor();
            var turnEndProcessor = new Processors.Phases.TurnEndProcessor();

            // Create processors for ActionProcessorObserver
            var damageTakenProcessor = new Processors.Phases.DamageTakenProcessor();
            var contactReceivedProcessor = new Processors.Phases.ContactReceivedProcessor();
            var weatherChangeProcessor = new Processors.Phases.WeatherChangeProcessor();
            var switchInProcessor = new Processors.Phases.SwitchInProcessor(damageContextFactory);

            // Create TurnExecutor
            _turnExecutor = new Processors.Phases.TurnExecutor(
                turnStartProcessor,
                actionCollectionProcessor,
                actionSortingProcessor,
                faintedSwitchingProcessor,
                endOfTurnEffectsProcessor,
                durationDecrementProcessor,
                turnEndProcessor,
                _eventBus,
                Queue,
                view,
                _stateValidator,
                _logger,
                turnOrderResolver,
                damageTakenProcessor,
                contactReceivedProcessor,
                weatherChangeProcessor,
                switchInProcessor);

            // Create BattleLoop
            _battleLoop = new Processors.Phases.BattleLoop(
                _eventBus,
                _logger,
                async (turnNumber) => await _turnExecutor.ExecuteTurn(Field, turnNumber));
        }

        /// <summary>
        /// Runs the complete battle until a conclusion is reached.
        /// </summary>
        /// <returns>The detailed battle result.</returns>
        public async Task<BattleResult> RunBattle()
        {
            if (Field == null)
                throw new InvalidOperationException("CombatEngine must be initialized before running battle.");

            // Process battle start
            await _battleStartProcessor.ProcessAsync(Field);

            // Run battle loop
            var result = await _battleLoop.RunBattle(Field, Outcome);

            // Update outcome from result
            Outcome = result.Outcome;

            // Process battle end
            _battleEndProcessor.ProcessBattleEnd(Field, result);

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

            await _turnExecutor.ExecuteTurn(Field, turnNumber);
        }

    }
}

