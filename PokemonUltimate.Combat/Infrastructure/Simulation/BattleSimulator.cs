using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PokemonUltimate.Combat.Engine;
using PokemonUltimate.Combat.Field;
using PokemonUltimate.Combat.Infrastructure.Builders;
using PokemonUltimate.Combat.Infrastructure.Constants;
using PokemonUltimate.Combat.Infrastructure.Statistics;
using PokemonUltimate.Core.Infrastructure.Factories;

namespace PokemonUltimate.Combat.Infrastructure.Simulation
{
    /// <summary>
    /// Simulates multiple battles with the same configuration and aggregates statistics.
    /// Useful for testing strategies, analyzing move effectiveness, and performance testing.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.6: Combat Engine
    /// **Documentation**: See `docs/features/2-combat-system/2.6-combat-engine/architecture.md`
    /// </remarks>
    public class BattleSimulator
    {
        /// <summary>
        /// Results from a batch of simulated battles.
        /// </summary>
        public class SimulationResults
        {
            /// <summary>
            /// Total number of battles simulated.
            /// </summary>
            public int TotalBattles { get; set; }

            /// <summary>
            /// Number of battles won by player.
            /// </summary>
            public int PlayerWins { get; set; }

            /// <summary>
            /// Number of battles won by enemy.
            /// </summary>
            public int EnemyWins { get; set; }

            /// <summary>
            /// Number of draws.
            /// </summary>
            public int Draws { get; set; }

            /// <summary>
            /// Aggregated statistics from all battles.
            /// </summary>
            public BattleStatistics AggregatedStatistics { get; set; }

            /// <summary>
            /// Individual battle results.
            /// </summary>
            public List<BattleResult> IndividualResults { get; set; } = new List<BattleResult>();

            /// <summary>
            /// Average turns per battle.
            /// </summary>
            public double AverageTurns => IndividualResults.Count > 0
                ? IndividualResults.Average(r => r.TurnsTaken)
                : 0;

            /// <summary>
            /// Win rate for player (0.0 to 1.0).
            /// </summary>
            public double PlayerWinRate => TotalBattles > 0
                ? (double)PlayerWins / TotalBattles
                : 0;

            /// <summary>
            /// Win rate for enemy (0.0 to 1.0).
            /// </summary>
            public double EnemyWinRate => TotalBattles > 0
                ? (double)EnemyWins / TotalBattles
                : 0;
        }

        /// <summary>
        /// Configuration for battle simulation.
        /// </summary>
        public class SimulationConfig
        {
            /// <summary>
            /// Number of battles to simulate.
            /// </summary>
            public int NumberOfBattles { get; set; } = 100;

            /// <summary>
            /// Whether to use different random seeds for each battle.
            /// If false, all battles use the same seed (deterministic).
            /// </summary>
            public bool UseRandomSeeds { get; set; } = true;

            /// <summary>
            /// Optional seed for reproducible simulations.
            /// If set and UseRandomSeeds is false, all battles use this seed.
            /// </summary>
            public int? FixedSeed { get; set; }

            /// <summary>
            /// Whether to reset statistics between battles.
            /// If false, statistics accumulate across all battles.
            /// </summary>
            public bool ResetStatisticsBetweenBattles { get; set; } = false;

            /// <summary>
            /// Whether to collect individual battle results.
            /// If false, only aggregated statistics are collected.
            /// </summary>
            public bool CollectIndividualResults { get; set; } = true;
        }

        /// <summary>
        /// Simulates multiple battles using the provided builder configuration.
        /// </summary>
        /// <param name="builder">The battle builder with configured parties, rules, AI, etc.</param>
        /// <param name="config">Simulation configuration. If null, uses defaults.</param>
        /// <param name="progress">Optional progress reporter (0-100).</param>
        /// <returns>Aggregated results from all simulated battles.</returns>
        public static async Task<SimulationResults> SimulateAsync(
            BattleBuilder builder,
            SimulationConfig config = null,
            IProgress<int> progress = null)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            config = config ?? new SimulationConfig();

            if (config.NumberOfBattles <= 0)
                throw new ArgumentException("NumberOfBattles must be greater than 0", nameof(config));

            var results = new SimulationResults
            {
                TotalBattles = config.NumberOfBattles
            };

            // Create a shared statistics collector for aggregation across all battles
            // This collector will accumulate statistics from all battles
            var aggregatedCollector = new BattleStatisticsCollector();

            // Random for generating seeds if needed
            var random = config.FixedSeed.HasValue
                ? new Random(config.FixedSeed.Value)
                : new Random();

            for (int i = 0; i < config.NumberOfBattles; i++)
            {
                // Create a new builder instance with the same configuration
                var battleBuilder = CloneBuilder(builder);

                // Set random seed for this battle
                if (config.UseRandomSeeds)
                {
                    var seed = config.FixedSeed ?? random.Next();
                    battleBuilder.WithRandomSeed(seed);
                }
                else if (config.FixedSeed.HasValue)
                {
                    battleBuilder.WithRandomSeed(config.FixedSeed.Value);
                }

                // Create a per-battle statistics collector
                var battleCollector = new BattleStatisticsCollector();

                // Enable statistics collection with the per-battle collector
                battleBuilder.WithStatistics(battleCollector);

                // Build and run battle (use ConfigureAwait(false) to avoid capturing context)
                var engine = battleBuilder.Build();
                var result = await engine.RunBattle().ConfigureAwait(false);

                // Collect individual result if requested
                if (config.CollectIndividualResults)
                {
                    results.IndividualResults.Add(result);
                }

                // Track outcome
                switch (result.Outcome)
                {
                    case BattleOutcome.Victory:
                        results.PlayerWins++;
                        break;
                    case BattleOutcome.Defeat:
                        results.EnemyWins++;
                        break;
                    case BattleOutcome.Draw:
                        results.Draws++;
                        break;
                }

                // Aggregate statistics from this battle into the aggregated collector
                var battleStats = battleCollector.GetStatistics();
                if (battleStats != null)
                {
                    AggregateStatistics(aggregatedCollector.GetStatistics(), battleStats);
                }

                // Reset per-battle statistics collector for next battle if requested
                // Note: We don't reset the aggregated collector - it accumulates across all battles
                if (config.ResetStatisticsBetweenBattles)
                {
                    battleCollector.Reset();
                }

                // IMPORTANT: Dispose engine to clean up event subscriptions and prevent memory leaks
                // This ensures event handlers are unsubscribed and resources are freed
                engine.Dispose();

                // Progress reporting removed - will only report at the end
            }

            results.AggregatedStatistics = aggregatedCollector.GetStatistics();

            // Report 100% completion at the end
            if (progress != null)
            {
                progress.Report(100);
            }

            return results;
        }

        /// <summary>
        /// Clones a BattleBuilder with the same configuration.
        /// Creates fresh Pokemon instances for each battle to ensure independence.
        /// </summary>
        private static BattleBuilder CloneBuilder(BattleBuilder original)
        {
            var clone = BattleBuilder.Create();

            // Create fresh Pokemon instances from species data
            // This ensures each battle has independent Pokemon with fresh stats/IVs
            foreach (var pokemon in original.PlayerParty)
            {
                // Create a new instance with the same species and level
                // Each battle will have different IVs/nature by default (random)
                var freshPokemon = PokemonFactory.Create(pokemon.Species, pokemon.Level);
                clone.WithPlayerPokemon(freshPokemon);
            }
            foreach (var pokemon in original.EnemyParty)
            {
                // Create a new instance with the same species and level
                var freshPokemon = PokemonFactory.Create(pokemon.Species, pokemon.Level);
                clone.WithEnemyPokemon(freshPokemon);
            }

            // Copy rules based on party sizes
            // Try to match the original format
            var playerSlots = original.PlayerParty.Count;
            var enemySlots = original.EnemyParty.Count;

            // Determine battle format based on party sizes
            if (playerSlots == 1 && enemySlots == 1)
                clone.Singles();
            else if (playerSlots == 2 && enemySlots == 2)
                clone.Doubles();
            else if (playerSlots == 3 && enemySlots == 3)
                clone.Triples();
            else if (playerSlots == 6 && enemySlots == 6)
                clone.FullTeam();
            else
                clone.WithRules(playerSlots, enemySlots);

            // Use RandomAI by default for simulations
            // This ensures each battle has independent AI decisions
            clone.WithRandomAI();

            return clone;
        }

        /// <summary>
        /// Aggregates statistics from a single battle into aggregated statistics.
        /// </summary>
        private static void AggregateStatistics(BattleStatistics aggregated, BattleStatistics battle)
        {
            // Aggregate totals
            aggregated.TotalTurns += battle.TotalTurns;
            aggregated.TotalActions += battle.TotalActions;

            // Aggregate actions by type
            foreach (var actionType in battle.ActionsByType)
            {
                if (!aggregated.ActionsByType.ContainsKey(actionType.Key))
                    aggregated.ActionsByType[actionType.Key] = 0;
                aggregated.ActionsByType[actionType.Key] += actionType.Value;
            }

            // Aggregate damage and healing
            aggregated.PlayerDamageDealt += battle.PlayerDamageDealt;
            aggregated.EnemyDamageDealt += battle.EnemyDamageDealt;
            aggregated.PlayerHealing += battle.PlayerHealing;
            aggregated.EnemyHealing += battle.EnemyHealing;

            // Aggregate move usage
            foreach (var move in battle.PlayerMoveUsage)
            {
                if (!aggregated.PlayerMoveUsage.ContainsKey(move.Key))
                    aggregated.PlayerMoveUsage[move.Key] = 0;
                aggregated.PlayerMoveUsage[move.Key] += move.Value;
            }
            foreach (var move in battle.EnemyMoveUsage)
            {
                if (!aggregated.EnemyMoveUsage.ContainsKey(move.Key))
                    aggregated.EnemyMoveUsage[move.Key] = 0;
                aggregated.EnemyMoveUsage[move.Key] += move.Value;
            }

            // Aggregate move usage by Pokemon
            foreach (var pokemon in battle.MoveUsageByPokemon)
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
            foreach (var move in battle.DamageByMove)
            {
                if (!aggregated.DamageByMove.ContainsKey(move.Key))
                    aggregated.DamageByMove[move.Key] = 0;
                aggregated.DamageByMove[move.Key] += move.Value;
            }

            // Aggregate individual damage values by move
            foreach (var move in battle.DamageValuesByMove)
            {
                if (!aggregated.DamageValuesByMove.ContainsKey(move.Key))
                    aggregated.DamageValuesByMove[move.Key] = new List<int>();
                aggregated.DamageValuesByMove[move.Key].AddRange(move.Value);
            }

            // Aggregate critical hits and missed moves
            aggregated.CriticalHits += battle.CriticalHits;
            aggregated.MissedMoves += battle.MissedMoves;

            // Aggregate fainted Pokemon (lists)
            aggregated.PlayerFainted.AddRange(battle.PlayerFainted);
            aggregated.EnemyFainted.AddRange(battle.EnemyFainted);

            // Aggregate switches
            foreach (var pokemon in battle.PokemonSwitches)
            {
                if (!aggregated.PokemonSwitches.ContainsKey(pokemon.Key))
                    aggregated.PokemonSwitches[pokemon.Key] = 0;
                aggregated.PokemonSwitches[pokemon.Key] += pokemon.Value;
            }

            // Aggregate status effects
            foreach (var status in battle.StatusEffectsApplied)
            {
                if (!aggregated.StatusEffectsApplied.ContainsKey(status.Key))
                    aggregated.StatusEffectsApplied[status.Key] = 0;
                aggregated.StatusEffectsApplied[status.Key] += status.Value;
            }

            // Aggregate stat changes
            foreach (var pokemon in battle.StatChanges)
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
            aggregated.WeatherChanges.AddRange(battle.WeatherChanges);
            aggregated.TerrainChanges.AddRange(battle.TerrainChanges);

            // Aggregate actions by Pokemon
            foreach (var pokemon in battle.ActionsByPokemon)
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
            foreach (var action in battle.PlayerActionsByType)
            {
                if (!aggregated.PlayerActionsByType.ContainsKey(action.Key))
                    aggregated.PlayerActionsByType[action.Key] = 0;
                aggregated.PlayerActionsByType[action.Key] += action.Value;
            }
            foreach (var action in battle.EnemyActionsByType)
            {
                if (!aggregated.EnemyActionsByType.ContainsKey(action.Key))
                    aggregated.EnemyActionsByType[action.Key] = 0;
                aggregated.EnemyActionsByType[action.Key] += action.Value;
            }
        }
    }
}
