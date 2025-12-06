using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PokemonUltimate.Combat;
using PokemonUltimate.Combat.Actions;
using PokemonUltimate.Combat.AI;
using PokemonUltimate.Combat.Damage;
using PokemonUltimate.Combat.Factories;
using PokemonUltimate.Combat.Helpers;
using PokemonUltimate.Content.Catalogs.Pokemon;
using PokemonUltimate.Core.Blueprints;
using PokemonUltimate.Core.Factories;
using PokemonUltimate.Core.Instances;

namespace PokemonUltimate.BattleDebugger
{
    /// <summary>
    /// Battle Debugger - Battle generator with detailed debug information.
    /// Perfect for debugging battle mechanics and verifying correctness.
    /// </summary>
    class Program
    {
        // ========== CONFIGURACIÓN - Edita aquí lo que quieres probar ==========

        /// <summary>
        /// Pokemon del jugador que quieres probar (null = aleatorio)
        /// </summary>
        static PokemonSpeciesData? PlayerPokemon = PokemonCatalog.Abra;

        /// <summary>
        /// Pokemon enemigo contra el que probar (null = aleatorio)
        /// </summary>
        static PokemonSpeciesData? EnemyPokemon = PokemonCatalog.Bulbasaur;

        /// <summary>
        /// Nivel de ambos Pokemon
        /// </summary>
        static int Level = 50;

        /// <summary>
        /// Número de batallas a ejecutar
        /// </summary>
        static int NumberOfBattles = 100;

        /// <summary>
        /// Modo de salida: true = detallado (muestra todo), false = resumen (solo resultados)
        /// </summary>
        static bool DetailedOutput = false;

        // ========================================================================

        private static readonly Random _random = new Random();
        private static readonly List<PokemonSpeciesData> _availablePokemon = PokemonCatalog.All.ToList();

        // Pokemon seleccionados para todas las batallas (se establecen al inicio si son null)
        private static PokemonSpeciesData? _selectedPlayerPokemon;
        private static PokemonSpeciesData? _selectedEnemyPokemon;

        static async Task Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            PrintHeader();

            // Seleccionar Pokemon aleatorios una sola vez si son null
            _selectedPlayerPokemon = PlayerPokemon ?? _availablePokemon[_random.Next(_availablePokemon.Count)];
            _selectedEnemyPokemon = EnemyPokemon ?? _availablePokemon[_random.Next(_availablePokemon.Count)];

            Console.ForegroundColor = ConsoleColor.Cyan;
            if (PlayerPokemon != null && EnemyPokemon != null)
            {
                Console.WriteLine($"Probando: {PlayerPokemon.Name} vs {EnemyPokemon.Name}");
                Console.WriteLine($"Nivel: {Level} | Batallas: {NumberOfBattles}");
            }
            else
            {
                Console.WriteLine("Modo aleatorio - Pokemon seleccionados para todas las batallas:");
                Console.WriteLine($"  Jugador: {_selectedPlayerPokemon.Name}");
                Console.WriteLine($"  Enemigo: {_selectedEnemyPokemon.Name}");
                Console.WriteLine($"Nivel: {Level} | Batallas: {NumberOfBattles}");
            }
            Console.ResetColor();
            Console.WriteLine();

            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine("Este debugger muestra información detallada:");
            Console.WriteLine("  • Cálculos completos de daño (base, multiplicadores, final)");
            Console.WriteLine("  • Efectividad de tipos");
            Console.WriteLine("  • Verificación de STAB");
            Console.WriteLine("  • Estadísticas y modificadores");
            Console.WriteLine("  • Movimientos y condiciones de campo");
            Console.ResetColor();
            Console.WriteLine();

            var playerWins = 0;
            var enemyWins = 0;
            var draws = 0;
            var playerName = _selectedPlayerPokemon?.Name ?? "Player";
            var enemyName = _selectedEnemyPokemon?.Name ?? "Enemy";

            // Diccionario para rastrear movimientos usados: PokemonName -> (MoveName -> Count)
            var moveUsageStats = new Dictionary<string, Dictionary<string, int>>();

            // Diccionario para rastrear efectos de estado causados: PokemonName -> (StatusName -> Count)
            var statusEffectStats = new Dictionary<string, Dictionary<string, int>>();

            for (int i = 0; i < NumberOfBattles; i++)
            {
                if (DetailedOutput || NumberOfBattles > 1)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"\n{new string('═', 100)}");
                    Console.WriteLine($"BATALLA #{i + 1}");
                    Console.WriteLine(new string('═', 100));
                    Console.ResetColor();
                }

                var result = await RunBattle(moveUsageStats, statusEffectStats);

                // Contar victorias para estadísticas
                if (result.Outcome == BattleOutcome.Victory)
                {
                    playerWins++;
                }
                else if (result.Outcome == BattleOutcome.Defeat)
                {
                    enemyWins++;
                }
                else
                {
                    draws++; // Empates o batallas incompletas
                }

                if (i < NumberOfBattles - 1 && DetailedOutput)
                {
                    Console.ForegroundColor = ConsoleColor.Gray;
                    Console.WriteLine("\nPresiona cualquier tecla para continuar a la siguiente batalla...");
                    Console.ResetColor();
                    Console.ReadKey(true);
                }
            }

            // Mostrar resumen si hay múltiples batallas
            if (NumberOfBattles > 1)
            {
                Console.WriteLine("\n═══════════════════════════════════════════════════════════════");
                Console.WriteLine("RESUMEN:");
                Console.WriteLine($"  {playerName} ganó: {playerWins} ({playerWins * 100.0 / NumberOfBattles:F1}%)");
                Console.WriteLine($"  {enemyName} ganó: {enemyWins} ({enemyWins * 100.0 / NumberOfBattles:F1}%)");
                if (draws > 0)
                {
                    Console.WriteLine($"  Empates: {draws} ({draws * 100.0 / NumberOfBattles:F1}%)");
                }
                Console.WriteLine($"  Total: {playerWins + enemyWins + draws} / {NumberOfBattles}");
            }

            // Mostrar estadísticas de movimientos
            if (moveUsageStats.Count > 0)
            {
                Console.WriteLine("\n═══════════════════════════════════════════════════════════════");
                Console.WriteLine("ESTADÍSTICAS DE MOVIMIENTOS MÁS USADOS:");
                Console.WriteLine("═══════════════════════════════════════════════════════════════");

                foreach (var pokemonStats in moveUsageStats.OrderByDescending(p => p.Value.Values.Sum()))
                {
                    var pokemonName = pokemonStats.Key;
                    var moves = pokemonStats.Value.OrderByDescending(m => m.Value).Take(5).ToList();
                    var totalMoves = pokemonStats.Value.Values.Sum();

                    Console.WriteLine($"\n{pokemonName} (Total movimientos usados: {totalMoves}):");
                    foreach (var move in moves)
                    {
                        var percentage = (move.Value * 100.0) / totalMoves;
                        Console.WriteLine($"  {move.Key}: {move.Value} veces ({percentage:F1}%)");
                    }
                }
            }

            // Mostrar estadísticas de efectos de estado causados
            if (statusEffectStats.Count > 0)
            {
                Console.WriteLine("\n═══════════════════════════════════════════════════════════════");
                Console.WriteLine("ESTADÍSTICAS DE EFECTOS DE ESTADO CAUSADOS:");
                Console.WriteLine("═══════════════════════════════════════════════════════════════");

                foreach (var pokemonStats in statusEffectStats.OrderByDescending(p => p.Value.Values.Sum()))
                {
                    var pokemonName = pokemonStats.Key;
                    var statuses = pokemonStats.Value.OrderByDescending(s => s.Value).ToList();
                    var totalStatuses = pokemonStats.Value.Values.Sum();

                    Console.WriteLine($"\n{pokemonName} (Total efectos causados: {totalStatuses}):");
                    foreach (var status in statuses)
                    {
                        var percentage = (status.Value * 100.0) / totalStatuses;
                        Console.WriteLine($"  {status.Key}: {status.Value} veces ({percentage:F1}%)");
                    }
                }
            }

            Console.WriteLine("\n" + new string('═', 100));
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("¡Todas las batallas completadas!");
            Console.ResetColor();
            Console.WriteLine("Presiona cualquier tecla para salir...");
            Console.ReadKey();
        }

        static void PrintHeader()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(new string('═', 100));
            Console.WriteLine("  POKEMON ULTIMATE - BATTLE DEBUGGER");
            Console.WriteLine("  Battle Generator with Detailed Debug Information");
            Console.WriteLine(new string('═', 100));
            Console.ResetColor();
            Console.WriteLine();
        }

        static async Task<BattleResult> RunBattle(Dictionary<string, Dictionary<string, int>> moveUsageStats, Dictionary<string, Dictionary<string, int>> statusEffectStats)
        {
            // Generate parties (use selected Pokemon - same for all battles if random was selected)
            var playerParty = new[] {
                PokemonFactory.Create(_selectedPlayerPokemon!, Level)
            };
            var enemyParty = new[] {
                PokemonFactory.Create(_selectedEnemyPokemon!, Level)
            };

            // Create AI
            var playerAI = new RandomAI();
            var enemyAI = new RandomAI();

            // Create debug view
            var view = new DebugBattleView();

            // Create engine
            var engine = CombatEngineFactory.Create();
            var rules = new BattleRules { PlayerSlots = 1, EnemySlots = 1 };

            // Initialize
            engine.Initialize(rules, playerParty, enemyParty, playerAI, enemyAI, view);
            view.SetField(engine.Field);

            // Display initial state
            if (DetailedOutput)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("\n[BATTLE SETUP]");
                Console.ResetColor();
                var playerSpecies = playerParty[0].Species;
                var enemySpecies = enemyParty[0].Species;
                Console.WriteLine($"Player: {playerParty[0].DisplayName} Lv.{playerParty[0].Level} ({playerSpecies.PrimaryType}" +
                                (playerSpecies.SecondaryType.HasValue ? $"/{playerSpecies.SecondaryType.Value}" : "") + ")");
                Console.WriteLine($"Enemy:  {enemyParty[0].DisplayName} Lv.{enemyParty[0].Level} ({enemySpecies.PrimaryType}" +
                                (enemySpecies.SecondaryType.HasValue ? $"/{enemySpecies.SecondaryType.Value}" : "") + ")");

                view.DisplayBattleState();
            }

            // Run battle with debug interception
            var result = await RunBattleWithDebug(engine, view, moveUsageStats, statusEffectStats);

            // Display result
            if (DetailedOutput)
            {
                DisplayBattleResult(result, engine.Field);
            }
            else
            {
                // Modo resumen: solo mostrar resultado simple
                var playerSpecies = playerParty[0].Species;
                var enemySpecies = enemyParty[0].Species;

                if (result.Outcome == BattleOutcome.Victory)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"  ✓ {playerParty[0].DisplayName} ganó en {result.TurnsTaken} turnos");
                }
                else if (result.Outcome == BattleOutcome.Defeat)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"  ✗ {enemyParty[0].DisplayName} ganó en {result.TurnsTaken} turnos");
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"  ? Empate o batalla incompleta ({result.TurnsTaken} turnos)");
                }
                Console.ResetColor();
            }

            return result;
        }

        static async Task ProcessQueueWithTracking(BattleQueue queue, BattleField field, IBattleView view, Dictionary<string, Dictionary<string, int>> statusEffectStats)
        {
            int iterationCount = 0;
            const int maxIterations = 1000;

            // Use reflection to access private _queue field
            var queueField = typeof(BattleQueue).GetField("_queue", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (queueField == null)
            {
                // Fallback: use standard ProcessQueue
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

                // Get first action
                var action = linkedList.First.Value;
                linkedList.RemoveFirst();

                // Track ApplyStatusAction before executing
                if (action is ApplyStatusAction statusAction && statusAction.User?.Pokemon != null && statusAction.Status != Core.Enums.PersistentStatus.None)
                {
                    var pokemonName = statusAction.User.Pokemon.Species.Name;
                    var statusName = statusAction.Status.ToString();

                    if (!statusEffectStats.ContainsKey(pokemonName))
                    {
                        statusEffectStats[pokemonName] = new Dictionary<string, int>();
                    }

                    if (!statusEffectStats[pokemonName].ContainsKey(statusName))
                    {
                        statusEffectStats[pokemonName][statusName] = 0;
                    }

                    statusEffectStats[pokemonName][statusName]++;
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

        static async Task<BattleResult> RunBattleWithDebug(CombatEngine engine, DebugBattleView view, Dictionary<string, Dictionary<string, int>> moveUsageStats, Dictionary<string, Dictionary<string, int>> statusEffectStats)
        {
            int turnCount = 0;
            const int maxTurns = 1000;
            var outcome = BattleOutcome.Ongoing;

            // Create a custom battle queue processor that intercepts damage actions
            while (outcome == BattleOutcome.Ongoing && turnCount < maxTurns)
            {
                view.NextTurn();

                if (DetailedOutput)
                {
                    view.DisplayBattleState();
                }

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

                // Display turn order
                if (DetailedOutput)
                {
                    DisplayTurnOrder(sortedActions, helpers.TurnOrderResolver, engine.Field);
                }

                // Process actions with debug interception
                foreach (var action in sortedActions)
                {
                    // Capture status before action to detect changes
                    var volatileStatusBefore = new Dictionary<BattleSlot, Core.Enums.VolatileStatus>();
                    var persistentStatusBefore = new Dictionary<BattleSlot, Core.Enums.PersistentStatus>();
                    foreach (var slot in engine.Field.GetAllActiveSlots())
                    {
                        volatileStatusBefore[slot] = slot.VolatileStatus;
                        persistentStatusBefore[slot] = slot.Pokemon?.Status ?? Core.Enums.PersistentStatus.None;
                    }

                    // Track move usage
                    if (action is UseMoveAction moveAction && moveAction.User?.Pokemon != null)
                    {
                        var pokemonName = moveAction.User.Pokemon.Species.Name;
                        var moveName = moveAction.Move.Name;

                        if (!moveUsageStats.ContainsKey(pokemonName))
                        {
                            moveUsageStats[pokemonName] = new Dictionary<string, int>();
                        }

                        if (!moveUsageStats[pokemonName].ContainsKey(moveName))
                        {
                            moveUsageStats[pokemonName][moveName] = 0;
                        }

                        moveUsageStats[pokemonName][moveName]++;
                    }

                    // Capture HP before action
                    view.CaptureHpBefore(action);

                    // Display action info
                    if (DetailedOutput)
                    {
                        DisplayActionInfo(action);
                    }

                    // Execute logic
                    var reactions = action.ExecuteLogic(engine.Field);
                    var reactionList = reactions?.ToList() ?? new List<BattleAction>();

                    // Track ApplyStatusAction directly from reactions (this is the correct way)
                    foreach (var reaction in reactionList)
                    {
                        if (reaction is ApplyStatusAction statusAction && statusAction.User?.Pokemon != null && statusAction.Status != Core.Enums.PersistentStatus.None)
                        {
                            var pokemonName = statusAction.User.Pokemon.Species.Name;
                            var statusName = statusAction.Status.ToString();

                            if (!statusEffectStats.ContainsKey(pokemonName))
                            {
                                statusEffectStats[pokemonName] = new Dictionary<string, int>();
                            }

                            if (!statusEffectStats[pokemonName].ContainsKey(statusName))
                            {
                                statusEffectStats[pokemonName][statusName] = 0;
                            }

                            statusEffectStats[pokemonName][statusName]++;
                        }
                    }

                    // Track volatile status changes (new volatile statuses added)
                    // Check after executing logic and reactions
                    foreach (var slot in engine.Field.GetAllActiveSlots())
                    {
                        if (volatileStatusBefore.ContainsKey(slot))
                        {
                            var before = volatileStatusBefore[slot];
                            var after = slot.VolatileStatus;
                            var newStatuses = after & ~before; // Only new flags

                            if (newStatuses != Core.Enums.VolatileStatus.None && action.User?.Pokemon != null)
                            {
                                var pokemonName = action.User.Pokemon.Species.Name;

                                // Check each volatile status flag
                                foreach (Core.Enums.VolatileStatus status in Enum.GetValues(typeof(Core.Enums.VolatileStatus)))
                                {
                                    if (status != Core.Enums.VolatileStatus.None && (newStatuses & status) != 0)
                                    {
                                        if (!statusEffectStats.ContainsKey(pokemonName))
                                        {
                                            statusEffectStats[pokemonName] = new Dictionary<string, int>();
                                        }

                                        var statusName = $"Volatile_{status}";
                                        if (!statusEffectStats[pokemonName].ContainsKey(statusName))
                                        {
                                            statusEffectStats[pokemonName][statusName] = 0;
                                        }

                                        statusEffectStats[pokemonName][statusName]++;
                                    }
                                }
                            }
                        }
                    }

                    // Display damage details for any damage actions in reactions
                    if (DetailedOutput)
                    {
                        foreach (var reaction in reactionList)
                        {
                            if (reaction is DamageAction damageAction)
                            {
                                view.DisplayDamageDetails(damageAction);
                            }
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

                // Process queue with interception to track status effects
                await ProcessQueueWithTracking(engine.Queue, engine.Field, view, statusEffectStats);

                turnCount++;

                // Check outcome
                outcome = BattleArbiter.CheckOutcome(engine.Field);
            }

            // Final state
            if (DetailedOutput)
            {
                view.DisplayBattleState();
            }

            return new BattleResult
            {
                Outcome = outcome,
                TurnsTaken = turnCount
            };
        }

        static void DisplayActionInfo(BattleAction action)
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write("\n[ACTION] ");
            Console.ResetColor();

            if (action is UseMoveAction moveAction)
            {
                var user = moveAction.User?.Pokemon?.DisplayName ?? "Unknown";
                var target = moveAction.Target?.Pokemon?.DisplayName ?? "Unknown";
                var move = moveAction.Move.Name;
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine($"{user} uses {move} → {target}");
                Console.ResetColor();
            }
            else if (action is SwitchAction switchAction)
            {
                var oldPokemon = switchAction.Slot?.Pokemon?.DisplayName ?? "Unknown";
                var newPokemon = switchAction.NewPokemon?.DisplayName ?? "Unknown";
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"Switch: {oldPokemon} → {newPokemon}");
                Console.ResetColor();
            }
            else
            {
                Console.WriteLine($"{action.GetType().Name.Replace("Action", "")}");
            }
        }

        static void DisplayTurnOrder(IEnumerable<BattleAction> actions, TurnOrderResolver resolver, BattleField field)
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine("\n[TURN ORDER]");
            Console.ResetColor();

            var actionList = actions.ToList();
            for (int i = 0; i < actionList.Count; i++)
            {
                var action = actionList[i];
                var user = action.User?.Pokemon?.DisplayName ?? "System";
                var priority = resolver.GetPriority(action);
                var speed = action.User != null ? resolver.GetEffectiveSpeed(action.User, field) : 0;

                Console.Write($"  {i + 1}. {user}");

                if (priority != 0)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.Write($" [Priority: {priority:+0;-#}]");
                    Console.ResetColor();
                }

                if (speed > 0)
                {
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.Write($" [Speed: {speed:F0}]");
                    Console.ResetColor();
                }

                if (action is UseMoveAction moveAction)
                {
                    Console.Write($" → {moveAction.Move.Name}");
                }

                Console.WriteLine();
            }
            Console.WriteLine();
        }

        static void DisplayBattleResult(BattleResult result, BattleField field)
        {
            Console.WriteLine("\n" + new string('═', 100));
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("BATTLE RESULT");
            Console.ResetColor();
            Console.WriteLine(new string('═', 100));

            Console.WriteLine($"Outcome: {result.Outcome}");
            Console.WriteLine($"Turns Taken: {result.TurnsTaken}");

            // Show final HP
            Console.WriteLine("\nFinal HP:");
            foreach (var slot in field.PlayerSide.Slots)
            {
                if (!slot.IsEmpty && slot.Pokemon != null)
                {
                    Console.WriteLine($"  Player: {slot.Pokemon.DisplayName} - {slot.Pokemon.CurrentHP}/{slot.Pokemon.MaxHP} HP");
                }
            }
            foreach (var slot in field.EnemySide.Slots)
            {
                if (!slot.IsEmpty && slot.Pokemon != null)
                {
                    Console.WriteLine($"  Enemy:  {slot.Pokemon.DisplayName} - {slot.Pokemon.CurrentHP}/{slot.Pokemon.MaxHP} HP");
                }
            }

            Console.WriteLine(new string('═', 100));
        }

    }
}
