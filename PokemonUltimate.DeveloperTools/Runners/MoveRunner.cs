using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PokemonUltimate.Combat;
using PokemonUltimate.Combat.Actions;
using PokemonUltimate.Combat.Statistics;
using PokemonUltimate.Content.Catalogs.Moves;
using PokemonUltimate.Content.Catalogs.Pokemon;
using PokemonUltimate.Core.Blueprints;
using PokemonUltimate.Core.Enums;
using PokemonUltimate.Core.Factories;
using PokemonUltimate.Core.Instances;

namespace PokemonUltimate.DeveloperTools.Runners
{
    public class MoveRunner
    {
        /// <summary>
        /// Move test statistics wrapper for compatibility with existing UI code.
        /// Uses the new Statistics System internally.
        /// </summary>
        public class MoveTestStatistics
        {
            private readonly Combat.Statistics.BattleStatistics _internalStats;

            public MoveTestStatistics(Combat.Statistics.BattleStatistics internalStats)
            {
                _internalStats = internalStats ?? throw new ArgumentNullException(nameof(internalStats));
            }

            public List<int> DamageValues
            {
                get
                {
                    // Aggregate all damage values from all Pokemon
                    var allDamage = new List<int>();
                    foreach (var damageList in _internalStats.DamageStats.Values)
                    {
                        allDamage.AddRange(damageList);
                    }
                    return allDamage;
                }
            }

            public int CriticalHits => _internalStats.CriticalHits;
            public int Misses => _internalStats.Misses;

            public Dictionary<string, int> StatusEffectsCaused
            {
                get
                {
                    // Aggregate status effects from all Pokemon
                    var aggregated = new Dictionary<string, int>();
                    foreach (var pokemonStats in _internalStats.StatusEffectStats.Values)
                    {
                        foreach (var kvp in pokemonStats)
                        {
                            if (!aggregated.ContainsKey(kvp.Key))
                                aggregated[kvp.Key] = 0;
                            aggregated[kvp.Key] += kvp.Value;
                        }
                    }
                    return aggregated;
                }
            }

            public Dictionary<string, int> VolatileStatusEffectsCaused
            {
                get
                {
                    // Aggregate volatile status effects from all Pokemon
                    var aggregated = new Dictionary<string, int>();
                    foreach (var pokemonStats in _internalStats.VolatileStatusStats.Values)
                    {
                        foreach (var kvp in pokemonStats)
                        {
                            var key = $"Volatile_{kvp.Key}";
                            if (!aggregated.ContainsKey(key))
                                aggregated[key] = 0;
                            aggregated[key] += kvp.Value;
                        }
                    }
                    return aggregated;
                }
            }

            public Dictionary<string, int> ActionsGenerated => _internalStats.ActionTypeStats;
        }

        public class MoveTestConfig
        {
            public MoveData MoveToTest { get; set; } = null!;
            public PokemonSpeciesData AttackerPokemon { get; set; } = null!;
            public PokemonSpeciesData TargetPokemon { get; set; } = null!;
            public int Level { get; set; } = 50;
            public int NumberOfTests { get; set; } = 100;
            public bool DetailedOutput { get; set; } = false;
        }

        public async Task<MoveTestStatistics> RunTestsAsync(MoveTestConfig config, IProgress<int>? progress = null)
        {
            // Create statistics collector (shared across all tests)
            var statisticsCollector = new BattleStatisticsCollector();
            var internalStats = statisticsCollector.GetStatistics();
            var stats = new MoveTestStatistics(internalStats);

            // Reset statistics at the start of the batch
            statisticsCollector.Reset();

            for (int i = 0; i < config.NumberOfTests; i++)
            {
                await RunSingleTestAsync(config, statisticsCollector);

                // Statistics accumulate across all tests (not reset between tests)

                // Actualizar progreso solo cada 1% o cada 10 tests (lo que sea menor)
                // Esto previene stack overflow con muchas simulaciones
                var currentPercent = (i + 1) * 100 / config.NumberOfTests;
                var reportInterval = Math.Max(1, config.NumberOfTests / 100); // Al menos cada 1%
                if (progress != null && ((i + 1) % reportInterval == 0 || i == config.NumberOfTests - 1))
                {
                    progress.Report(currentPercent);
                }

                // Permitir que la UI se actualice
                await Task.Yield();
            }

            return stats;
        }

        private async Task RunSingleTestAsync(MoveTestConfig config, BattleStatisticsCollector statisticsCollector)
        {
            // Crear Pokemon
            var attacker = PokemonFactory.Create(config.AttackerPokemon, config.Level);
            var target = PokemonFactory.Create(config.TargetPokemon, config.Level);

            // Crear campo de batalla
            var rules = new BattleRules { PlayerSlots = 1, EnemySlots = 1 };
            var playerParty = new[] { attacker };
            var enemyParty = new[] { target };

            var field = new BattleField();
            field.Initialize(rules, playerParty, enemyParty);
            var playerSlot = field.PlayerSide.Slots[0];
            var enemySlot = field.EnemySide.Slots[0];

            // Crear instancia del movimiento
            var moveInstance = new MoveInstance(config.MoveToTest);
            if (!attacker.Moves.Contains(moveInstance))
            {
                attacker.Moves.Add(moveInstance);
            }

            // Crear acción de usar movimiento
            var useMoveAction = new UseMoveAction(playerSlot, enemySlot, moveInstance);

            // Create a temporary queue to use the statistics system
            var queue = new BattleQueue();
            queue.AddObserver(statisticsCollector);

            // Note: We don't call OnBattleStart here because it resets statistics
            // Statistics will accumulate across all tests in the batch
            // Reset() is called once at the start of RunTestsAsync

            // Enqueue the action
            queue.Enqueue(useMoveAction);

            // Process queue - statistics will be tracked automatically
            await queue.ProcessQueue(field, NullBattleView.Instance);

            // Remove observer after test to clean up
            queue.RemoveObserver(statisticsCollector);
        }
    }
}

