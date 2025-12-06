using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PokemonUltimate.Combat;
using PokemonUltimate.Combat.Actions;
using PokemonUltimate.Combat.AI;
using PokemonUltimate.Combat.Factories;
using PokemonUltimate.Content.Catalogs.Pokemon;
using PokemonUltimate.Core.Blueprints;
using PokemonUltimate.Core.Factories;
using PokemonUltimate.Core.Instances;

namespace PokemonUltimate.BattleDebuggerUI
{
    public class BattleRunner
    {
        private readonly Random _random = new Random();
        private readonly List<PokemonSpeciesData> _availablePokemon = PokemonCatalog.All.ToList();

        public class BattleStatistics
        {
            public int PlayerWins { get; set; }
            public int EnemyWins { get; set; }
            public int Draws { get; set; }
            public Dictionary<string, Dictionary<string, int>> MoveUsageStats { get; set; } = new();
            public Dictionary<string, Dictionary<string, int>> StatusEffectStats { get; set; } = new();
        }

        public class BattleConfig
        {
            public PokemonSpeciesData? PlayerPokemon { get; set; }
            public PokemonSpeciesData? EnemyPokemon { get; set; }
            public int Level { get; set; } = 50;
            public int NumberOfBattles { get; set; } = 100;
            public bool DetailedOutput { get; set; } = false;
        }

        public async Task<BattleStatistics> RunBattlesAsync(BattleConfig config, IProgress<int> progress = null)
        {
            var stats = new BattleStatistics();
            
            // Seleccionar Pokemon aleatorios una sola vez si son null
            var selectedPlayerPokemon = config.PlayerPokemon ?? _availablePokemon[_random.Next(_availablePokemon.Count)];
            var selectedEnemyPokemon = config.EnemyPokemon ?? _availablePokemon[_random.Next(_availablePokemon.Count)];

            for (int i = 0; i < config.NumberOfBattles; i++)
            {
                var result = await RunSingleBattleAsync(selectedPlayerPokemon, selectedEnemyPokemon, config.Level, stats);

                // Contar victorias
                if (result.Outcome == BattleOutcome.Victory)
                {
                    stats.PlayerWins++;
                }
                else if (result.Outcome == BattleOutcome.Defeat)
                {
                    stats.EnemyWins++;
                }
                else
                {
                    stats.Draws++;
                }

                // Actualizar progreso
                progress?.Report((i + 1) * 100 / config.NumberOfBattles);
            }

            return stats;
        }

        private async Task<BattleResult> RunSingleBattleAsync(
            PokemonSpeciesData playerPokemon,
            PokemonSpeciesData enemyPokemon,
            int level,
            BattleStatistics stats)
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

            // Ejecutar batalla con tracking
            var result = await RunBattleWithTracking(engine, view, stats);

            return result;
        }

        private async Task<BattleResult> RunBattleWithTracking(CombatEngine engine, IBattleView view, BattleStatistics stats)
        {
            int turnCount = 0;
            const int maxTurns = 1000;
            var outcome = BattleOutcome.Ongoing;

            while (outcome == BattleOutcome.Ongoing && turnCount < maxTurns)
            {
                // Collect actions
                var pendingActions = new List<BattleAction>();
                foreach (var slot in engine.Field.GetAllActiveSlots())
                {
                    if (slot.ActionProvider != null)
                    {
                        var action = await slot.ActionProvider.GetAction(engine.Field, slot);
                        if (action != null)
                        {
                            pendingActions.Add(action);
                        }
                    }
                }

                // Sort by turn order
                var helpers = CombatEngineFactory.CreateHelpers();
                var sortedActions = helpers.TurnOrderResolver.SortActions(pendingActions, engine.Field);

                // Process actions with tracking
                foreach (var action in sortedActions)
                {
                    // Track move usage
                    if (action is UseMoveAction moveAction && moveAction.User?.Pokemon != null)
                    {
                        var pokemonName = moveAction.User.Pokemon.Species.Name;
                        var moveName = moveAction.Move.Name;

                        if (!stats.MoveUsageStats.ContainsKey(pokemonName))
                        {
                            stats.MoveUsageStats[pokemonName] = new Dictionary<string, int>();
                        }

                        if (!stats.MoveUsageStats[pokemonName].ContainsKey(moveName))
                        {
                            stats.MoveUsageStats[pokemonName][moveName] = 0;
                        }

                        stats.MoveUsageStats[pokemonName][moveName]++;
                    }

                    // Execute logic
                    var reactions = action.ExecuteLogic(engine.Field);
                    var reactionList = reactions?.ToList() ?? new List<BattleAction>();

                    // Track status effects in reactions
                    foreach (var reaction in reactionList)
                    {
                        if (reaction is ApplyStatusAction statusAction && statusAction.User?.Pokemon != null && statusAction.Status != PokemonUltimate.Core.Enums.PersistentStatus.None)
                        {
                            var pokemonName = statusAction.User.Pokemon.Species.Name;
                            var statusName = statusAction.Status.ToString();

                            if (!stats.StatusEffectStats.ContainsKey(pokemonName))
                            {
                                stats.StatusEffectStats[pokemonName] = new Dictionary<string, int>();
                            }

                            if (!stats.StatusEffectStats[pokemonName].ContainsKey(statusName))
                            {
                                stats.StatusEffectStats[pokemonName][statusName] = 0;
                            }

                            stats.StatusEffectStats[pokemonName][statusName]++;
                        }
                    }

                    // Execute visual
                    await action.ExecuteVisual(view);

                    // Enqueue reactions
                    foreach (var reaction in reactionList)
                    {
                        engine.Queue.Enqueue(reaction);
                    }
                }

                // Process queue with tracking
                await ProcessQueueWithTracking(engine.Queue, engine.Field, view, stats);

                turnCount++;
                outcome = BattleArbiter.CheckOutcome(engine.Field);
            }

            return new BattleResult
            {
                Outcome = outcome,
                TurnsTaken = turnCount
            };
        }

        private async Task ProcessQueueWithTracking(BattleQueue queue, BattleField field, IBattleView view, BattleStatistics stats)
        {
            int iterationCount = 0;
            const int maxIterations = 1000;

            // Use reflection to access private _queue field
            var queueField = typeof(BattleQueue).GetField("_queue", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (queueField == null)
            {
                await queue.ProcessQueue(field, view);
                return;
            }

            var linkedList = queueField.GetValue(queue) as System.Collections.Generic.LinkedList<BattleAction>;
            if (linkedList == null)
            {
                await queue.ProcessQueue(field, view);
                return;
            }

            while (linkedList.Count > 0)
            {
                if (iterationCount++ > maxIterations)
                {
                    throw new InvalidOperationException("Battle queue infinite loop detected");
                }

                var action = linkedList.First.Value;
                linkedList.RemoveFirst();

                // Track ApplyStatusAction before executing
                if (action is ApplyStatusAction statusAction && statusAction.User?.Pokemon != null && statusAction.Status != PokemonUltimate.Core.Enums.PersistentStatus.None)
                {
                    var pokemonName = statusAction.User.Pokemon.Species.Name;
                    var statusName = statusAction.Status.ToString();

                    if (!stats.StatusEffectStats.ContainsKey(pokemonName))
                    {
                        stats.StatusEffectStats[pokemonName] = new Dictionary<string, int>();
                    }

                    if (!stats.StatusEffectStats[pokemonName].ContainsKey(statusName))
                    {
                        stats.StatusEffectStats[pokemonName][statusName] = 0;
                    }

                    stats.StatusEffectStats[pokemonName][statusName]++;
                }

                // Execute logic
                var reactions = action.ExecuteLogic(field);

                // Execute visual
                await action.ExecuteVisual(view);

                // Insert reactions at front
                if (reactions != null)
                {
                    foreach (var reaction in reactions.Reverse())
                    {
                        linkedList.AddFirst(reaction);
                    }
                }
            }
        }
    }

}

