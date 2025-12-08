using System.Linq;
using PokemonUltimate.Combat.Actions;
using PokemonUltimate.Combat.Engine;
using PokemonUltimate.Combat.Infrastructure.Builders;
using PokemonUltimate.Combat.Infrastructure.Events;
using PokemonUltimate.Combat.Infrastructure.Simulation;
using PokemonUltimate.Combat.Infrastructure.Statistics;
using PokemonUltimate.Content.Catalogs.Pokemon;
using PokemonUltimate.Core.Data.Blueprints;
using PokemonUltimate.Core.Data.Enums;
using PokemonUltimate.Core.Infrastructure.Factories;
using PokemonUltimate.Localization.Providers;
using PokemonUltimate.Localization.Services;

namespace PokemonUltimate.BattleTest
{
    /// <summary>
    /// Console application to test the new battle system with random Pokemon.
    /// </summary>
    class Program
    {
        // Configuration
        private const bool WaitBetweenBattles = true; // Set to false to run all battles without waiting
        private const bool WaitBeforeExit = true; // Set to false to exit immediately after all battles

        private static readonly Random _random = new Random();
        private static int _actionCounter = 0; // Counter for actions executed

        static async Task Main(string[] args)
        {
            // Initialize localization service
            LocalizationService.Initialize(new LocalizationProvider(), "es");

            Console.WriteLine("=== Pokemon Battle Test ===");
            Console.WriteLine();
            Console.WriteLine("Select mode:");
            Console.WriteLine("  1. Run individual battles (with detailed output)");
            Console.WriteLine("  2. Run simulation (batch battles with aggregated statistics)");
            Console.Write("Enter choice (1 or 2, default: 1): ");

            var choice = Console.ReadLine()?.Trim();
            bool useSimulation = choice == "2";

            if (useSimulation)
            {
                await RunSimulation(args);
            }
            else
            {
                await RunIndividualBattles(args);
            }
        }

        static async Task RunSimulation(string[] args)
        {
            Console.WriteLine();
            Console.WriteLine("=== Battle Simulation Mode ===");
            Console.WriteLine();

            // Get number of battles from args or use default
            int numberOfBattles = 100;
            if (args.Length > 0 && int.TryParse(args[0], out int battles))
            {
                numberOfBattles = battles;
            }

            // Get level from args or use default
            int level = 50;
            if (args.Length > 1 && int.TryParse(args[1], out int pokemonLevel))
            {
                level = pokemonLevel;
            }

            // Get Pokemon species from args or use random
            PokemonSpeciesData? playerSpecies = null;
            PokemonSpeciesData? enemySpecies = null;

            if (args.Length > 2)
            {
                // Try to find player Pokemon by name
                var playerName = args[2];
                playerSpecies = PokemonCatalog.All.FirstOrDefault(p =>
                    p.Name.Equals(playerName, StringComparison.OrdinalIgnoreCase));
            }

            if (args.Length > 3)
            {
                // Try to find enemy Pokemon by name
                var enemyName = args[3];
                enemySpecies = PokemonCatalog.All.FirstOrDefault(p =>
                    p.Name.Equals(enemyName, StringComparison.OrdinalIgnoreCase));
            }

            // Use random if not specified
            if (playerSpecies == null)
                playerSpecies = PokemonCatalog.All[_random.Next(PokemonCatalog.All.Count)];
            if (enemySpecies == null)
            {
                var availableEnemies = PokemonCatalog.All.Where(p => p != playerSpecies).ToList();
                enemySpecies = availableEnemies[_random.Next(availableEnemies.Count)];
            }

            Console.WriteLine($"Simulating {numberOfBattles} battles:");
            Console.WriteLine($"  Player: {playerSpecies.Name} (Lv.{level})");
            Console.WriteLine($"  Enemy:  {enemySpecies.Name} (Lv.{level})");
            Console.WriteLine();

            // Configure base battle
            var builder = BattleBuilder.Create()
                .Singles()
                .WithPlayerPokemon(PokemonFactory.Create(playerSpecies, level))
                .WithEnemyPokemon(PokemonFactory.Create(enemySpecies, level))
                .WithRandomAI();

            // Configure simulation
            var config = new BattleSimulator.SimulationConfig
            {
                NumberOfBattles = numberOfBattles,
                UseRandomSeeds = true,
                CollectIndividualResults = false  // No guardar resultados individuales para ahorrar memoria
            };

            // Simulate with progress reporting
            var progress = new Progress<int>(percent =>
            {
                if (percent % 10 == 0 || percent == 100)
                {
                    Console.Write($"\rSimulating... {percent}%");
                }
            });

            var startTime = DateTime.Now;
            var results = await BattleSimulator.SimulateAsync(builder, config, progress);
            var duration = DateTime.Now - startTime;

            Console.WriteLine(); // New line after progress
            Console.WriteLine();
            Console.WriteLine("=== Simulation Results ===");
            Console.WriteLine($"Total Battles: {results.TotalBattles}");
            Console.WriteLine($"Duration: {duration.TotalSeconds:F2} seconds");
            Console.WriteLine($"Battles per second: {results.TotalBattles / duration.TotalSeconds:F2}");
            Console.WriteLine();
            Console.WriteLine($"Player Wins: {results.PlayerWins} ({results.PlayerWinRate:P2})");
            Console.WriteLine($"Enemy Wins: {results.EnemyWins} ({results.EnemyWinRate:P2})");
            Console.WriteLine($"Draws: {results.Draws}");
            Console.WriteLine($"Average Turns: {results.AverageTurns:F2}");
            Console.WriteLine();

            // Display aggregated statistics
            var stats = results.AggregatedStatistics;
            Console.WriteLine("=== Aggregated Statistics ===");
            Console.WriteLine($"Total Actions: {stats.TotalActions}");
            Console.WriteLine($"Total Turns: {stats.TotalTurns}");
            Console.WriteLine($"Player Damage: {stats.PlayerDamageDealt}");
            Console.WriteLine($"Enemy Damage: {stats.EnemyDamageDealt}");
            Console.WriteLine($"Critical Hits: {stats.CriticalHits}");
            Console.WriteLine();

            // Display top moves
            if (stats.PlayerMoveUsage.Count > 0)
            {
                Console.WriteLine("=== Top Moves (Player) ===");
                foreach (var move in stats.PlayerMoveUsage.OrderByDescending(m => m.Value).Take(5))
                {
                    var damage = stats.DamageByMove.GetValueOrDefault(move.Key, 0);
                    var avgDamage = move.Value > 0 ? damage / move.Value : 0;
                    var percentage = stats.PlayerMoveUsage.Values.Sum() > 0
                        ? (move.Value * 100.0 / stats.PlayerMoveUsage.Values.Sum())
                        : 0;
                    Console.WriteLine($"  {move.Key}: {move.Value} uses ({percentage:F1}%), {damage} total damage ({avgDamage} avg)");
                }
                Console.WriteLine();
            }

            if (stats.EnemyMoveUsage.Count > 0)
            {
                Console.WriteLine("=== Top Moves (Enemy) ===");
                foreach (var move in stats.EnemyMoveUsage.OrderByDescending(m => m.Value).Take(5))
                {
                    var damage = stats.DamageByMove.GetValueOrDefault(move.Key, 0);
                    var avgDamage = move.Value > 0 ? damage / move.Value : 0;
                    var percentage = stats.EnemyMoveUsage.Values.Sum() > 0
                        ? (move.Value * 100.0 / stats.EnemyMoveUsage.Values.Sum())
                        : 0;
                    Console.WriteLine($"  {move.Key}: {move.Value} uses ({percentage:F1}%), {damage} total damage ({avgDamage} avg)");
                }
                Console.WriteLine();
            }

            if (WaitBeforeExit)
            {
                Console.WriteLine("Press any key to exit...");
                Console.ReadKey(true);
            }
        }

        static async Task RunIndividualBattles(string[] args)
        {
            // Get number of battles from args or use default
            int numberOfBattles = 10;
            if (args.Length > 0 && int.TryParse(args[0], out int battles))
            {
                numberOfBattles = battles;
            }

            // Get level from args or use default
            int level = 50;
            if (args.Length > 1 && int.TryParse(args[1], out int pokemonLevel))
            {
                level = pokemonLevel;
            }

            Console.WriteLine($"Running {numberOfBattles} battle(s) at level {level}");
            Console.WriteLine();

            for (int i = 0; i < numberOfBattles; i++)
            {
                if (numberOfBattles > 1)
                {
                    Console.WriteLine($"--- Battle {i + 1}/{numberOfBattles} ---");
                }

                await RunRandomBattle(level);
                Console.WriteLine();

                // Wait for user to press a key before next battle (if configured)
                if (WaitBetweenBattles && i < numberOfBattles - 1)
                {
                    Console.WriteLine("Press any key to start the next battle...");
                    Console.ReadKey(true);
                    Console.WriteLine();
                }
            }

            Console.WriteLine("All battles completed!");
            if (WaitBeforeExit)
            {
                Console.WriteLine("Press any key to exit...");
                Console.ReadKey(true);
            }
        }

        static async Task RunRandomBattle(int level)
        {
            // Reset action counter for this battle
            _actionCounter = 0;

            // Create builder with statistics collection and action logging
            var builder = BattleBuilder.Create()
                .Singles()
                .Random(level, _random.Next())
                .WithStatistics()  // Enable automatic statistics collection
                .OnActionExecuted((s, e) => DisplayAction(e))  // Show each action as it executes
                .WithRandomAI();

            // Display Pokemon info before battle starts
            if (builder.PlayerParty.Count > 1)
            {
                Console.WriteLine($"Player Team ({builder.PlayerParty.Count} Pokemon):");
                foreach (var pokemon in builder.PlayerParty)
                {
                    Console.WriteLine($"  - {pokemon.DisplayName} (Lv.{pokemon.Level})");
                }
            }
            else
            {
                var playerPokemon = builder.PlayerParty.FirstOrDefault();
                if (playerPokemon != null)
                    Console.WriteLine($"Player: {playerPokemon.DisplayName} (Lv.{playerPokemon.Level})");
            }

            if (builder.EnemyParty.Count > 1)
            {
                Console.WriteLine($"Enemy Team ({builder.EnemyParty.Count} Pokemon):");
                foreach (var pokemon in builder.EnemyParty)
                {
                    Console.WriteLine($"  - {pokemon.DisplayName} (Lv.{pokemon.Level})");
                }
            }
            else
            {
                var enemyPokemon = builder.EnemyParty.FirstOrDefault();
                if (enemyPokemon != null)
                    Console.WriteLine($"Enemy:  {enemyPokemon.DisplayName} (Lv.{enemyPokemon.Level})");
            }

            Console.WriteLine();

            // Build and initialize battle
            var engine = builder.Build();

            Console.WriteLine("Battle started!");
            Console.WriteLine();

            // Run battle
            var startTime = DateTime.Now;
            var result = await engine.RunBattle();
            var duration = DateTime.Now - startTime;

            // Get statistics from builder
            var stats = builder.StatisticsCollector?.GetStatistics();

            // Display results
            Console.WriteLine("=== Battle Results ===");
            Console.WriteLine($"Outcome: {result.Outcome}");
            Console.WriteLine($"Turns: {result.TurnsTaken}");
            Console.WriteLine($"Duration: {duration.TotalSeconds:F2} seconds");
            Console.WriteLine();

            // Display statistics if available
            if (stats != null)
            {
                DisplayStatistics(stats);
            }

            // Display final HP
            if (engine.Field != null)
            {
                var playerSlot = engine.Field.PlayerSide.Slots.FirstOrDefault(s => !s.IsEmpty);
                var enemySlot = engine.Field.EnemySide.Slots.FirstOrDefault(s => !s.IsEmpty);

                if (playerSlot?.Pokemon != null)
                {
                    Console.WriteLine(
                        $"Player {playerSlot.Pokemon.DisplayName}: {playerSlot.Pokemon.CurrentHP}/{playerSlot.Pokemon.MaxHP} HP");
                    if (playerSlot.Pokemon.IsFainted)
                    {
                        Console.WriteLine("  Status: FAINTED");
                    }
                    else if (playerSlot.Pokemon.Status != PersistentStatus.None)
                    {
                        Console.WriteLine($"  Status: {playerSlot.Pokemon.Status}");
                    }
                }

                if (enemySlot?.Pokemon != null)
                {
                    Console.WriteLine(
                        $"Enemy {enemySlot.Pokemon.DisplayName}: {enemySlot.Pokemon.CurrentHP}/{enemySlot.Pokemon.MaxHP} HP");
                    if (enemySlot.Pokemon.IsFainted)
                    {
                        Console.WriteLine("  Status: FAINTED");
                    }
                    else if (enemySlot.Pokemon.Status != PersistentStatus.None)
                    {
                        Console.WriteLine($"  Status: {enemySlot.Pokemon.Status}");
                    }
                }
            }
        }

        static void DisplayStatistics(BattleStatistics stats)
        {
            Console.WriteLine("=== Battle Statistics ===");
            Console.WriteLine($"Total Actions: {stats.TotalActions}");
            Console.WriteLine($"Player Damage: {stats.PlayerDamageDealt}");
            Console.WriteLine($"Enemy Damage: {stats.EnemyDamageDealt}");
            Console.WriteLine($"Critical Hits: {stats.CriticalHits}");
            Console.WriteLine();

            // Calculate total move usage (player + enemy)
            var allMoves = new Dictionary<string, int>();
            foreach (var move in stats.PlayerMoveUsage)
            {
                allMoves[move.Key] = move.Value;
            }
            foreach (var move in stats.EnemyMoveUsage)
            {
                if (allMoves.ContainsKey(move.Key))
                    allMoves[move.Key] += move.Value;
                else
                    allMoves[move.Key] = move.Value;
            }

            var totalMoveUses = allMoves.Values.Sum();

            // Display moves with percentages
            if (allMoves.Count > 0 && totalMoveUses > 0)
            {
                Console.WriteLine("=== Move Usage Statistics ===");
                Console.WriteLine($"Total move uses: {totalMoveUses}");
                Console.WriteLine();

                // Player moves
                if (stats.PlayerMoveUsage.Count > 0)
                {
                    Console.WriteLine("Player Moves:");
                    foreach (var move in stats.PlayerMoveUsage.OrderByDescending(m => m.Value))
                    {
                        var percentage = totalMoveUses > 0 ? (move.Value * 100.0 / totalMoveUses) : 0;
                        var damage = stats.DamageByMove.GetValueOrDefault(move.Key, 0);
                        var avgDamage = move.Value > 0 ? damage / move.Value : 0;
                        Console.WriteLine($"  {move.Key}: {move.Value} uses ({percentage:F1}%), {damage} total damage ({avgDamage} avg)");
                    }
                    Console.WriteLine();
                }

                // Enemy moves
                if (stats.EnemyMoveUsage.Count > 0)
                {
                    Console.WriteLine("Enemy Moves:");
                    foreach (var move in stats.EnemyMoveUsage.OrderByDescending(m => m.Value))
                    {
                        var percentage = totalMoveUses > 0 ? (move.Value * 100.0 / totalMoveUses) : 0;
                        var damage = stats.DamageByMove.GetValueOrDefault(move.Key, 0);
                        var avgDamage = move.Value > 0 ? damage / move.Value : 0;
                        Console.WriteLine($"  {move.Key}: {move.Value} uses ({percentage:F1}%), {damage} total damage ({avgDamage} avg)");
                    }
                    Console.WriteLine();
                }

                // Combined moves (all moves used in battle)
                Console.WriteLine("All Moves Used (by percentage):");
                foreach (var move in allMoves.OrderByDescending(m => m.Value))
                {
                    var percentage = totalMoveUses > 0 ? (move.Value * 100.0 / totalMoveUses) : 0;
                    var playerUses = stats.PlayerMoveUsage.GetValueOrDefault(move.Key, 0);
                    var enemyUses = stats.EnemyMoveUsage.GetValueOrDefault(move.Key, 0);
                    Console.WriteLine($"  {move.Key}: {move.Value} total ({percentage:F1}%) [Player: {playerUses}, Enemy: {enemyUses}]");
                }
                Console.WriteLine();
            }

            // Display actions per turn
            if (stats.ActionsPerTurn.Count > 0)
            {
                var avgActionsPerTurn = stats.ActionsPerTurn.Values.Average();
                Console.WriteLine($"Average actions per turn: {avgActionsPerTurn:F2}");
                Console.WriteLine();
            }

            // Display actions by team
            if (stats.PlayerActionsByType.Count > 0 || stats.EnemyActionsByType.Count > 0)
            {
                Console.WriteLine("=== Actions by Team ===");

                if (stats.PlayerActionsByType.Count > 0)
                {
                    var playerTotal = stats.PlayerActionsByType.Values.Sum();
                    Console.WriteLine($"Player Team ({playerTotal} total actions):");
                    foreach (var action in stats.PlayerActionsByType.OrderByDescending(a => a.Value))
                    {
                        var percentage = playerTotal > 0 ? (action.Value * 100.0 / playerTotal) : 0;
                        Console.WriteLine($"  {action.Key}: {action.Value} ({percentage:F1}%)");
                    }
                    Console.WriteLine();
                }

                if (stats.EnemyActionsByType.Count > 0)
                {
                    var enemyTotal = stats.EnemyActionsByType.Values.Sum();
                    Console.WriteLine($"Enemy Team ({enemyTotal} total actions):");
                    foreach (var action in stats.EnemyActionsByType.OrderByDescending(a => a.Value))
                    {
                        var percentage = enemyTotal > 0 ? (action.Value * 100.0 / enemyTotal) : 0;
                        Console.WriteLine($"  {action.Key}: {action.Value} ({percentage:F1}%)");
                    }
                    Console.WriteLine();
                }
            }

            // Display actions by Pokemon
            if (stats.ActionsByPokemon.Count > 0)
            {
                Console.WriteLine("=== Actions by Pokemon ===");
                foreach (var pokemon in stats.ActionsByPokemon.OrderBy(p => p.Key))
                {
                    var pokemonTotal = pokemon.Value.Values.Sum();
                    Console.WriteLine($"{pokemon.Key} ({pokemonTotal} total actions):");
                    foreach (var action in pokemon.Value.OrderByDescending(a => a.Value))
                    {
                        var percentage = pokemonTotal > 0 ? (action.Value * 100.0 / pokemonTotal) : 0;
                        Console.WriteLine($"  {action.Key}: {action.Value} ({percentage:F1}%)");
                    }
                    Console.WriteLine();
                }
            }

            // Display fainted Pokemon
            if (stats.PlayerFainted.Count > 0)
            {
                Console.WriteLine($"Player Pokemon fainted: {string.Join(", ", stats.PlayerFainted)}");
            }
            if (stats.EnemyFainted.Count > 0)
            {
                Console.WriteLine($"Enemy Pokemon fainted: {string.Join(", ", stats.EnemyFainted)}");
            }
            if (stats.PlayerFainted.Count > 0 || stats.EnemyFainted.Count > 0)
            {
                Console.WriteLine();
            }
        }

        static void DisplayAction(ActionExecutedEventArgs e)
        {
            _actionCounter++;
            var action = e.Action;
            var actionType = action.GetType().Name;

            // Get user name
            string userName = action.User?.Pokemon?.DisplayName ?? "System";

            // Format action description based on type
            string actionDescription = FormatActionDescription(action);

            Console.WriteLine($"  [{_actionCounter:D3}] [{actionType}] {userName}: {actionDescription}");
        }

        static string FormatActionDescription(BattleAction action)
        {
            return action switch
            {
                UseMoveAction moveAction =>
                    $"{moveAction.MoveInstance?.Move?.Name ?? "Unknown"} → {moveAction.Target?.Pokemon?.DisplayName ?? "Unknown"}",

                DamageAction damageAction =>
                    $"{damageAction.Context?.FinalDamage ?? 0} damage to {damageAction.Target?.Pokemon?.DisplayName ?? "Unknown"}" +
                    (damageAction.Context?.IsCritical == true ? " (CRITICAL!)" : ""),

                HealAction healAction =>
                    $"{healAction.Amount} HP to {healAction.Target?.Pokemon?.DisplayName ?? "Unknown"}",

                FaintAction faintAction =>
                    $"{faintAction.Target?.Pokemon?.DisplayName ?? "Unknown"} fainted",

                ApplyStatusAction statusAction =>
                    $"{statusAction.Status} to {statusAction.Target?.Pokemon?.DisplayName ?? "Unknown"}",

                StatChangeAction statAction =>
                    $"{statAction.Stat} {statAction.Change:+0;-0;+0} to {statAction.Target?.Pokemon?.DisplayName ?? "Unknown"}",

                SwitchAction switchAction =>
                    $"Switched to {switchAction.NewPokemon?.DisplayName ?? "Unknown"}" +
                    (switchAction.OldPokemon != null ? $" (was {switchAction.OldPokemon.DisplayName})" : ""),

                SetWeatherAction weatherAction =>
                    $"Weather changed to {weatherAction.WeatherData?.Name ?? weatherAction.Weather.ToString()}",

                SetTerrainAction terrainAction =>
                    $"Terrain changed to {terrainAction.TerrainData?.Name ?? terrainAction.Terrain.ToString()}",

                MessageAction messageAction =>
                    messageAction.Message ?? "Message",

                _ => action.GetType().Name
            };
        }
    }
}
