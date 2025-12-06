using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PokemonUltimate.Combat;
using PokemonUltimate.Combat.Actions;
using PokemonUltimate.Combat.AI;
using PokemonUltimate.Combat.Factories;
using PokemonUltimate.Combat.Statistics;
using PokemonUltimate.Content.Catalogs.Pokemon;
using PokemonUltimate.Core.Blueprints;
using PokemonUltimate.Core.Factories;
using PokemonUltimate.Core.Instances;

namespace PokemonUltimate.UnifiedDebuggerUI.Runners
{
    public class BattleRunner
    {
        private readonly Random _random = new Random();
        private readonly List<PokemonSpeciesData> _availablePokemon = PokemonCatalog.All.ToList();

        /// <summary>
        /// Battle statistics wrapper for compatibility with existing UI code.
        /// Uses the new Statistics System internally.
        /// </summary>
        public class BattleStatistics
        {
            private readonly Combat.Statistics.BattleStatistics _internalStats;

            public BattleStatistics(Combat.Statistics.BattleStatistics internalStats)
            {
                _internalStats = internalStats ?? throw new ArgumentNullException(nameof(internalStats));
            }

            public int PlayerWins
            {
                get => _internalStats.PlayerWins;
                set => _internalStats.PlayerWins = value;
            }

            public int EnemyWins
            {
                get => _internalStats.EnemyWins;
                set => _internalStats.EnemyWins = value;
            }

            public int Draws
            {
                get => _internalStats.Draws;
                set => _internalStats.Draws = value;
            }

            public Dictionary<string, Dictionary<string, int>> MoveUsageStats => _internalStats.MoveUsageStats;
            public Dictionary<string, Dictionary<string, int>> StatusEffectStats => _internalStats.StatusEffectStats;
        }

        public class BattleConfig
        {
            public PokemonSpeciesData? PlayerPokemon { get; set; }
            public PokemonSpeciesData? EnemyPokemon { get; set; }
            public int Level { get; set; } = 50;
            public int NumberOfBattles { get; set; } = 100;
            public bool DetailedOutput { get; set; } = false;
        }

        public async Task<BattleStatistics> RunBattlesAsync(BattleConfig config, IProgress<int>? progress = null)
        {
            // Create statistics collector (shared across all battles)
            var statisticsCollector = new BattleStatisticsCollector();
            var internalStats = statisticsCollector.GetStatistics();
            var stats = new BattleStatistics(internalStats);
            
            // Reset statistics at the start of the batch
            statisticsCollector.Reset();
            
            // Seleccionar Pokemon aleatorios una sola vez si son null
            var selectedPlayerPokemon = config.PlayerPokemon ?? _availablePokemon[_random.Next(_availablePokemon.Count)];
            var selectedEnemyPokemon = config.EnemyPokemon ?? _availablePokemon[_random.Next(_availablePokemon.Count)];

            for (int i = 0; i < config.NumberOfBattles; i++)
            {
                var result = await RunSingleBattleAsync(selectedPlayerPokemon, selectedEnemyPokemon, config.Level, statisticsCollector);

                // Statistics are automatically tracked by the collector
                // Outcome is tracked in OnBattleEnd
                // Statistics accumulate across battles (not reset between battles)

                // Actualizar progreso
                progress?.Report((i + 1) * 100 / config.NumberOfBattles);
            }

            return stats;
        }

        private async Task<BattleResult> RunSingleBattleAsync(
            PokemonSpeciesData playerPokemon,
            PokemonSpeciesData enemyPokemon,
            int level,
            BattleStatisticsCollector statisticsCollector)
        {
            // Crear parties
            var playerParty = new[] { PokemonFactory.Create(playerPokemon, level) };
            var enemyParty = new[] { PokemonFactory.Create(enemyPokemon, level) };

            // Crear AI
            var playerAI = new RandomAI();
            var enemyAI = new RandomAI();

            // Crear view (null view para no mostrar nada)
            var view = Combat.NullBattleView.Instance;

            // Crear engine
            var engine = CombatEngineFactory.Create();
            var rules = new BattleRules { PlayerSlots = 1, EnemySlots = 1 };

            // Initialize
            engine.Initialize(rules, playerParty, enemyParty, playerAI, enemyAI, view);

            // Register statistics observer BEFORE battle starts
            engine.Queue.AddObserver(statisticsCollector);
            
            // Note: We don't call OnBattleStart here because it resets statistics
            // Statistics will accumulate across all battles in the batch
            // Reset() is called once at the start of RunBattlesAsync

            // Ejecutar batalla normalmente - statistics are tracked automatically
            var result = await engine.RunBattle();

            // Notify observer of battle end (tracks outcome)
            statisticsCollector.OnBattleEnd(result.Outcome, engine.Field);

            // Remove observer after battle to clean up
            engine.Queue.RemoveObserver(statisticsCollector);

            return result;
        }

    }
}

