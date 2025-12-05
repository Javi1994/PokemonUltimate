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
using PokemonUltimate.Core.Enums;
using PokemonUltimate.Core.Factories;

namespace PokemonUltimate.BattleDemo
{
    /// <summary>
    /// Battle Demo - Visual AI vs AI battle simulator.
    /// Demonstrates the combat system with visual console output.
    /// </summary>
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            PrintHeader();

            // Demo scenarios
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine("You can control the battle pace by pressing ENTER after each turn.");
            Console.WriteLine("Press 'q' during a battle to skip to the next scenario.");
            Console.ResetColor();
            Console.WriteLine();

            await RunScenario1_Basic1v1();
            await RunScenario2_TypeAdvantage();
            await RunScenario3_MultiplePokemon();
            await RunScenario4_RandomVsAlwaysAttack();

            Console.WriteLine("\n" + new string('═', 80));
            Console.WriteLine("All demos completed!");
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }

        static void PrintHeader()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(new string('═', 80));
            Console.WriteLine("  POKEMON ULTIMATE - BATTLE DEMO");
            Console.WriteLine("  AI vs AI Battle Simulator");
            Console.WriteLine(new string('═', 80));
            Console.ResetColor();
            Console.WriteLine();
        }

        /// <summary>
        /// Scenario 1: Basic 1v1 battle - Pikachu vs Charmander
        /// </summary>
        static async Task RunScenario1_Basic1v1()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("\n" + new string('═', 80));
            Console.WriteLine("SCENARIO 1: Basic 1v1 - Pikachu vs Charmander");
            Console.WriteLine(new string('═', 80));
            Console.ResetColor();

            var playerParty = new[] { PokemonFactory.Create(PokemonCatalog.Pikachu, 50) };
            var enemyParty = new[] { PokemonFactory.Create(PokemonCatalog.Charmander, 50) };
            var playerAI = new RandomAI();
            var enemyAI = new RandomAI();
            var view = new ConsoleBattleView();

            var engine = CombatEngineFactory.Create();
            var rules = new BattleRules { PlayerSlots = 1, EnemySlots = 1 };

            engine.Initialize(rules, playerParty, enemyParty, playerAI, enemyAI, view);
            view.SetField(engine.Field);
            view.DisplayBattleState();

            var result = await RunBattleWithDisplay(engine, view);

            DisplayBattleResult(result);

            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write("\nPress any key to continue to next scenario... ");
            Console.ResetColor();
            Console.ReadKey(true);
        }

        /// <summary>
        /// Scenario 2: Type advantage battle - Water vs Fire
        /// </summary>
        static async Task RunScenario2_TypeAdvantage()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("\n" + new string('═', 80));
            Console.WriteLine("SCENARIO 2: Type Advantage - Squirtle vs Charmander");
            Console.WriteLine(new string('═', 80));
            Console.ResetColor();

            var playerParty = new[] { PokemonFactory.Create(PokemonCatalog.Squirtle, 50) };
            var enemyParty = new[] { PokemonFactory.Create(PokemonCatalog.Charmander, 50) };
            var playerAI = new AlwaysAttackAI();
            var enemyAI = new AlwaysAttackAI();
            var view = new ConsoleBattleView();

            var engine = CombatEngineFactory.Create();
            var rules = new BattleRules { PlayerSlots = 1, EnemySlots = 1 };

            engine.Initialize(rules, playerParty, enemyParty, playerAI, enemyAI, view);
            view.SetField(engine.Field);
            view.DisplayBattleState();

            var result = await RunBattleWithDisplay(engine, view);

            DisplayBattleResult(result);

            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write("\nPress any key to continue to next scenario... ");
            Console.ResetColor();
            Console.ReadKey(true);
        }

        /// <summary>
        /// Scenario 3: Multiple Pokemon battle
        /// </summary>
        static async Task RunScenario3_MultiplePokemon()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("\n" + new string('═', 80));
            Console.WriteLine("SCENARIO 3: Multiple Pokemon - 3 vs 3");
            Console.WriteLine(new string('═', 80));
            Console.ResetColor();

            var playerParty = new[]
            {
                PokemonFactory.Create(PokemonCatalog.Pikachu, 50),
                PokemonFactory.Create(PokemonCatalog.Bulbasaur, 50),
                PokemonFactory.Create(PokemonCatalog.Squirtle, 50)
            };
            var enemyParty = new[]
            {
                PokemonFactory.Create(PokemonCatalog.Charmander, 50),
                PokemonFactory.Create(PokemonCatalog.Pikachu, 50),
                PokemonFactory.Create(PokemonCatalog.Bulbasaur, 50)
            };
            var playerAI = new RandomAI();
            var enemyAI = new RandomAI();
            var view = new ConsoleBattleView();

            var engine = CombatEngineFactory.Create();
            var rules = new BattleRules { PlayerSlots = 1, EnemySlots = 1 };

            engine.Initialize(rules, playerParty, enemyParty, playerAI, enemyAI, view);
            view.SetField(engine.Field);
            view.DisplayBattleState();

            var result = await RunBattleWithDisplay(engine, view);

            DisplayBattleResult(result);

            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write("\nPress any key to continue to next scenario... ");
            Console.ResetColor();
            Console.ReadKey(true);
        }

        /// <summary>
        /// Scenario 4: Random AI vs Always Attack AI
        /// </summary>
        static async Task RunScenario4_RandomVsAlwaysAttack()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("\n" + new string('═', 80));
            Console.WriteLine("SCENARIO 4: Strategy Comparison - Random vs Always Attack");
            Console.WriteLine(new string('═', 80));
            Console.ResetColor();

            var playerParty = new[] { PokemonFactory.Create(PokemonCatalog.Pikachu, 50) };
            var enemyParty = new[] { PokemonFactory.Create(PokemonCatalog.Charmander, 50) };
            var playerAI = new RandomAI();
            var enemyAI = new AlwaysAttackAI();
            var view = new ConsoleBattleView();

            var engine = CombatEngineFactory.Create();
            var rules = new BattleRules { PlayerSlots = 1, EnemySlots = 1 };

            engine.Initialize(rules, playerParty, enemyParty, playerAI, enemyAI, view);
            view.SetField(engine.Field);
            view.DisplayBattleState();

            var result = await RunBattleWithDisplay(engine, view);

            DisplayBattleResult(result);

            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write("\nPress any key to continue to next scenario... ");
            Console.ResetColor();
            Console.ReadKey(true);
        }

        /// <summary>
        /// Runs a battle with visual display after each turn.
        /// Waits for user input before executing each turn.
        /// </summary>
        static async Task<BattleResult> RunBattleWithDisplay(CombatEngine engine, ConsoleBattleView view)
        {
            int turnCount = 0;
            const int maxTurns = 1000;
            var outcome = BattleOutcome.Ongoing;

            while (outcome == BattleOutcome.Ongoing && turnCount < maxTurns)
            {
                view.NextTurn();
                view.DisplayBattleState();

                // Wait for user input before executing the turn
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.Write("\nPress ENTER to execute turn, 'q' to quit battle... ");
                Console.ResetColor();

                var key = Console.ReadKey(true);

                if (key.Key == ConsoleKey.Q)
                {
                    Console.WriteLine("\nBattle cancelled by user.");
                    break;
                }

                // Execute the turn with debug information
                await RunTurnWithDebug(engine, view);
                turnCount++;

                // Check outcome
                outcome = BattleArbiter.CheckOutcome(engine.Field);
            }

            // Final state display
            view.DisplayBattleState();

            return new BattleResult
            {
                Outcome = outcome,
                TurnsTaken = turnCount
            };
        }

        /// <summary>
        /// Executes a turn with debug information displayed.
        /// </summary>
        static async Task RunTurnWithDebug(CombatEngine engine, ConsoleBattleView view)
        {
            // 1. Collect actions from all active slots
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

            // 2. Sort by turn order and show debug info
            var turnOrderHelpers = CombatEngineFactory.CreateHelpers();
            var sortedActions = turnOrderHelpers.TurnOrderResolver.SortActions(pendingActions, engine.Field);
            view.DisplayTurnOrderDebug(sortedActions, engine.Field);

            // 3. Enqueue all actions
            engine.Queue.EnqueueRange(sortedActions);

            // 4. Process the queue with debug information
            await ProcessQueueWithDebug(engine, view);
        }

        /// <summary>
        /// Processes the battle queue with debug information displayed.
        /// </summary>
        static async Task ProcessQueueWithDebug(CombatEngine engine, ConsoleBattleView view)
        {
            const int maxIterations = 1000;
            int iterationCount = 0;
            var queue = engine.Queue;
            var field = engine.Field;

            // Use reflection to access private queue, or create a custom processor
            // For simplicity, we'll manually process showing debug info
            while (queue.Count > 0)
            {
                if (iterationCount++ > maxIterations)
                {
                    throw new InvalidOperationException("Battle queue infinite loop detected");
                }

                // Get queue size before processing
                var queueSizeBefore = queue.Count;

                // We need to manually process since BattleQueue doesn't expose internals
                // Let's create a custom processor that mirrors BattleQueue logic
                await ProcessSingleActionWithDebug(queue, field, view, queueSizeBefore);
            }
        }

        /// <summary>
        /// Processes a single action from the queue with debug output.
        /// Uses reflection to access BattleQueue internals.
        /// </summary>
        static async Task ProcessSingleActionWithDebug(BattleQueue queue, BattleField field, IBattleView view, int queueSizeBefore)
        {
            // Access private _queue field via reflection
            var queueField = typeof(BattleQueue).GetField("_queue",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            if (queueField == null)
            {
                // Fallback: just process normally without detailed debug
                await queue.ProcessQueue(field, view);
                return;
            }

            var linkedList = queueField.GetValue(queue) as System.Collections.Generic.LinkedList<BattleAction>;
            if (linkedList == null || linkedList.Count == 0)
                return;

            // Get first action
            var firstNode = linkedList.First;
            if (firstNode == null)
                return;

            var action = firstNode.Value;
            linkedList.RemoveFirst();

            // Show debug info
            var actionName = GetActionName(action);
            var user = action.User?.Pokemon?.DisplayName ?? "System";

            System.Console.ForegroundColor = System.ConsoleColor.DarkGray;
            System.Console.Write($"[DEBUG] Executing: [{actionName}]");
            System.Console.ResetColor();
            System.Console.Write($" by {user}");

            if (action is UseMoveAction moveAction)
            {
                var target = moveAction.Target?.Pokemon?.DisplayName ?? "None";
                System.Console.Write($" → {moveAction.Move.Name} → {target}");
            }

            if (queueSizeBefore > 1)
            {
                System.Console.ForegroundColor = System.ConsoleColor.DarkGray;
                System.Console.Write($" | Queue: {queueSizeBefore - 1} remaining");
                System.Console.ResetColor();
            }

            System.Console.WriteLine();

            // Capture state before execution for damage/heal display
            var hpBefore = CaptureHpBefore(action);

            // Phase 1: Execute Logic
            var reactions = action.ExecuteLogic(field);
            var reactionList = reactions?.ToList() ?? new List<BattleAction>();

            // Show detailed action effects (after logic execution)
            ShowActionEffectDetails(action, field, hpBefore);

            if (reactionList.Count > 0)
            {
                System.Console.ForegroundColor = System.ConsoleColor.Magenta;
                System.Console.WriteLine($"[DEBUG]   → Generated {reactionList.Count} reaction action(s)");
                System.Console.ResetColor();
            }

            // Phase 2: Execute Visual
            await action.ExecuteVisual(view);

            // Phase 3: Insert reactions at front
            if (reactionList.Count > 0)
            {
                // Insert in reverse order to maintain order at front
                for (int i = reactionList.Count - 1; i >= 0; i--)
                {
                    linkedList.AddFirst(reactionList[i]);
                }
            }
        }

        /// <summary>
        /// Captures HP values before action execution for display purposes.
        /// </summary>
        static Dictionary<BattleSlot, int> CaptureHpBefore(BattleAction action)
        {
            var hpMap = new Dictionary<BattleSlot, int>();

            if (action is DamageAction damageAction && damageAction.Target?.Pokemon != null)
            {
                hpMap[damageAction.Target] = damageAction.Target.Pokemon.CurrentHP;
            }
            else if (action is HealAction healAction && healAction.Target?.Pokemon != null)
            {
                hpMap[healAction.Target] = healAction.Target.Pokemon.CurrentHP;
            }

            return hpMap;
        }

        /// <summary>
        /// Shows detailed effect information for an action.
        /// </summary>
        static void ShowActionEffectDetails(BattleAction action, BattleField field, Dictionary<BattleSlot, int> hpBefore)
        {
            switch (action)
            {
                case DamageAction damageAction:
                    var damageTarget = damageAction.Target?.Pokemon;
                    var damage = damageAction.Context.FinalDamage;
                    var targetName = damageTarget?.DisplayName ?? "Unknown";

                    // Get HP before damage was applied
                    var hpBeforeDamage = (damageAction.Target != null && hpBefore.ContainsKey(damageAction.Target))
                        ? hpBefore[damageAction.Target]
                        : (damageTarget?.CurrentHP ?? 0);

                    System.Console.ForegroundColor = System.ConsoleColor.Red;
                    System.Console.Write($"[DEBUG]   → Damage: {damage} HP");
                    System.Console.ResetColor();

                    if (damageTarget != null)
                    {
                        // HP after damage (already applied in ExecuteLogic)
                        var hpAfterDamage = damageTarget.CurrentHP;
                        System.Console.Write($" → {targetName} ({hpBeforeDamage} → {hpAfterDamage})");

                        // Show effectiveness if available
                        var effectiveness = damageAction.Context.TypeEffectiveness;
                        if (effectiveness != 1.0f)
                        {
                            var effectivenessText = effectiveness switch
                            {
                                2.0f => "Super Effective!",
                                4.0f => "Super Effective!",
                                0.5f => "Not Very Effective",
                                0.25f => "Not Very Effective",
                                0.0f => "No Effect",
                                _ => $"{effectiveness:F1}x"
                            };
                            System.Console.ForegroundColor = effectiveness >= 2.0f
                                ? System.ConsoleColor.Green
                                : System.ConsoleColor.Yellow;
                            System.Console.Write($" [{effectivenessText}]");
                            System.Console.ResetColor();
                        }

                        // Show critical hit
                        if (damageAction.Context.IsCritical)
                        {
                            System.Console.ForegroundColor = System.ConsoleColor.Yellow;
                            System.Console.Write(" [CRITICAL HIT!]");
                            System.Console.ResetColor();
                        }

                        // Show STAB
                        if (damageAction.Context.IsStab)
                        {
                            System.Console.ForegroundColor = System.ConsoleColor.Cyan;
                            System.Console.Write(" [STAB]");
                            System.Console.ResetColor();
                        }
                    }
                    System.Console.WriteLine();
                    break;

                case StatChangeAction statAction:
                    var statTarget = statAction.Target?.Pokemon?.DisplayName ?? "Unknown";
                    var statName = statAction.Stat.ToString();
                    var change = statAction.Change;
                    var changeSymbol = change > 0 ? "+" : "";

                    System.Console.ForegroundColor = change > 0 ? System.ConsoleColor.Green : System.ConsoleColor.Red;
                    System.Console.WriteLine($"[DEBUG]   → Stat Change: {statTarget}'s {statName} {changeSymbol}{change} stage(s)");
                    System.Console.ResetColor();
                    break;

                case HealAction healAction:
                    var healTarget = healAction.Target?.Pokemon?.DisplayName ?? "Unknown";
                    var healAmount = healAction.Amount;

                    // Get HP before heal was applied
                    var hpBeforeHeal = (healAction.Target != null && hpBefore.ContainsKey(healAction.Target))
                        ? hpBefore[healAction.Target]
                        : (healAction.Target?.Pokemon?.CurrentHP ?? 0);

                    System.Console.ForegroundColor = System.ConsoleColor.Green;
                    System.Console.Write($"[DEBUG]   → Heal: {healTarget} +{healAmount} HP");
                    System.Console.ResetColor();

                    if (healAction.Target?.Pokemon != null)
                    {
                        var hpAfterHeal = healAction.Target.Pokemon.CurrentHP;
                        System.Console.WriteLine($" ({hpBeforeHeal} → {hpAfterHeal})");
                    }
                    else
                    {
                        System.Console.WriteLine();
                    }
                    break;

                case ApplyStatusAction statusAction:
                    if (statusAction.Status != Core.Enums.PersistentStatus.None)
                    {
                        var statusTarget = statusAction.Target?.Pokemon?.DisplayName ?? "Unknown";
                        System.Console.ForegroundColor = System.ConsoleColor.Magenta;
                        System.Console.WriteLine($"[DEBUG]   → Status: {statusTarget} → {statusAction.Status}");
                        System.Console.ResetColor();
                    }
                    break;

                case FaintAction faintAction:
                    var faintTarget = faintAction.Target?.Pokemon?.DisplayName ?? "Unknown";
                    System.Console.ForegroundColor = System.ConsoleColor.Red;
                    System.Console.WriteLine($"[DEBUG]   → {faintTarget} fainted!");
                    System.Console.ResetColor();
                    break;

                case SwitchAction switchAction:
                    var switchOut = switchAction.Slot?.Pokemon?.DisplayName ?? "Unknown";
                    var switchIn = switchAction.NewPokemon?.DisplayName ?? "Unknown";
                    System.Console.ForegroundColor = System.ConsoleColor.Cyan;
                    System.Console.WriteLine($"[DEBUG]   → Switch: {switchOut} → {switchIn}");
                    System.Console.ResetColor();
                    break;
            }
        }

        /// <summary>
        /// Gets a short name for an action type for debug display.
        /// </summary>
        private static string GetActionName(BattleAction action)
        {
            return action switch
            {
                UseMoveAction moveAction => $"Move({moveAction.Move.Name})",
                DamageAction => "Damage",
                FaintAction => "Faint",
                HealAction => "Heal",
                StatChangeAction => "StatChange",
                ApplyStatusAction => "ApplyStatus",
                SwitchAction => "Switch",
                MessageAction msgAction => $"Msg({msgAction.Message.Substring(0, System.Math.Min(20, msgAction.Message.Length))}...)",
                _ => action.GetType().Name.Replace("Action", "")
            };
        }

        static void DisplayBattleResult(BattleResult result)
        {
            Console.WriteLine("\n" + new string('═', 80));
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("BATTLE RESULT");
            Console.ResetColor();
            Console.WriteLine(new string('═', 80));

            Console.WriteLine($"Outcome: {result.Outcome}");
            Console.WriteLine($"Turns Taken: {result.TurnsTaken}");

            if (result.MvpPokemon != null && result.MvpPokemon.Pokemon != null)
            {
                Console.WriteLine($"MVP: {result.MvpPokemon.Pokemon.DisplayName}");
            }

            if (result.DefeatedEnemies.Count > 0)
            {
                Console.WriteLine($"Defeated Enemies: {result.DefeatedEnemies.Count}");
            }

            Console.WriteLine(new string('═', 80));
        }
    }
}

