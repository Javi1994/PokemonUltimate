using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PokemonUltimate.Combat.Infrastructure.Simulation;
using PokemonUltimate.Combat.Infrastructure.Statistics;
using PokemonUltimate.Content.Catalogs.Moves;
using PokemonUltimate.Content.Catalogs.Pokemon;
using PokemonUltimate.Core.Data.Blueprints;
using PokemonUltimate.Core.Domain.Instances;
using PokemonUltimate.Core.Domain.Instances.Move;
using PokemonUltimate.Core.Infrastructure.Factories;
using InfrastructureStats = PokemonUltimate.Combat.Infrastructure.Statistics.BattleStatistics;

namespace PokemonUltimate.DeveloperTools.Runners
{
    /// <summary>
    /// Move runner using the new MoveSimulator system.
    /// Tests individual moves without full battle simulation.
    /// </summary>
    /// <remarks>
    /// **Feature**: 6: Development Tools
    /// **Sub-Feature**: 6.6: Move Debugger
    /// **Documentation**: See `docs/features/6-development-tools/6.6-move-debugger/README.md`
    /// </remarks>
    public class MoveRunner
    {
        /// <summary>
        /// Move test statistics wrapper for compatibility with existing UI code.
        /// Uses the new Statistics System internally.
        /// </summary>
        public class MoveStatistics
        {
            private readonly InfrastructureStats _internalStats;
            private readonly double _totalSimulationTimeSeconds;

            public MoveStatistics(InfrastructureStats internalStats, double totalSimulationTimeSeconds = 0)
            {
                _internalStats = internalStats ?? throw new ArgumentNullException(nameof(internalStats));
                _totalSimulationTimeSeconds = totalSimulationTimeSeconds;
            }

            /// <summary>
            /// Total simulation time in seconds.
            /// </summary>
            public double TotalSimulationTimeSeconds => _totalSimulationTimeSeconds;

            public List<int> DamageValues
            {
                get
                {
                    // Extract individual damage values from DamageValuesByMove dictionary
                    // This gives us the actual damage values for each hit, not just totals
                    var allDamage = new List<int>();
                    foreach (var moveDamageList in _internalStats.DamageValuesByMove.Values)
                    {
                        allDamage.AddRange(moveDamageList);
                    }
                    return allDamage;
                }
            }

            public int CriticalHits => _internalStats.CriticalHits;
            public int Misses => _internalStats.MissedMoves;

            public Dictionary<string, int> StatusEffectsCaused
            {
                get
                {
                    // The new system tracks StatusEffectsApplied as (Effect name -> count)
                    return new Dictionary<string, int>(_internalStats.StatusEffectsApplied);
                }
            }

            public Dictionary<string, int> VolatileStatusEffectsCaused
            {
                get
                {
                    // The new system doesn't track volatile status effects separately
                    // Return empty dictionary for now
                    return new Dictionary<string, int>();
                }
            }

            public Dictionary<string, int> ActionsGenerated => _internalStats.ActionsByType;
        }

        /// <summary>
        /// Configuration for move testing.
        /// </summary>
        public class MoveConfig
        {
            /// <summary>
            /// The move to test. Cannot be null.
            /// </summary>
            public MoveData MoveToTest { get; set; } = null!;

            /// <summary>
            /// The Pokemon using the move. Cannot be null.
            /// </summary>
            public PokemonSpeciesData AttackerPokemon { get; set; } = null!;

            /// <summary>
            /// The target Pokemon. Cannot be null.
            /// </summary>
            public PokemonSpeciesData TargetPokemon { get; set; } = null!;

            /// <summary>
            /// Level for both Pokemon. Defaults to 50.
            /// </summary>
            public int Level { get; set; } = 50;

            /// <summary>
            /// Number of move executions to test. Defaults to 100.
            /// </summary>
            public int NumberOfTests { get; set; } = 100;

            /// <summary>
            /// Whether to show detailed output. Currently unused but kept for future use.
            /// </summary>
            public bool DetailedOutput { get; set; } = false;
        }

        /// <summary>
        /// Runs move tests using MoveSimulator.
        /// </summary>
        /// <param name="config">Move test configuration. Cannot be null.</param>
        /// <param name="progress">Optional progress reporter (0-100).</param>
        /// <returns>Move statistics from all tests.</returns>
        /// <exception cref="ArgumentNullException">If config is null.</exception>
        public async Task<MoveStatistics> RunTestsAsync(MoveConfig config, IProgress<int>? progress = null)
        {
            if (config == null)
                throw new ArgumentNullException(nameof(config));
            if (config.MoveToTest == null)
                throw new ArgumentException("MoveToTest cannot be null", nameof(config));
            if (config.AttackerPokemon == null)
                throw new ArgumentException("AttackerPokemon cannot be null", nameof(config));
            if (config.TargetPokemon == null)
                throw new ArgumentException("TargetPokemon cannot be null", nameof(config));
            if (config.NumberOfTests <= 0)
                throw new ArgumentException("NumberOfTests must be greater than 0", nameof(config));
            if (config.Level <= 0 || config.Level > 100)
                throw new ArgumentException("Level must be between 1 and 100", nameof(config));

            // Create initial Pokemon instances (MoveSimulator will create fresh ones for each test)
            var attacker = PokemonFactory.Create(config.AttackerPokemon, config.Level);
            var target = PokemonFactory.Create(config.TargetPokemon, config.Level);
            var moveInstance = new MoveInstance(config.MoveToTest);

            // Configure simulation
            var simulationConfig = new MoveSimulator.MoveSimulationConfig
            {
                NumberOfTests = config.NumberOfTests,
                UseRandomSeeds = true, // Each test uses different random seed
                ResetStatisticsBetweenTests = false // Accumulate statistics across all tests
            };

            // Measure total simulation time
            var simulationStartTime = DateTime.Now;

            // Run simulation using MoveSimulator
            var results = await MoveSimulator.SimulateAsync(
                moveInstance,
                attacker,
                target,
                simulationConfig,
                progress);

            var simulationEndTime = DateTime.Now;
            var totalSimulationTime = (simulationEndTime - simulationStartTime).TotalSeconds;

            // Wrap results in compatibility wrapper (use aggregated statistics)
            var stats = new MoveStatistics(results.AggregatedStatistics, totalSimulationTime);

            return stats;
        }
    }
}

