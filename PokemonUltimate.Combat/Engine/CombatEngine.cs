using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PokemonUltimate.Combat.Damage;
using PokemonUltimate.Combat.Damage.Definition;
using PokemonUltimate.Combat.Engine.BattleFlow;
using PokemonUltimate.Combat.Engine.BattleFlow.Definition;
using PokemonUltimate.Combat.Engine.BattleFlow.Steps;
using PokemonUltimate.Combat.Engine.Service;
using PokemonUltimate.Combat.Engine.Validation;
using PokemonUltimate.Combat.Engine.Validation.Definition;
using PokemonUltimate.Combat.Field;
using PokemonUltimate.Combat.Handlers.Registry;
using PokemonUltimate.Combat.Infrastructure.Constants;
using PokemonUltimate.Combat.Infrastructure.Events;
using PokemonUltimate.Combat.Infrastructure.Factories.Definition;
using PokemonUltimate.Combat.Infrastructure.Logging;
using PokemonUltimate.Combat.Infrastructure.Logging.Definition;
using PokemonUltimate.Combat.Infrastructure.Providers.Definition;
using PokemonUltimate.Combat.Infrastructure.Statistics;
using PokemonUltimate.Combat.Utilities;
using PokemonUltimate.Combat.View.Definition;
using PokemonUltimate.Core.Data.Constants;
using PokemonUltimate.Core.Domain.Instances.Pokemon;

namespace PokemonUltimate.Combat.Engine
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
        private readonly IDamagePipeline _damagePipeline;
        private readonly CombatEffectHandlerRegistry _handlerRegistry;
        private readonly AccuracyChecker _accuracyChecker;
        private readonly Utilities.TargetResolver _targetResolver;

        // Battle flow executor
        private BattleFlowExecutor _battleFlowExecutor;
        private BattleFlowContext _battleFlowContext;

        // Tracked statistics collectors for cleanup
        private readonly List<BattleStatisticsCollector> _statisticsCollectors = new List<BattleStatisticsCollector>();


        /// <summary>
        /// The battlefield for this battle.
        /// </summary>
        public BattleField Field { get; private set; }

        /// <summary>
        /// The action queue for processing battle actions.
        /// </summary>
        public BattleQueueService QueueService { get; private set; }

        /// <summary>
        /// The current outcome of the battle.
        /// </summary>
        public BattleOutcome Outcome { get; private set; }

        /// <summary>
        /// The shared TargetResolver instance for this battle.
        /// Available before Initialize is called, so providers can use it.
        /// </summary>
        public Utilities.TargetResolver TargetResolver => _targetResolver;


        /// <summary>
        /// Creates a new CombatEngine with required dependencies.
        /// </summary>
        /// <param name="battleFieldFactory">Factory for creating BattleField instances. Cannot be null.</param>
        /// <param name="battleQueueFactory">Factory for creating BattleQueue instances. Cannot be null.</param>
        /// <param name="randomProvider">Random provider for random operations. Cannot be null.</param>
        /// <param name="accuracyChecker">Accuracy checker for move accuracy. If null, creates a temporary one.</param>
        /// <param name="damagePipeline">Damage pipeline for damage calculation. If null, creates a temporary one.</param>
        /// <param name="handlerRegistry">Unified handler registry. If null, creates and initializes a default one.</param>
        /// <param name="stateValidator">Battle state validator. If null, creates a default one.</param>
        /// <param name="logger">Battle logger. If null, creates a default one.</param>
        /// <exception cref="ArgumentNullException">If any required parameter is null.</exception>
        public CombatEngine(
            IBattleFieldFactory battleFieldFactory,
            IBattleQueueFactory battleQueueFactory,
            IRandomProvider randomProvider,
            AccuracyChecker accuracyChecker = null,
            IDamagePipeline damagePipeline = null,
            CombatEffectHandlerRegistry handlerRegistry = null,
            IBattleStateValidator stateValidator = null,
            IBattleLogger logger = null)
        {
            _battleFieldFactory = battleFieldFactory ?? throw new ArgumentNullException(nameof(battleFieldFactory));
            _battleQueueFactory = battleQueueFactory ?? throw new ArgumentNullException(nameof(battleQueueFactory));
            _randomProvider = randomProvider ?? throw new ArgumentNullException(nameof(randomProvider));

            // Create dependencies
            _stateValidator = stateValidator ?? new BattleStateValidator();
            _logger = logger ?? new BattleLogger("CombatEngine");

            // Store dependencies for use in Initialize
            _damagePipeline = damagePipeline ?? new DamagePipeline(_randomProvider);
            _handlerRegistry = handlerRegistry ?? CombatEffectHandlerRegistry.CreateDefault();
            _accuracyChecker = accuracyChecker ?? new AccuracyChecker(_randomProvider);
            _targetResolver = new Utilities.TargetResolver(); // Create shared instance

            // Battle flow executor will be created in Initialize
            _battleFlowExecutor = null;
            _battleFlowContext = null;
        }

        /// <summary>
        /// Initializes the combat engine with parties and action providers.
        /// Sets up the battle flow context and executor with all steps.
        /// </summary>
        /// <param name="rules">Battle configuration. Cannot be null.</param>
        /// <param name="playerParty">Player's Pokemon party. Cannot be null.</param>
        /// <param name="enemyParty">Enemy's Pokemon party. Cannot be null.</param>
        /// <param name="playerProvider">Provider for player actions. Cannot be null.</param>
        /// <param name="enemyProvider">Provider for enemy actions. Cannot be null.</param>
        /// <param name="view">Battle view for visual feedback. Cannot be null.</param>
        /// <param name="isDebugMode">Whether to enable debug events. Defaults to false.</param>
        /// <exception cref="ArgumentNullException">If any parameter is null.</exception>
        public void Initialize(
            BattleRules rules,
            IReadOnlyList<PokemonInstance> playerParty,
            IReadOnlyList<PokemonInstance> enemyParty,
            IActionProvider playerProvider,
            IActionProvider enemyProvider,
            IBattleView view,
            bool isDebugMode = false)
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

            // Create battle flow context
            _battleFlowContext = new BattleFlowContext
            {
                Rules = rules,
                PlayerParty = playerParty,
                EnemyParty = enemyParty,
                PlayerProvider = playerProvider,
                EnemyProvider = enemyProvider,
                View = view,
                StateValidator = _stateValidator,
                Logger = _logger,
                Engine = this, // Provide reference to engine for features
                TargetResolver = _targetResolver, // Share TargetResolver instance
                IsDebugMode = isDebugMode
            };

            // Create battle flow steps
            var steps = new List<IBattleFlowStep>
            {
                // === FASE 1: SETUP ===
                new CreateFieldStep(_battleFieldFactory),
                new AssignActionProvidersStep(),
                new CreateQueueStep(_battleQueueFactory),
                new ValidateInitialStateStep(),
                new CreateDependenciesStep(_randomProvider, _damagePipeline, _handlerRegistry, _accuracyChecker),

                // === FASE 2: EJECUCIÓN ===
                new BattleStartFlowStep(),
                new ExecuteBattleLoopStep(),

                // === FASE 3: CLEANUP ===
                new BattleEndFlowStep()
            };

            // Create battle flow executor
            _battleFlowExecutor = new BattleFlowExecutor(steps, _stateValidator, _logger);
        }

        /// <summary>
        /// Runs the complete battle until a conclusion is reached.
        /// Executes the unified battle flow using steps.
        /// </summary>
        /// <returns>The detailed battle result.</returns>
        public async Task<BattleResult> RunBattle()
        {
            if (_battleFlowExecutor == null || _battleFlowContext == null)
                throw new InvalidOperationException("CombatEngine must be initialized before running battle.");

            // Execute the complete battle flow (use ConfigureAwait(false) to avoid capturing context)
            await _battleFlowExecutor.ExecuteFlow(_battleFlowContext).ConfigureAwait(false);

            // Update properties from context
            Field = _battleFlowContext.Field;
            QueueService = _battleFlowContext.QueueService;
            Outcome = _battleFlowContext.Outcome;

            return _battleFlowContext.Result;
        }

        /// <summary>
        /// Registers a statistics collector for automatic cleanup on dispose.
        /// </summary>
        /// <param name="collector">The statistics collector to track.</param>
        internal void RegisterStatisticsCollector(BattleStatisticsCollector collector)
        {
            if (collector == null)
                return;

            if (!_statisticsCollectors.Contains(collector))
            {
                _statisticsCollectors.Add(collector);
            }
        }

        /// <summary>
        /// Cleans up resources, unsubscribing all tracked statistics collectors.
        /// </summary>
        public void Dispose()
        {
            // Unsubscribe all tracked statistics collectors
            foreach (var collector in _statisticsCollectors)
            {
                collector.Unsubscribe();
            }
            _statisticsCollectors.Clear();
        }

        /// <summary>
        /// Executes a single turn of battle.
        /// </summary>
        /// <param name="turnNumber">The current turn number (for event publishing).</param>
        public async Task RunTurn(int turnNumber = 0)
        {
            if (_battleFlowContext == null || _battleFlowContext.TurnEngine == null)
                throw new InvalidOperationException("CombatEngine must be initialized before running turn.");

            await _battleFlowContext.TurnEngine.ExecuteTurn(_battleFlowContext.Field, turnNumber);
        }

    }
}

