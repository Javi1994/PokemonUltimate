using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PokemonUltimate.Combat;
using PokemonUltimate.Combat.Actions;
using PokemonUltimate.Combat.Factories;
using PokemonUltimate.Combat.Helpers;
using PokemonUltimate.Core.Instances;

namespace PokemonUltimate.BattleDemo
{
    /// <summary>
    /// Console implementation of IBattleView for visual battle display.
    /// Shows battle state, messages, and animations in the console.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.7: Integration
    /// **Documentation**: See `docs/features/2-combat-system/2.7-integration/architecture.md`
    /// </remarks>
    public class ConsoleBattleView : IBattleView
    {
        private BattleField? _field;
        private int _turnNumber;
        private TurnOrderResolver? _turnOrderResolver;

        /// <summary>
        /// Sets the battle field for display purposes.
        /// </summary>
        public void SetField(BattleField field)
        {
            _field = field;
            _turnNumber = 0;

            // Create TurnOrderResolver for debug display
            var helpers = CombatEngineFactory.CreateHelpers();
            _turnOrderResolver = helpers.TurnOrderResolver;
        }

        /// <summary>
        /// Increments and returns the current turn number.
        /// </summary>
        public int NextTurn()
        {
            return ++_turnNumber;
        }

        /// <summary>
        /// Displays a message to the player.
        /// </summary>
        public Task ShowMessage(string message)
        {
            Console.WriteLine($"  → {message}");
            return Task.CompletedTask;
        }

        /// <summary>
        /// Plays a damage animation for a slot.
        /// </summary>
        public Task PlayDamageAnimation(BattleSlot slot)
        {
            if (slot?.Pokemon == null || slot.IsEmpty)
                return Task.CompletedTask;

            var pokemon = slot.Pokemon;
            var hpPercent = (double)pokemon.CurrentHP / pokemon.MaxHP;
            var barLength = 20;
            var filled = (int)(barLength * hpPercent);
            var bar = new string('█', filled) + new string('░', barLength - filled);

            Console.ForegroundColor = hpPercent > 0.5 ? ConsoleColor.Green :
                                     hpPercent > 0.25 ? ConsoleColor.Yellow : ConsoleColor.Red;
            Console.WriteLine($"    {pokemon.DisplayName}: [{bar}] {pokemon.CurrentHP}/{pokemon.MaxHP} HP");
            Console.ResetColor();

            return Task.CompletedTask;
        }

        /// <summary>
        /// Updates the HP bar for a slot.
        /// </summary>
        public Task UpdateHPBar(BattleSlot slot)
        {
            return PlayDamageAnimation(slot);
        }

        /// <summary>
        /// Plays a move animation.
        /// </summary>
        public Task PlayMoveAnimation(BattleSlot user, BattleSlot target, string moveId)
        {
            // Animation is handled by ShowMessage, so this is just a placeholder
            return Task.CompletedTask;
        }

        /// <summary>
        /// Plays the faint animation for a Pokemon.
        /// </summary>
        public Task PlayFaintAnimation(BattleSlot slot)
        {
            if (slot?.Pokemon == null)
                return Task.CompletedTask;

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"    {slot.Pokemon.DisplayName} fainted!");
            Console.ResetColor();
            return Task.CompletedTask;
        }

        /// <summary>
        /// Plays an animation for a status effect being applied.
        /// </summary>
        public Task PlayStatusAnimation(BattleSlot slot, string statusName)
        {
            if (slot?.Pokemon == null)
                return Task.CompletedTask;

            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine($"    {slot.Pokemon.DisplayName} is now {statusName}!");
            Console.ResetColor();
            return Task.CompletedTask;
        }

        /// <summary>
        /// Updates the display after a stat change.
        /// </summary>
        public Task ShowStatChange(BattleSlot slot, string statName, int stages)
        {
            if (slot?.Pokemon == null)
                return Task.CompletedTask;

            var direction = stages > 0 ? "rose" : "fell";
            var absStages = Math.Abs(stages);
            var level = absStages >= 2 ? "sharply " : "";

            Console.ForegroundColor = stages > 0 ? ConsoleColor.Green : ConsoleColor.Red;
            Console.WriteLine($"    {slot.Pokemon.DisplayName}'s {statName} {level}{direction}!");
            Console.ResetColor();
            return Task.CompletedTask;
        }

        /// <summary>
        /// Plays a switch-out animation.
        /// </summary>
        public Task PlaySwitchOutAnimation(BattleSlot slot)
        {
            if (slot?.Pokemon == null)
                return Task.CompletedTask;

            Console.WriteLine($"    {slot.Pokemon.DisplayName} was withdrawn!");
            return Task.CompletedTask;
        }

        /// <summary>
        /// Plays a switch-in animation.
        /// </summary>
        public Task PlaySwitchInAnimation(BattleSlot slot)
        {
            if (slot?.Pokemon == null)
                return Task.CompletedTask;

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"    Go! {slot.Pokemon.DisplayName}!");
            Console.ResetColor();
            return Task.CompletedTask;
        }

        /// <summary>
        /// Displays the current battle state.
        /// </summary>
        public void DisplayBattleState()
        {
            if (_field == null)
                return;

            Console.WriteLine("\n" + new string('═', 80));
            Console.WriteLine($"TURN {_turnNumber}");
            Console.WriteLine(new string('═', 80));

            // Player side
            Console.WriteLine("\n[PLAYER]");
            for (int i = 0; i < _field.PlayerSide.Slots.Count; i++)
            {
                var slot = _field.PlayerSide.Slots[i];
                if (!slot.IsEmpty && slot.Pokemon != null)
                {
                    var pokemon = slot.Pokemon;
                    var hpPercent = (double)pokemon.CurrentHP / pokemon.MaxHP;
                    var barLength = 30;
                    var filled = (int)(barLength * hpPercent);
                    var bar = new string('█', filled) + new string('░', barLength - filled);

                    Console.ForegroundColor = hpPercent > 0.5 ? ConsoleColor.Green :
                                             hpPercent > 0.25 ? ConsoleColor.Yellow : ConsoleColor.Red;
                    Console.Write($"  {pokemon.DisplayName} Lv.{pokemon.Level} [{bar}] {pokemon.CurrentHP}/{pokemon.MaxHP}");

                    if (pokemon.Status != Core.Enums.PersistentStatus.None)
                    {
                        Console.Write($" [{pokemon.Status}]");
                    }

                    Console.WriteLine();
                    Console.ResetColor();
                }
            }

            // Enemy side
            Console.WriteLine("\n[ENEMY]");
            for (int i = 0; i < _field.EnemySide.Slots.Count; i++)
            {
                var slot = _field.EnemySide.Slots[i];
                if (!slot.IsEmpty && slot.Pokemon != null)
                {
                    var pokemon = slot.Pokemon;
                    var hpPercent = (double)pokemon.CurrentHP / pokemon.MaxHP;
                    var barLength = 30;
                    var filled = (int)(barLength * hpPercent);
                    var bar = new string('█', filled) + new string('░', barLength - filled);

                    Console.ForegroundColor = hpPercent > 0.5 ? ConsoleColor.Green :
                                             hpPercent > 0.25 ? ConsoleColor.Yellow : ConsoleColor.Red;
                    Console.Write($"  {pokemon.DisplayName} Lv.{pokemon.Level} [{bar}] {pokemon.CurrentHP}/{pokemon.MaxHP}");

                    if (pokemon.Status != Core.Enums.PersistentStatus.None)
                    {
                        Console.Write($" [{pokemon.Status}]");
                    }

                    Console.WriteLine();
                    Console.ResetColor();
                }
            }

            Console.WriteLine();
        }

        /// <summary>
        /// Displays debug information about turn order resolution.
        /// Shows priority, speed, and final order of actions.
        /// </summary>
        public void DisplayTurnOrderDebug(IEnumerable<BattleAction> actions, BattleField field)
        {
            if (actions == null || !actions.Any())
                return;

            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine("\n[DEBUG] Turn Order Resolution:");
            Console.ResetColor();

            // Ensure we have a TurnOrderResolver instance
            if (_turnOrderResolver == null)
            {
                var helpers = CombatEngineFactory.CreateHelpers();
                _turnOrderResolver = helpers.TurnOrderResolver;
            }

            var actionList = actions.ToList();
            for (int i = 0; i < actionList.Count; i++)
            {
                var action = actionList[i];
                var actionName = GetActionName(action);
                var user = action.User?.Pokemon?.DisplayName ?? "System";
                var priority = _turnOrderResolver.GetPriority(action);
                var speed = action.User != null ? _turnOrderResolver.GetEffectiveSpeed(action.User, field) : 0;

                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.Write($"  {i + 1}. [{actionName}] ");
                Console.ResetColor();
                Console.Write($"User: {user}");

                if (priority != 0)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.Write($" | Priority: {priority:+0;-#}");
                    Console.ResetColor();
                }

                if (speed > 0)
                {
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.Write($" | Speed: {speed:F0}");
                    Console.ResetColor();
                }

                // Show target for UseMoveAction
                if (action is UseMoveAction moveAction)
                {
                    var target = moveAction.Target?.Pokemon?.DisplayName ?? "None";
                    var moveName = moveAction.Move.Name;
                    Console.Write($" | Move: {moveName} → {target}");
                }

                Console.WriteLine();
            }
            Console.WriteLine();
        }

        /// <summary>
        /// Displays debug information about action queue processing.
        /// Shows which action is being executed and how many reactions it generated.
        /// </summary>
        public void DisplayActionQueueDebug(BattleAction action, int reactionCount, int queueSize)
        {
            var actionName = GetActionName(action);
            var user = action.User?.Pokemon?.DisplayName ?? "System";

            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write($"[DEBUG] Executing: [{actionName}]");
            Console.ResetColor();
            Console.Write($" by {user}");

            if (reactionCount > 0)
            {
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.Write($" → {reactionCount} reaction(s)");
                Console.ResetColor();
            }

            if (queueSize > 0)
            {
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.Write($" | Queue: {queueSize} remaining");
                Console.ResetColor();
            }

            Console.WriteLine();
        }

        /// <summary>
        /// Gets a short name for an action type for debug display.
        /// </summary>
        private string GetActionName(BattleAction action)
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
                MessageAction msgAction => $"Msg({msgAction.Message.Substring(0, Math.Min(20, msgAction.Message.Length))}...)",
                _ => action.GetType().Name.Replace("Action", "")
            };
        }

        // ========== Player Input Methods (Stubs for AI vs AI demo) ==========

        /// <summary>
        /// Stub implementation - not used in AI vs AI demo.
        /// </summary>
        public Task<BattleActionType> SelectActionType(BattleSlot slot)
        {
            throw new NotImplementedException("Player input not implemented in AI vs AI demo");
        }

        /// <summary>
        /// Stub implementation - not used in AI vs AI demo.
        /// </summary>
        public Task<MoveInstance> SelectMove(IReadOnlyList<MoveInstance> moves)
        {
            throw new NotImplementedException("Player input not implemented in AI vs AI demo");
        }

        /// <summary>
        /// Stub implementation - not used in AI vs AI demo.
        /// </summary>
        public Task<BattleSlot> SelectTarget(IReadOnlyList<BattleSlot> validTargets)
        {
            throw new NotImplementedException("Player input not implemented in AI vs AI demo");
        }

        /// <summary>
        /// Stub implementation - not used in AI vs AI demo.
        /// </summary>
        public Task<PokemonInstance> SelectSwitch(IReadOnlyList<PokemonInstance> availablePokemon)
        {
            throw new NotImplementedException("Player input not implemented in AI vs AI demo");
        }
    }
}

