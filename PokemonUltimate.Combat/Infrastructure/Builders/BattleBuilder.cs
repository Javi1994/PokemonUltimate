using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PokemonUltimate.Combat.AI;
using PokemonUltimate.Combat.Engine;
using PokemonUltimate.Combat.Field;
using PokemonUltimate.Combat.Infrastructure.Constants;
using PokemonUltimate.Combat.Infrastructure.Events;
using PokemonUltimate.Combat.Infrastructure.Factories;
using PokemonUltimate.Combat.Infrastructure.Providers.Definition;
using PokemonUltimate.Combat.Infrastructure.Statistics;
using PokemonUltimate.Combat.View;
using PokemonUltimate.Combat.View.Definition;
using PokemonUltimate.Content.Catalogs.Pokemon;
using PokemonUltimate.Core.Data.Blueprints;
using PokemonUltimate.Core.Data.Enums;
using PokemonUltimate.Core.Domain.Instances.Pokemon;
using PokemonUltimate.Core.Infrastructure.Factories;

namespace PokemonUltimate.Combat.Infrastructure.Builders
{
    /// <summary>
    /// Builder pattern for creating and initializing battles easily.
    /// Provides a fluent API for configuring battle setup.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.6: Combat Engine
    /// **Documentation**: See `docs/features/2-combat-system/2.6-combat-engine/architecture.md`
    /// </remarks>
    public class BattleBuilder
    {
        private readonly List<PokemonInstance> _playerParty = new List<PokemonInstance>();
        private readonly List<PokemonInstance> _enemyParty = new List<PokemonInstance>();
        private BattleRules _rules = BattleRules.Singles;
        private IActionProvider _playerProvider;
        private IActionProvider _enemyProvider;
        private IBattleView _view = NullBattleView.Instance;
        private CombatEngine _engine;
        private int? _randomSeed;
        private static readonly Random _defaultRandom = new Random();
        private Random _random = _defaultRandom;
        private bool _useFullTeam = false; // Flag to indicate if full team (6 Pokemon) should be used
        private bool _isDebugMode = false; // Flag to enable debug events

        // Event handlers
        private EventHandler<BattleStartEventArgs> _onBattleStart;
        private EventHandler<BattleEndEventArgs> _onBattleEnd;
        private EventHandler<TurnEventArgs> _onTurnStart;
        private EventHandler<TurnEventArgs> _onTurnEnd;
        private EventHandler<ActionExecutedEventArgs> _onActionExecuted;
        private EventHandler<StepExecutedEventArgs> _onStepExecuted;
        private EventHandler<StepStartedEventArgs> _onStepStarted;
        private EventHandler<StepFinishedEventArgs> _onStepFinished;
        private EventHandler<BattleStateChangedEventArgs> _onBattleStateChanged;
        private BattleStatisticsCollector _statisticsCollector;

        /// <summary>
        /// Creates a new BattleBuilder instance.
        /// </summary>
        public static BattleBuilder Create() => new BattleBuilder();

        /// <summary>
        /// Adds a Pokemon instance to the player's party.
        /// </summary>
        public BattleBuilder WithPlayerPokemon(PokemonInstance pokemon)
        {
            if (pokemon == null)
                throw new ArgumentNullException(nameof(pokemon));
            _playerParty.Add(pokemon);
            return this;
        }

        /// <summary>
        /// Adds multiple Pokemon to the player's party.
        /// </summary>
        public BattleBuilder WithPlayerParty(params PokemonInstance[] party)
        {
            if (party == null)
                throw new ArgumentNullException(nameof(party));
            _playerParty.AddRange(party);
            return this;
        }

        /// <summary>
        /// Adds multiple Pokemon to the player's party from a collection.
        /// </summary>
        public BattleBuilder WithPlayerParty(IEnumerable<PokemonInstance> party)
        {
            if (party == null)
                throw new ArgumentNullException(nameof(party));
            _playerParty.AddRange(party);
            return this;
        }

        /// <summary>
        /// Adds a Pokemon instance to the enemy's party.
        /// </summary>
        public BattleBuilder WithEnemyPokemon(PokemonInstance pokemon)
        {
            if (pokemon == null)
                throw new ArgumentNullException(nameof(pokemon));
            _enemyParty.Add(pokemon);
            return this;
        }

        /// <summary>
        /// Adds multiple Pokemon to the enemy's party.
        /// </summary>
        public BattleBuilder WithEnemyParty(params PokemonInstance[] party)
        {
            if (party == null)
                throw new ArgumentNullException(nameof(party));
            _enemyParty.AddRange(party);
            return this;
        }

        /// <summary>
        /// Adds multiple Pokemon to the enemy's party from a collection.
        /// </summary>
        public BattleBuilder WithEnemyParty(IEnumerable<PokemonInstance> party)
        {
            if (party == null)
                throw new ArgumentNullException(nameof(party));
            _enemyParty.AddRange(party);
            return this;
        }

        /// <summary>
        /// Adds a random Pokemon from the catalog to the player's party.
        /// </summary>
        /// <param name="level">The level for the Pokemon.</param>
        /// <param name="excludeSpecies">Optional Pokemon species to exclude from selection.</param>
        public BattleBuilder WithRandomPlayerPokemon(int level, PokemonSpeciesData excludeSpecies = null)
        {
            var species = GetRandomSpecies(excludeSpecies);
            _playerParty.Add(PokemonFactory.Create(species, level));
            return this;
        }

        /// <summary>
        /// Adds a random Pokemon from the catalog to the enemy's party.
        /// Automatically excludes Pokemon already in the player's party to avoid duplicates.
        /// </summary>
        /// <param name="level">The level for the Pokemon.</param>
        /// <param name="excludeSpecies">Optional Pokemon species to exclude from selection.</param>
        public BattleBuilder WithRandomEnemyPokemon(int level, PokemonSpeciesData excludeSpecies = null)
        {
            // Get all player Pokemon species to exclude
            var excludedSpecies = _playerParty.Select(p => p.Species).ToList();
            if (excludeSpecies != null)
                excludedSpecies.Add(excludeSpecies);

            var species = GetRandomSpecies(excludedSpecies);
            _enemyParty.Add(PokemonFactory.Create(species, level));
            return this;
        }

        /// <summary>
        /// Adds multiple random Pokemon to the player's party.
        /// </summary>
        /// <param name="count">Number of random Pokemon to add.</param>
        /// <param name="level">The level for all Pokemon.</param>
        /// <param name="allowDuplicates">Whether to allow duplicate species in the party.</param>
        public BattleBuilder WithRandomPlayerParty(int count, int level, bool allowDuplicates = false)
        {
            if (count <= 0)
                throw new ArgumentException("Count must be greater than 0", nameof(count));

            var excludedSpecies = allowDuplicates ? new List<PokemonSpeciesData>() : new List<PokemonSpeciesData>();

            for (int i = 0; i < count; i++)
            {
                var species = GetRandomSpecies(excludedSpecies);
                _playerParty.Add(PokemonFactory.Create(species, level));

                if (!allowDuplicates)
                    excludedSpecies.Add(species);
            }

            return this;
        }

        /// <summary>
        /// Adds multiple random Pokemon to the enemy's party.
        /// Automatically excludes Pokemon already in the player's party.
        /// </summary>
        /// <param name="count">Number of random Pokemon to add.</param>
        /// <param name="level">The level for all Pokemon.</param>
        /// <param name="allowDuplicates">Whether to allow duplicate species in the enemy party.</param>
        public BattleBuilder WithRandomEnemyParty(int count, int level, bool allowDuplicates = false)
        {
            if (count <= 0)
                throw new ArgumentException("Count must be greater than 0", nameof(count));

            // Always exclude player Pokemon
            var excludedSpecies = _playerParty.Select(p => p.Species).ToList();

            for (int i = 0; i < count; i++)
            {
                var species = GetRandomSpecies(excludedSpecies);
                _enemyParty.Add(PokemonFactory.Create(species, level));

                if (!allowDuplicates)
                    excludedSpecies.Add(species);
            }

            return this;
        }

        /// <summary>
        /// Adds a random Pokemon of a specific type to the player's party.
        /// </summary>
        /// <param name="type">The Pokemon type to filter by.</param>
        /// <param name="level">The level for the Pokemon.</param>
        /// <param name="excludeSpecies">Optional Pokemon species to exclude from selection.</param>
        public BattleBuilder WithRandomPlayerPokemonOfType(PokemonType type, int level, PokemonSpeciesData excludeSpecies = null)
        {
            var availablePokemon = PokemonCatalog.GetAllByType(type).ToList();
            if (excludeSpecies != null)
                availablePokemon.Remove(excludeSpecies);

            if (availablePokemon.Count == 0)
                throw new InvalidOperationException($"No Pokemon of type {type} available in catalog.");

            var species = availablePokemon[_random.Next(availablePokemon.Count)];
            _playerParty.Add(PokemonFactory.Create(species, level));
            return this;
        }

        /// <summary>
        /// Adds a random Pokemon of a specific type to the enemy's party.
        /// Automatically excludes Pokemon already in the player's party.
        /// </summary>
        /// <param name="type">The Pokemon type to filter by.</param>
        /// <param name="level">The level for the Pokemon.</param>
        /// <param name="excludeSpecies">Optional Pokemon species to exclude from selection.</param>
        public BattleBuilder WithRandomEnemyPokemonOfType(PokemonType type, int level, PokemonSpeciesData excludeSpecies = null)
        {
            var excludedSpecies = _playerParty.Select(p => p.Species).ToList();
            if (excludeSpecies != null)
                excludedSpecies.Add(excludeSpecies);

            var availablePokemon = PokemonCatalog.GetAllByType(type)
                .Where(p => !excludedSpecies.Contains(p))
                .ToList();

            if (availablePokemon.Count == 0)
                throw new InvalidOperationException($"No Pokemon of type {type} available (excluding player party).");

            var species = availablePokemon[_random.Next(availablePokemon.Count)];
            _enemyParty.Add(PokemonFactory.Create(species, level));
            return this;
        }

        /// <summary>
        /// Sets a custom Random instance for random Pokemon selection.
        /// Useful for reproducible battles with specific random seeds.
        /// </summary>
        public BattleBuilder WithRandom(Random random)
        {
            _random = random ?? throw new ArgumentNullException(nameof(random));
            return this;
        }

        /// <summary>
        /// Gets the player party that will be used in the battle.
        /// </summary>
        public IReadOnlyList<PokemonInstance> PlayerParty => _playerParty;

        /// <summary>
        /// Gets the enemy party that will be used in the battle.
        /// </summary>
        public IReadOnlyList<PokemonInstance> EnemyParty => _enemyParty;

        /// <summary>
        /// Sets the battle rules. Can use predefined rules like BattleRules.Singles, BattleRules.Doubles, etc.
        /// </summary>
        public BattleBuilder WithRules(BattleRules rules)
        {
            _rules = rules ?? throw new ArgumentNullException(nameof(rules));
            return this;
        }

        /// <summary>
        /// Sets custom battle rules with specified slots.
        /// </summary>
        public BattleBuilder WithRules(int playerSlots, int enemySlots)
        {
            _rules = new BattleRules
            {
                PlayerSlots = playerSlots,
                EnemySlots = enemySlots
            };
            return this;
        }

        /// <summary>
        /// Sets the battle to singles format (1v1).
        /// </summary>
        public BattleBuilder Singles() => WithRules(BattleRules.Singles);

        /// <summary>
        /// Sets the battle to doubles format (2v2).
        /// </summary>
        public BattleBuilder Doubles() => WithRules(BattleRules.Doubles);

        /// <summary>
        /// Sets the battle to triples format (3v3).
        /// </summary>
        public BattleBuilder Triples() => WithRules(BattleRules.Triples);

        /// <summary>
        /// Sets the battle to use full teams (6 Pokemon per side).
        /// The active slots will be determined by the battle format (Singles/Doubles/Triples).
        /// </summary>
        public BattleBuilder FullTeam()
        {
            _useFullTeam = true;
            return this;
        }

        /// <summary>
        /// Fills both parties with random Pokemon based on the current battle format.
        /// - If FullTeam() was called: Creates 6 Pokemon per side
        /// - Otherwise: Creates Pokemon equal to the number of active slots (1 for Singles, 2 for Doubles, 3 for Triples)
        /// </summary>
        /// <param name="level">The level for all Pokemon.</param>
        public BattleBuilder Random(int level)
        {
            return Random(level, null);
        }

        /// <summary>
        /// Fills both parties with random Pokemon based on the current battle format.
        /// Uses the provided seed for reproducible random selection and battle engine.
        /// - If FullTeam() was called: Creates 6 Pokemon per side
        /// - Otherwise: Creates Pokemon equal to the number of active slots (1 for Singles, 2 for Doubles, 3 for Triples)
        /// </summary>
        /// <param name="level">The level for all Pokemon.</param>
        /// <param name="seed">Optional seed for random number generation. If provided, creates a new Random instance and sets it for the battle engine.</param>
        public BattleBuilder Random(int level, int? seed)
        {
            // If seed is provided, create a new Random instance and set it for both Pokemon selection and battle engine
            if (seed.HasValue)
            {
                _random = new Random(seed.Value);
                _randomSeed = seed.Value;
            }

            int playerCount = _useFullTeam ? 6 : _rules.PlayerSlots;
            int enemyCount = _useFullTeam ? 6 : _rules.EnemySlots;

            // Fill player party
            for (int i = 0; i < playerCount; i++)
            {
                var excludedSpecies = _playerParty.Select(p => p.Species).ToList();
                var species = GetRandomSpecies(excludedSpecies);
                _playerParty.Add(PokemonFactory.Create(species, level));
            }

            // Fill enemy party (exclude player Pokemon)
            var enemyExcludedSpecies = _playerParty.Select(p => p.Species).ToList();
            for (int i = 0; i < enemyCount; i++)
            {
                var species = GetRandomSpecies(enemyExcludedSpecies);
                _enemyParty.Add(PokemonFactory.Create(species, level));
                enemyExcludedSpecies.Add(species); // Prevent duplicates in enemy party
            }

            return this;
        }

        /// <summary>
        /// Sets up a 1v1 singles battle with random Pokemon.
        /// </summary>
        /// <param name="level">The level for all Pokemon.</param>
        public BattleBuilder SinglesRandom(int level)
        {
            Singles();
            WithRandomPlayerPokemon(level);
            WithRandomEnemyPokemon(level);
            return this;
        }

        /// <summary>
        /// Sets up a 2v2 doubles battle with random Pokemon.
        /// </summary>
        /// <param name="level">The level for all Pokemon.</param>
        public BattleBuilder DoublesRandom(int level)
        {
            Doubles();
            WithRandomPlayerParty(2, level);
            WithRandomEnemyParty(2, level);
            return this;
        }

        /// <summary>
        /// Sets up a 3v3 triples battle with random Pokemon.
        /// </summary>
        /// <param name="level">The level for all Pokemon.</param>
        public BattleBuilder TriplesRandom(int level)
        {
            Triples();
            WithRandomPlayerParty(3, level);
            WithRandomEnemyParty(3, level);
            return this;
        }

        /// <summary>
        /// Sets up a full team battle (6v6) with random Pokemon.
        /// Uses singles format (1 active Pokemon at a time).
        /// </summary>
        /// <param name="level">The level for all Pokemon.</param>
        public BattleBuilder FullTeamRandom(int level)
        {
            Singles(); // Full team uses singles format (6 Pokemon, 1 active)
            WithRandomPlayerParty(6, level);
            WithRandomEnemyParty(6, level);
            return this;
        }

        /// <summary>
        /// Sets up a full team battle (6v6) with random Pokemon for doubles format.
        /// </summary>
        /// <param name="level">The level for all Pokemon.</param>
        public BattleBuilder FullTeamDoublesRandom(int level)
        {
            Doubles(); // Full team doubles (6 Pokemon, 2 active)
            WithRandomPlayerParty(6, level);
            WithRandomEnemyParty(6, level);
            return this;
        }

        /// <summary>
        /// Sets up a horde battle (1v3) with random Pokemon.
        /// </summary>
        /// <param name="level">The level for all Pokemon.</param>
        public BattleBuilder HordeRandom(int level)
        {
            WithRules(BattleRules.Horde);
            WithRandomPlayerPokemon(level);
            WithRandomEnemyParty(3, level);
            return this;
        }

        /// <summary>
        /// Sets up a horde battle (1v5) with random Pokemon.
        /// </summary>
        /// <param name="level">The level for all Pokemon.</param>
        public BattleBuilder Horde1v5Random(int level)
        {
            WithRules(BattleRules.Horde1v5);
            WithRandomPlayerPokemon(level);
            WithRandomEnemyParty(5, level);
            return this;
        }

        /// <summary>
        /// Sets the action provider for the player.
        /// </summary>
        public BattleBuilder WithPlayerAI(IActionProvider provider)
        {
            _playerProvider = provider ?? throw new ArgumentNullException(nameof(provider));
            return this;
        }

        /// <summary>
        /// Sets the player to use RandomAI.
        /// Note: Requires engine to be created first (via WithEngine) or will be created in Build().
        /// </summary>
        public BattleBuilder WithRandomPlayerAI()
        {
            // Create engine if not already created to get TargetResolver
            if (_engine == null)
                _engine = CombatEngineFactory.Create(_randomSeed);
            return WithPlayerAI(new RandomAI(_engine.TargetResolver));
        }

        /// <summary>
        /// Sets the action provider for the enemy.
        /// </summary>
        public BattleBuilder WithEnemyAI(IActionProvider provider)
        {
            _enemyProvider = provider ?? throw new ArgumentNullException(nameof(provider));
            return this;
        }

        /// <summary>
        /// Sets the enemy to use RandomAI.
        /// Note: Requires engine to be created first (via WithEngine) or will be created in Build().
        /// </summary>
        public BattleBuilder WithRandomEnemyAI()
        {
            // Create engine if not already created to get TargetResolver
            if (_engine == null)
                _engine = CombatEngineFactory.Create(_randomSeed);
            return WithEnemyAI(new RandomAI(_engine.TargetResolver));
        }

        /// <summary>
        /// Sets both player and enemy to use RandomAI.
        /// </summary>
        public BattleBuilder WithRandomAI() => WithRandomPlayerAI().WithRandomEnemyAI();

        /// <summary>
        /// Sets the battle view.
        /// </summary>
        public BattleBuilder WithView(IBattleView view)
        {
            _view = view ?? throw new ArgumentNullException(nameof(view));
            return this;
        }

        /// <summary>
        /// Sets a random seed for reproducible battles.
        /// </summary>
        public BattleBuilder WithRandomSeed(int seed)
        {
            _randomSeed = seed;
            return this;
        }

        /// <summary>
        /// Sets a custom engine instance. If not set, a new engine will be created.
        /// </summary>
        public BattleBuilder WithEngine(CombatEngine engine)
        {
            _engine = engine;
            return this;
        }

        /// <summary>
        /// Enables debug mode for this battle.
        /// When enabled, debug events (step execution, state changes) will be raised.
        /// </summary>
        public BattleBuilder WithDebugMode(bool enabled = true)
        {
            _isDebugMode = enabled;
            return this;
        }

        /// <summary>
        /// Subscribes a handler for the BattleStart event.
        /// </summary>
        public BattleBuilder OnBattleStart(EventHandler<BattleStartEventArgs> handler)
        {
            _onBattleStart = handler;
            return this;
        }

        /// <summary>
        /// Subscribes a handler for the BattleEnd event.
        /// </summary>
        public BattleBuilder OnBattleEnd(EventHandler<BattleEndEventArgs> handler)
        {
            _onBattleEnd = handler;
            return this;
        }

        /// <summary>
        /// Subscribes a handler for the TurnStart event.
        /// </summary>
        public BattleBuilder OnTurnStart(EventHandler<TurnEventArgs> handler)
        {
            _onTurnStart = handler;
            return this;
        }

        /// <summary>
        /// Subscribes a handler for the TurnEnd event.
        /// </summary>
        public BattleBuilder OnTurnEnd(EventHandler<TurnEventArgs> handler)
        {
            _onTurnEnd = handler;
            return this;
        }

        /// <summary>
        /// Subscribes a handler for the ActionExecuted event.
        /// </summary>
        public BattleBuilder OnActionExecuted(EventHandler<ActionExecutedEventArgs> handler)
        {
            _onActionExecuted = handler;
            return this;
        }

        /// <summary>
        /// Subscribes a handler for the StepExecuted event (debug only).
        /// </summary>
        public BattleBuilder OnStepExecuted(EventHandler<StepExecutedEventArgs> handler)
        {
            _onStepExecuted = handler;
            return this;
        }

        /// <summary>
        /// Subscribes a handler for the StepStarted event (debug only).
        /// </summary>
        public BattleBuilder OnStepStarted(EventHandler<StepStartedEventArgs> handler)
        {
            _onStepStarted = handler;
            return this;
        }

        /// <summary>
        /// Subscribes a handler for the StepFinished event (debug only).
        /// </summary>
        public BattleBuilder OnStepFinished(EventHandler<StepFinishedEventArgs> handler)
        {
            _onStepFinished = handler;
            return this;
        }

        /// <summary>
        /// Subscribes a handler for the BattleStateChanged event (debug only).
        /// </summary>
        public BattleBuilder OnBattleStateChanged(EventHandler<BattleStateChangedEventArgs> handler)
        {
            _onBattleStateChanged = handler;
            return this;
        }

        /// <summary>
        /// Subscribes to all production events using the provided handlers.
        /// </summary>
        public BattleBuilder WithProductionEvents(
            EventHandler<BattleStartEventArgs> onBattleStart = null,
            EventHandler<BattleEndEventArgs> onBattleEnd = null,
            EventHandler<TurnEventArgs> onTurnStart = null,
            EventHandler<TurnEventArgs> onTurnEnd = null,
            EventHandler<ActionExecutedEventArgs> onActionExecuted = null)
        {
            if (onBattleStart != null) _onBattleStart = onBattleStart;
            if (onBattleEnd != null) _onBattleEnd = onBattleEnd;
            if (onTurnStart != null) _onTurnStart = onTurnStart;
            if (onTurnEnd != null) _onTurnEnd = onTurnEnd;
            if (onActionExecuted != null) _onActionExecuted = onActionExecuted;
            return this;
        }

        /// <summary>
        /// Subscribes to all debug events using the provided handlers.
        /// These events will only fire if debug mode is enabled.
        /// </summary>
        public BattleBuilder WithDebugEvents(
            EventHandler<StepExecutedEventArgs> onStepExecuted = null,
            EventHandler<StepStartedEventArgs> onStepStarted = null,
            EventHandler<StepFinishedEventArgs> onStepFinished = null,
            EventHandler<BattleStateChangedEventArgs> onBattleStateChanged = null)
        {
            if (onStepExecuted != null) _onStepExecuted = onStepExecuted;
            if (onStepStarted != null) _onStepStarted = onStepStarted;
            if (onStepFinished != null) _onStepFinished = onStepFinished;
            if (onBattleStateChanged != null) _onBattleStateChanged = onBattleStateChanged;
            return this;
        }

        /// <summary>
        /// Enables automatic statistics collection for this battle.
        /// The statistics collector will be automatically registered as a battle feature.
        /// The statistics collector can be accessed via StatisticsCollector property after Build().
        /// </summary>
        /// <param name="collector">Optional existing collector. If null, creates a new one.</param>
        /// <returns>The builder instance for method chaining.</returns>
        public BattleBuilder WithStatistics(BattleStatisticsCollector collector = null)
        {
            _statisticsCollector = collector ?? new BattleStatisticsCollector();
            // Note: The collector will be registered as a feature in Build(), not subscribed here
            // This keeps the integration clean and automatic
            return this;
        }

        /// <summary>
        /// Gets the statistics collector if statistics were enabled.
        /// </summary>
        public BattleStatisticsCollector StatisticsCollector => _statisticsCollector;


        /// <summary>
        /// Builds and initializes the CombatEngine with the configured settings.
        /// </summary>
        /// <returns>The initialized CombatEngine.</returns>
        /// <exception cref="InvalidOperationException">If required configuration is missing.</exception>
        public CombatEngine Build()
        {
            // Validate required fields
            if (_playerParty.Count == 0)
                throw new InvalidOperationException("Player party cannot be empty. Add at least one Pokemon using WithPlayerPokemon() or WithPlayerParty().");
            if (_enemyParty.Count == 0)
                throw new InvalidOperationException("Enemy party cannot be empty. Add at least one Pokemon using WithEnemyPokemon() or WithEnemyParty().");

            // Create engine first if not provided (needed for TargetResolver)
            if (_engine == null)
                _engine = CombatEngineFactory.Create(_randomSeed);

            // Set default AI providers if not specified (use shared TargetResolver from engine)
            if (_playerProvider == null)
                _playerProvider = new RandomAI(_engine.TargetResolver);
            if (_enemyProvider == null)
                _enemyProvider = new RandomAI(_engine.TargetResolver);

            // Subscribe to events if handlers were provided
            if (_onBattleStart != null)
                BattleEventManager.BattleStart += _onBattleStart;
            if (_onBattleEnd != null)
                BattleEventManager.BattleEnd += _onBattleEnd;
            if (_onTurnStart != null)
                BattleEventManager.TurnStart += _onTurnStart;
            if (_onTurnEnd != null)
                BattleEventManager.TurnEnd += _onTurnEnd;
            if (_onActionExecuted != null)
                BattleEventManager.ActionExecuted += _onActionExecuted;
            if (_onStepExecuted != null)
                BattleEventManager.StepExecuted += _onStepExecuted;
            if (_onStepStarted != null)
                BattleEventManager.StepStarted += _onStepStarted;
            if (_onStepFinished != null)
                BattleEventManager.StepFinished += _onStepFinished;
            if (_onBattleStateChanged != null)
                BattleEventManager.BattleStateChanged += _onBattleStateChanged;

            // Initialize the engine with debug mode
            _engine.Initialize(
                _rules,
                _playerParty,
                _enemyParty,
                _playerProvider,
                _enemyProvider,
                _view,
                _isDebugMode);

            // Subscribe statistics collector to events if enabled
            if (_statisticsCollector != null)
            {
                _statisticsCollector.Subscribe();
                // Register collector with engine for automatic cleanup on dispose
                _engine.RegisterStatisticsCollector(_statisticsCollector);
            }

            return _engine;
        }

        /// <summary>
        /// Builds and initializes the CombatEngine, then runs the battle to completion.
        /// </summary>
        /// <returns>The battle result.</returns>
        public async Task<BattleResult> BuildAndRunAsync()
        {
            var engine = Build();
            return await engine.RunBattle();
        }

        /// <summary>
        /// Gets a random Pokemon species from the catalog, excluding specified species.
        /// </summary>
        private PokemonSpeciesData GetRandomSpecies(PokemonSpeciesData excludeSpecies)
        {
            return GetRandomSpecies(excludeSpecies != null ? new[] { excludeSpecies } : null);
        }

        /// <summary>
        /// Gets a random Pokemon species from the catalog, excluding specified species.
        /// </summary>
        private PokemonSpeciesData GetRandomSpecies(IEnumerable<PokemonSpeciesData> excludeSpecies = null)
        {
            var availablePokemon = PokemonCatalog.All.ToList();

            if (excludeSpecies != null)
            {
                var excludeSet = excludeSpecies.ToHashSet();
                availablePokemon = availablePokemon.Where(p => !excludeSet.Contains(p)).ToList();
            }

            if (availablePokemon.Count == 0)
                throw new InvalidOperationException("No Pokemon available in catalog (after exclusions).");

            return availablePokemon[_random.Next(availablePokemon.Count)];
        }
    }
}
