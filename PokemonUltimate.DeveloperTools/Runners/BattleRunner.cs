using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PokemonUltimate.Combat.Infrastructure.Builders;
using PokemonUltimate.Combat.Infrastructure.Simulation;
using PokemonUltimate.Combat.Infrastructure.Statistics;
using PokemonUltimate.Content.Catalogs.Pokemon;
using PokemonUltimate.Core.Data.Blueprints;
using PokemonUltimate.Core.Infrastructure.Factories;
using InfrastructureStats = PokemonUltimate.Combat.Infrastructure.Statistics.BattleStatistics;

namespace PokemonUltimate.DeveloperTools.Runners
{
    /// <summary>
    /// Battle runner using the new BattleSimulator and BattleBuilder system.
    /// </summary>
    /// <remarks>
    /// **Feature**: 6: Development Tools
    /// **Sub-Feature**: 6.5: Battle Debugger
    /// **Documentation**: See `docs/features/6-development-tools/6.5-battle-debugger/README.md`
    /// </remarks>
    public class BattleRunner
    {
        private readonly Random _random = new Random();
        private readonly List<PokemonSpeciesData> _availablePokemon = PokemonCatalog.All.ToList();

        /// <summary>
        /// Battle statistics wrapper for compatibility with existing UI code.
        /// Adapts the new Statistics System to the format expected by BattleDebuggerTab.
        /// </summary>
        public class BattleStatistics
        {
            private readonly BattleSimulator.SimulationResults _simulationResults;
            private readonly InfrastructureStats _aggregatedStats;
            private readonly double _totalSimulationTimeSeconds;

            public BattleStatistics(BattleSimulator.SimulationResults simulationResults, double totalSimulationTimeSeconds = 0)
            {
                _simulationResults = simulationResults ?? throw new ArgumentNullException(nameof(simulationResults));
                _aggregatedStats = simulationResults.AggregatedStatistics ?? new InfrastructureStats();
                _totalSimulationTimeSeconds = totalSimulationTimeSeconds;
            }

            /// <summary>
            /// Total simulation time in seconds.
            /// </summary>
            public double TotalSimulationTimeSeconds => _totalSimulationTimeSeconds;

            public int PlayerWins => _simulationResults.PlayerWins;
            public int EnemyWins => _simulationResults.EnemyWins;
            public int Draws => _simulationResults.Draws;

            /// <summary>
            /// Move usage statistics by Pokemon (Pokemon name -> Move name -> count).
            /// Maps from MoveUsageByPokemon in the new statistics system.
            /// </summary>
            public Dictionary<string, Dictionary<string, int>> MoveUsageStats => _aggregatedStats.MoveUsageByPokemon;

            /// <summary>
            /// Moves used by player Pokemon (move name -> count).
            /// </summary>
            public Dictionary<string, int> PlayerMoveUsage => _aggregatedStats.PlayerMoveUsage;

            /// <summary>
            /// Moves used by enemy Pokemon (move name -> count).
            /// </summary>
            public Dictionary<string, int> EnemyMoveUsage => _aggregatedStats.EnemyMoveUsage;

            /// <summary>
            /// Total number of turns across all battles.
            /// </summary>
            public int TotalTurns => _aggregatedStats.TotalTurns;

            /// <summary>
            /// Average number of turns per battle.
            /// </summary>
            public double AverageTurnsPerBattle
            {
                get
                {
                    var totalBattles = PlayerWins + EnemyWins + Draws;
                    return totalBattles > 0 ? (double)TotalTurns / totalBattles : 0;
                }
            }

            /// <summary>
            /// Status effect statistics by Pokemon.
            /// Since the new system tracks StatusEffectsApplied as (Effect name -> count) without Pokemon tracking,
            /// we create a simplified mapping that groups all status effects under "All" Pokemon.
            /// This maintains compatibility with the UI while using the new statistics system.
            /// </summary>
            public Dictionary<string, Dictionary<string, int>> StatusEffectStats
            {
                get
                {
                    // The new system doesn't track status effects by Pokemon directly in StatusEffectsApplied
                    // We'll create a simplified mapping: use "All" as the Pokemon key
                    // This maintains compatibility with the UI while using the new statistics system
                    var result = new Dictionary<string, Dictionary<string, int>>();

                    if (_aggregatedStats.StatusEffectsApplied.Count > 0)
                    {
                        result["All"] = new Dictionary<string, int>(_aggregatedStats.StatusEffectsApplied);
                    }

                    return result;
                }
            }
        }

        /// <summary>
        /// Configuration for battle simulation.
        /// </summary>
        public class BattleConfig
        {
            /// <summary>
            /// Player's Pokemon species. If null, a random Pokemon is selected.
            /// </summary>
            public PokemonSpeciesData? PlayerPokemon { get; set; }

            /// <summary>
            /// Enemy's Pokemon species. If null, a random Pokemon is selected.
            /// </summary>
            public PokemonSpeciesData? EnemyPokemon { get; set; }

            /// <summary>
            /// Level for all Pokemon. Defaults to 50.
            /// </summary>
            public int Level { get; set; } = 50;

            /// <summary>
            /// Number of battles to simulate. Defaults to 100.
            /// </summary>
            public int NumberOfBattles { get; set; } = 100;

            /// <summary>
            /// Whether to show detailed output. Currently unused but kept for future use.
            /// </summary>
            public bool DetailedOutput { get; set; } = false;
        }

        /// <summary>
        /// Runs battle simulations using BattleSimulator.
        /// </summary>
        /// <param name="config">Battle configuration. Cannot be null.</param>
        /// <param name="progress">Optional progress reporter (0-100).</param>
        /// <returns>Battle statistics from all simulations.</returns>
        /// <exception cref="ArgumentNullException">If config is null.</exception>
        public async Task<BattleStatistics> RunBattlesAsync(BattleConfig config, IProgress<int>? progress = null)
        {
            if (config == null)
                throw new ArgumentNullException(nameof(config));
            if (config.NumberOfBattles <= 0)
                throw new ArgumentException("NumberOfBattles must be greater than 0", nameof(config));
            if (config.Level <= 0 || config.Level > 100)
                throw new ArgumentException("Level must be between 1 and 100", nameof(config));

            // Select random Pokemon once if they are null
            var selectedPlayerPokemon = config.PlayerPokemon ?? _availablePokemon[_random.Next(_availablePokemon.Count)];
            var selectedEnemyPokemon = config.EnemyPokemon ?? _availablePokemon[_random.Next(_availablePokemon.Count)];

            // Create battle builder with configured Pokemon
            var builder = BattleBuilder.Create()
                .Singles()
                .WithPlayerPokemon(PokemonFactory.Create(selectedPlayerPokemon, config.Level))
                .WithEnemyPokemon(PokemonFactory.Create(selectedEnemyPokemon, config.Level))
                .WithRandomAI();

            // Configure simulation
            var simulationConfig = new BattleSimulator.SimulationConfig
            {
                NumberOfBattles = config.NumberOfBattles,
                UseRandomSeeds = true, // Each battle uses different random seed
                ResetStatisticsBetweenBattles = false, // Accumulate statistics across all battles
                CollectIndividualResults = false // Don't collect individual results to save memory
            };

            // Measure total simulation time
            var simulationStartTime = DateTime.Now;

            // Run simulation using the new BattleSimulator
            var results = await BattleSimulator.SimulateAsync(builder, simulationConfig, progress);

            var simulationEndTime = DateTime.Now;
            var totalSimulationTime = (simulationEndTime - simulationStartTime).TotalSeconds;

            // Store simulation time in results (we'll add a property for this)
            // For now, we'll return it via a wrapper or add it to BattleStatistics

            // Wrap results in compatibility wrapper
            return new BattleStatistics(results, totalSimulationTime);
        }
    }
}

