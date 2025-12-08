using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PokemonUltimate.Combat.Actions;
using PokemonUltimate.Combat.Damage;
using PokemonUltimate.Combat.Damage.Definition;
using PokemonUltimate.Combat.Engine;
using PokemonUltimate.Combat.Engine.BattleFlow;
using PokemonUltimate.Combat.Engine.BattleFlow.Steps;
using PokemonUltimate.Combat.Engine.Validation;
using PokemonUltimate.Combat.Engine.Validation.Definition;
using PokemonUltimate.Combat.Field;
using PokemonUltimate.Combat.Handlers.Registry;
using PokemonUltimate.Combat.Infrastructure.Builders;
using PokemonUltimate.Combat.Infrastructure.Constants;
using PokemonUltimate.Combat.Infrastructure.Events;
using PokemonUltimate.Combat.Infrastructure.Factories;
using PokemonUltimate.Combat.Infrastructure.Factories.Definition;
using PokemonUltimate.Combat.Infrastructure.Logging;
using PokemonUltimate.Combat.Infrastructure.Logging.Definition;
using PokemonUltimate.Combat.Infrastructure.Providers;
using PokemonUltimate.Combat.Infrastructure.Providers.Definition;
using PokemonUltimate.Combat.Infrastructure.Statistics;
using PokemonUltimate.Combat.Utilities;
using PokemonUltimate.Combat.Utilities.Extensions;
using PokemonUltimate.Combat.View;
using PokemonUltimate.Core.Domain.Instances;
using PokemonUltimate.Core.Domain.Instances.Move;
using PokemonUltimate.Core.Domain.Instances.Pokemon;
using PokemonUltimate.Core.Infrastructure.Factories;

namespace PokemonUltimate.Combat.Infrastructure.Simulation
{
    /// <summary>
    /// Simulates single move executions without full battle simulation.
    /// Useful for testing move effects, damage calculations, and move statistics.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.6: Combat Engine
    /// **Documentation**: See `docs/features/2-combat-system/2.6-combat-engine/architecture.md`
    /// </remarks>
    public class MoveSimulator
    {
        /// <summary>
        /// Configuration for move simulation.
        /// </summary>
        public class MoveSimulationConfig
        {
            /// <summary>
            /// Number of move executions to simulate.
            /// </summary>
            public int NumberOfTests { get; set; } = 100;

            /// <summary>
            /// Whether to use different random seeds for each test.
            /// If false, all tests use the same seed (deterministic).
            /// </summary>
            public bool UseRandomSeeds { get; set; } = true;

            /// <summary>
            /// Optional seed for reproducible simulations.
            /// If set and UseRandomSeeds is false, all tests use this seed.
            /// </summary>
            public int? FixedSeed { get; set; }

            /// <summary>
            /// Whether to reset statistics between tests.
            /// If false, statistics accumulate across all tests.
            /// </summary>
            public bool ResetStatisticsBetweenTests { get; set; } = false;

            /// <summary>
            /// Whether to collect individual test results.
            /// If false, only aggregated statistics are collected.
            /// </summary>
            public bool CollectIndividualResults { get; set; } = false;
        }

        /// <summary>
        /// Results from a batch of move simulations.
        /// </summary>
        public class MoveSimulationResults
        {
            /// <summary>
            /// Total number of move executions simulated.
            /// </summary>
            public int TotalTests { get; set; }

            /// <summary>
            /// Aggregated statistics from all move executions.
            /// </summary>
            public BattleStatistics AggregatedStatistics { get; set; }

            /// <summary>
            /// Individual test statistics (if CollectIndividualResults is true).
            /// </summary>
            public List<BattleStatistics> IndividualResults { get; set; } = new List<BattleStatistics>();
        }

        /// <summary>
        /// Simulates multiple move executions and aggregates statistics.
        /// </summary>
        /// <param name="moveToTest">The move to test. Cannot be null.</param>
        /// <param name="attacker">The Pokemon using the move. Cannot be null.</param>
        /// <param name="target">The target Pokemon. Cannot be null.</param>
        /// <param name="config">Simulation configuration. If null, uses defaults.</param>
        /// <param name="progress">Optional progress reporter (0-100).</param>
        /// <returns>Results containing aggregated statistics from all move executions.</returns>
        public static async Task<MoveSimulationResults> SimulateAsync(
            MoveInstance moveToTest,
            PokemonInstance attacker,
            PokemonInstance target,
            MoveSimulationConfig config = null,
            IProgress<int> progress = null)
        {
            if (moveToTest == null)
                throw new ArgumentNullException(nameof(moveToTest));
            if (attacker == null)
                throw new ArgumentNullException(nameof(attacker));
            if (target == null)
                throw new ArgumentNullException(nameof(target));

            config = config ?? new MoveSimulationConfig();

            if (config.NumberOfTests <= 0)
                throw new ArgumentException("NumberOfTests must be greater than 0", nameof(config));

            var results = new MoveSimulationResults
            {
                TotalTests = config.NumberOfTests
            };

            // Create a shared statistics collector for aggregation across all tests
            // This collector will accumulate statistics from all tests
            var aggregatedCollector = new BattleStatisticsCollector();

            // Random for generating seeds if needed
            var random = config.FixedSeed.HasValue
                ? new Random(config.FixedSeed.Value)
                : new Random();

            for (int i = 0; i < config.NumberOfTests; i++)
            {
                // Create fresh Pokemon instances for each test
                var freshAttacker = PokemonFactory.Create(attacker.Species, attacker.Level);
                var freshTarget = PokemonFactory.Create(target.Species, target.Level);

                // Add the move to the attacker
                var moveInstance = new MoveInstance(moveToTest.Move);
                if (!freshAttacker.Moves.Contains(moveInstance))
                {
                    freshAttacker.Moves.Add(moveInstance);
                }

                // Create AI providers (will get TargetResolver from engine after creation)
                var fixedMoveAI = new FixedMoveActionProvider(moveInstance);
                var noActionAI = new NoActionProvider();

                // Create a per-test statistics collector (similar to BattleSimulator pattern)
                var testCollector = new BattleStatisticsCollector();
                testCollector.AutoResetOnBattleStart = false; // Don't reset on battle start

                // Create engine dependencies (reuse AccuracyChecker and DamagePipeline if possible)
                var seed = config.UseRandomSeeds
                    ? (config.FixedSeed ?? random.Next())
                    : (config.FixedSeed ?? 0);

                var randomProvider = new RandomProvider(seed);
                var battleFieldFactory = new BattleFieldFactory();
                var battleQueueFactory = new BattleQueueFactory();

                // Reuse AccuracyChecker and DamagePipeline if seed allows (for same random provider)
                // Note: We create new ones per iteration to ensure different random seeds work correctly
                var accuracyChecker = new AccuracyChecker(randomProvider);
                var damagePipeline = new DamagePipeline(randomProvider);
                var handlerRegistry = CombatEffectHandlerRegistry.CreateDefault();
                var stateValidator = new BattleStateValidator();
                var logger = new BattleLogger("MoveSimulator");

                var engine = new CombatEngine(
                    battleFieldFactory,
                    battleQueueFactory,
                    randomProvider,
                    accuracyChecker,
                    damagePipeline,
                    handlerRegistry,
                    stateValidator,
                    logger);

                // Update FixedMoveActionProvider with shared TargetResolver from engine
                fixedMoveAI.SetTargetResolver(engine.TargetResolver);

                // Subscribe per-test statistics collector to events
                testCollector.Subscribe();
                // Register collector with engine for automatic cleanup on dispose
                engine.RegisterStatisticsCollector(testCollector);

                // Initialize the engine
                var rules = new BattleRules { PlayerSlots = 1, EnemySlots = 1 };
                engine.Initialize(rules, new[] { freshAttacker }, new[] { freshTarget }, fixedMoveAI, noActionAI, NullBattleView.Instance);

                // Execute single turn manually (use ConfigureAwait(false) to avoid capturing context)
                // Note: accuracyChecker and damagePipeline are already created above and passed to engine
                await ExecuteSingleTurn(engine, rules, freshAttacker, freshTarget, fixedMoveAI, noActionAI,
                    randomProvider, battleFieldFactory, battleQueueFactory, accuracyChecker, damagePipeline,
                    handlerRegistry, stateValidator, logger).ConfigureAwait(false);

                // Collect individual test statistics if requested
                if (config.CollectIndividualResults)
                {
                    var testStats = testCollector.GetStatistics();
                    if (testStats != null)
                    {
                        results.IndividualResults.Add(testStats);
                    }
                }

                // Aggregate statistics from this test into the aggregated collector
                var testStatsForAggregation = testCollector.GetStatistics();
                if (testStatsForAggregation != null)
                {
                    AggregateStatistics(aggregatedCollector.GetStatistics(), testStatsForAggregation);
                }

                // Reset per-test statistics collector for next test if requested
                // Note: We don't reset the aggregated collector - it accumulates across all tests
                if (config.ResetStatisticsBetweenTests)
                {
                    testCollector.Reset();
                }

                // IMPORTANT: Dispose engine to clean up event subscriptions and prevent memory leaks
                // This ensures event handlers are unsubscribed and resources are freed
                engine.Dispose();

                // Report progress
                if (progress != null)
                {
                    var percentComplete = (int)((i + 1) * 100.0 / config.NumberOfTests);
                    progress.Report(percentComplete);
                }
            }

            results.AggregatedStatistics = aggregatedCollector.GetStatistics();

            return results;
        }

        /// <summary>
        /// Executes a single turn with the given configuration.
        /// This initializes the battle flow context and executes only the first turn.
        /// </summary>
        private static async Task ExecuteSingleTurn(
            CombatEngine engine,
            BattleRules rules,
            PokemonInstance attacker,
            PokemonInstance target,
            IActionProvider playerProvider,
            IActionProvider enemyProvider,
            IRandomProvider randomProvider,
            IBattleFieldFactory battleFieldFactory,
            IBattleQueueFactory battleQueueFactory,
            AccuracyChecker accuracyChecker,
            IDamagePipeline damagePipeline,
            CombatEffectHandlerRegistry handlerRegistry,
            IBattleStateValidator stateValidator,
            IBattleLogger logger)
        {
            // Create battle flow context
            var context = new BattleFlowContext
            {
                Rules = rules,
                PlayerParty = new[] { attacker },
                EnemyParty = new[] { target },
                PlayerProvider = playerProvider,
                EnemyProvider = enemyProvider,
                View = NullBattleView.Instance,
                StateValidator = stateValidator,
                Logger = logger,
                Engine = engine,
                TargetResolver = engine.TargetResolver, // Set TargetResolver before CreateDependenciesStep
                IsDebugMode = false
            };

            // Execute setup steps manually to get TurnEngine ready (use ConfigureAwait(false))
            var createFieldStep = new CreateFieldStep(battleFieldFactory);
            await createFieldStep.ExecuteAsync(context).ConfigureAwait(false);

            var assignProvidersStep = new AssignActionProvidersStep();
            await assignProvidersStep.ExecuteAsync(context).ConfigureAwait(false);

            var createQueueStep = new CreateQueueStep(battleQueueFactory);
            await createQueueStep.ExecuteAsync(context).ConfigureAwait(false);

            var validateStep = new ValidateInitialStateStep();
            await validateStep.ExecuteAsync(context).ConfigureAwait(false);

            var createDependenciesStep = new CreateDependenciesStep(randomProvider, damagePipeline, handlerRegistry, accuracyChecker);
            await createDependenciesStep.ExecuteAsync(context).ConfigureAwait(false);

            var battleStartStep = new BattleStartFlowStep();
            await battleStartStep.ExecuteAsync(context).ConfigureAwait(false);

            // Execute only the first turn
            if (context.TurnEngine != null)
            {
                // Raise turn start event
                BattleEventManager.RaiseTurnStart(1, context.Field);

                // Execute the turn (use ConfigureAwait(false))
                await context.TurnEngine.ExecuteTurn(context.Field, 1).ConfigureAwait(false);

                // Raise turn end event
                BattleEventManager.RaiseTurnEnd(1, context.Field);
            }
            else
            {
                // Fallback: if TurnEngine is not available, run full battle
                // This shouldn't happen, but provides a safety net
                await engine.RunBattle().ConfigureAwait(false);
            }
        }

        /// <summary>
        /// AI provider that always selects a specific move.
        /// Used internally by MoveSimulator for testing specific moves.
        /// </summary>
        private class FixedMoveActionProvider : ActionProviderBase
        {
            private readonly MoveInstance _moveToUse;
            private TargetResolver _targetResolver;

            public FixedMoveActionProvider(MoveInstance moveToUse)
            {
                _moveToUse = moveToUse ?? throw new ArgumentNullException(nameof(moveToUse));
            }

            /// <summary>
            /// Sets the TargetResolver to use (called after engine is created).
            /// </summary>
            public void SetTargetResolver(TargetResolver targetResolver)
            {
                _targetResolver = targetResolver ?? throw new ArgumentNullException(nameof(targetResolver));
            }

            public override Task<BattleAction> GetAction(BattleField field, BattleSlot mySlot)
            {
                if (field == null)
                    throw new ArgumentNullException(nameof(field));
                if (mySlot == null)
                    throw new ArgumentNullException(nameof(mySlot));

                // Return null if slot is empty or Pokemon is fainted
                if (!mySlot.IsActive())
                    return Task.FromResult<BattleAction>(null);

                // Check if the Pokemon has this move and it has PP
                var pokemonMove = mySlot.Pokemon.Moves.FirstOrDefault(m =>
                    m.Move.Id == _moveToUse.Move.Id && m.HasPP);

                if (pokemonMove == null)
                {
                    // Move not available (not learned or no PP)
                    return Task.FromResult<BattleAction>(null);
                }

                // Get basic valid targets (without redirections - those are applied by TargetResolutionStep)
                if (_targetResolver == null)
                    throw new InvalidOperationException("TargetResolver must be set before using FixedMoveActionProvider.");
                var validTargets = _targetResolver.GetBasicTargets(mySlot, pokemonMove.Move, field);

                if (validTargets.Count == 0)
                {
                    // No valid targets (e.g., Field move or all targets fainted)
                    return Task.FromResult<BattleAction>(null);
                }

                // Pick the first valid target (or self if only one target and it's self)
                BattleSlot target;
                if (validTargets.Count == 1)
                {
                    target = validTargets[0];
                }
                else
                {
                    // For multi-target moves, pick the first enemy target, or first target if no enemy
                    target = validTargets.FirstOrDefault(t =>
                        field.EnemySide.Slots.Contains(t)) ?? validTargets[0];
                }

                // Return UseMoveAction with the fixed move
                return Task.FromResult<BattleAction>(new UseMoveAction(mySlot, target, pokemonMove));
            }
        }

        /// <summary>
        /// AI provider that never provides an action (always returns null).
        /// Used internally by MoveSimulator for testing scenarios where we want only one side to act.
        /// </summary>
        private class NoActionProvider : ActionProviderBase
        {
            public override Task<BattleAction> GetAction(BattleField field, BattleSlot mySlot)
            {
                // Always return null - this Pokemon will skip its turn
                return Task.FromResult<BattleAction>(null);
            }
        }

        /// <summary>
        /// Aggregates statistics from a single test into aggregated statistics.
        /// Uses the same logic as BattleSimulator for consistency.
        /// </summary>
        private static void AggregateStatistics(BattleStatistics aggregated, BattleStatistics test)
        {
            // Aggregate totals
            aggregated.TotalTurns += test.TotalTurns;
            aggregated.TotalActions += test.TotalActions;

            // Aggregate actions by type
            foreach (var actionType in test.ActionsByType)
            {
                if (!aggregated.ActionsByType.ContainsKey(actionType.Key))
                    aggregated.ActionsByType[actionType.Key] = 0;
                aggregated.ActionsByType[actionType.Key] += actionType.Value;
            }

            // Aggregate damage and healing
            aggregated.PlayerDamageDealt += test.PlayerDamageDealt;
            aggregated.EnemyDamageDealt += test.EnemyDamageDealt;
            aggregated.PlayerHealing += test.PlayerHealing;
            aggregated.EnemyHealing += test.EnemyHealing;

            // Aggregate move usage
            foreach (var move in test.PlayerMoveUsage)
            {
                if (!aggregated.PlayerMoveUsage.ContainsKey(move.Key))
                    aggregated.PlayerMoveUsage[move.Key] = 0;
                aggregated.PlayerMoveUsage[move.Key] += move.Value;
            }
            foreach (var move in test.EnemyMoveUsage)
            {
                if (!aggregated.EnemyMoveUsage.ContainsKey(move.Key))
                    aggregated.EnemyMoveUsage[move.Key] = 0;
                aggregated.EnemyMoveUsage[move.Key] += move.Value;
            }

            // Aggregate move usage by Pokemon
            foreach (var pokemon in test.MoveUsageByPokemon)
            {
                if (!aggregated.MoveUsageByPokemon.ContainsKey(pokemon.Key))
                    aggregated.MoveUsageByPokemon[pokemon.Key] = new Dictionary<string, int>();

                foreach (var move in pokemon.Value)
                {
                    if (!aggregated.MoveUsageByPokemon[pokemon.Key].ContainsKey(move.Key))
                        aggregated.MoveUsageByPokemon[pokemon.Key][move.Key] = 0;
                    aggregated.MoveUsageByPokemon[pokemon.Key][move.Key] += move.Value;
                }
            }

            // Aggregate damage by move
            foreach (var move in test.DamageByMove)
            {
                if (!aggregated.DamageByMove.ContainsKey(move.Key))
                    aggregated.DamageByMove[move.Key] = 0;
                aggregated.DamageByMove[move.Key] += move.Value;
            }

            // Aggregate individual damage values by move
            foreach (var move in test.DamageValuesByMove)
            {
                if (!aggregated.DamageValuesByMove.ContainsKey(move.Key))
                    aggregated.DamageValuesByMove[move.Key] = new List<int>();
                aggregated.DamageValuesByMove[move.Key].AddRange(move.Value);
            }

            // Aggregate critical hits and missed moves
            aggregated.CriticalHits += test.CriticalHits;
            aggregated.MissedMoves += test.MissedMoves;

            // Aggregate fainted Pokemon (lists)
            aggregated.PlayerFainted.AddRange(test.PlayerFainted);
            aggregated.EnemyFainted.AddRange(test.EnemyFainted);

            // Aggregate switches
            foreach (var pokemon in test.PokemonSwitches)
            {
                if (!aggregated.PokemonSwitches.ContainsKey(pokemon.Key))
                    aggregated.PokemonSwitches[pokemon.Key] = 0;
                aggregated.PokemonSwitches[pokemon.Key] += pokemon.Value;
            }

            // Aggregate status effects
            foreach (var status in test.StatusEffectsApplied)
            {
                if (!aggregated.StatusEffectsApplied.ContainsKey(status.Key))
                    aggregated.StatusEffectsApplied[status.Key] = 0;
                aggregated.StatusEffectsApplied[status.Key] += status.Value;
            }

            // Aggregate stat changes
            foreach (var pokemon in test.StatChanges)
            {
                if (!aggregated.StatChanges.ContainsKey(pokemon.Key))
                    aggregated.StatChanges[pokemon.Key] = new Dictionary<string, int>();

                foreach (var stat in pokemon.Value)
                {
                    if (!aggregated.StatChanges[pokemon.Key].ContainsKey(stat.Key))
                        aggregated.StatChanges[pokemon.Key][stat.Key] = 0;
                    aggregated.StatChanges[pokemon.Key][stat.Key] += stat.Value;
                }
            }

            // Aggregate weather and terrain changes (lists)
            aggregated.WeatherChanges.AddRange(test.WeatherChanges);
            aggregated.TerrainChanges.AddRange(test.TerrainChanges);

            // Aggregate actions by Pokemon
            foreach (var pokemon in test.ActionsByPokemon)
            {
                if (!aggregated.ActionsByPokemon.ContainsKey(pokemon.Key))
                    aggregated.ActionsByPokemon[pokemon.Key] = new Dictionary<string, int>();

                foreach (var action in pokemon.Value)
                {
                    if (!aggregated.ActionsByPokemon[pokemon.Key].ContainsKey(action.Key))
                        aggregated.ActionsByPokemon[pokemon.Key][action.Key] = 0;
                    aggregated.ActionsByPokemon[pokemon.Key][action.Key] += action.Value;
                }
            }

            // Aggregate actions by team
            foreach (var action in test.PlayerActionsByType)
            {
                if (!aggregated.PlayerActionsByType.ContainsKey(action.Key))
                    aggregated.PlayerActionsByType[action.Key] = 0;
                aggregated.PlayerActionsByType[action.Key] += action.Value;
            }
            foreach (var action in test.EnemyActionsByType)
            {
                if (!aggregated.EnemyActionsByType.ContainsKey(action.Key))
                    aggregated.EnemyActionsByType[action.Key] = 0;
                aggregated.EnemyActionsByType[action.Key] += action.Value;
            }
        }
    }
}
