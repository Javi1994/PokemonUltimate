using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PokemonUltimate.Combat;
using PokemonUltimate.Combat.Actions;
using PokemonUltimate.Combat.Damage;
using PokemonUltimate.Combat.Factories;
using PokemonUltimate.Combat.Helpers;
using PokemonUltimate.Core.Enums;
using PokemonUltimate.Core.Factories;
using PokemonUltimate.Core.Instances;

namespace PokemonUltimate.BattleDebugger
{
    /// <summary>
    /// Enhanced console battle view with detailed debug information.
    /// Shows damage calculations, type effectiveness, STAB, stats, and more.
    /// </summary>
    public class DebugBattleView : IBattleView
    {
        private BattleField? _field;
        private int _turnNumber;
        private TurnOrderResolver? _turnOrderResolver;
        private Dictionary<BattleSlot, int> _hpBeforeAction = new();

        public void SetField(BattleField field)
        {
            _field = field;
            _turnNumber = 0;
            var helpers = CombatEngineFactory.CreateHelpers();
            _turnOrderResolver = helpers.TurnOrderResolver;
        }

        public int NextTurn()
        {
            return ++_turnNumber;
        }

        public Task ShowMessage(string message)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine($"  → {message}");
            Console.ResetColor();
            return Task.CompletedTask;
        }

        public Task PlayDamageAnimation(BattleSlot slot)
        {
            return UpdateHPBar(slot);
        }

        public Task UpdateHPBar(BattleSlot slot)
        {
            if (slot?.Pokemon == null || slot.IsEmpty)
                return Task.CompletedTask;

            var pokemon = slot.Pokemon;
            var hpPercent = (double)pokemon.CurrentHP / pokemon.MaxHP;
            var barLength = 30;
            var filled = (int)(barLength * hpPercent);
            var bar = new string('█', filled) + new string('░', barLength - filled);

            Console.ForegroundColor = hpPercent > 0.5 ? ConsoleColor.Green :
                                     hpPercent > 0.25 ? ConsoleColor.Yellow : ConsoleColor.Red;
            Console.WriteLine($"    {pokemon.DisplayName}: [{bar}] {pokemon.CurrentHP}/{pokemon.MaxHP} HP");
            Console.ResetColor();

            return Task.CompletedTask;
        }

        public Task PlayMoveAnimation(BattleSlot user, BattleSlot target, string moveId)
        {
            return Task.CompletedTask;
        }

        public Task PlayFaintAnimation(BattleSlot slot)
        {
            if (slot?.Pokemon == null)
                return Task.CompletedTask;

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"    ✗ {slot.Pokemon.DisplayName} fainted!");
            Console.ResetColor();
            return Task.CompletedTask;
        }

        public Task PlayStatusAnimation(BattleSlot slot, string statusName)
        {
            if (slot?.Pokemon == null)
                return Task.CompletedTask;

            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine($"    ⚠ {slot.Pokemon.DisplayName} is now {statusName}!");
            Console.ResetColor();
            return Task.CompletedTask;
        }

        public Task ShowStatChange(BattleSlot slot, string statName, int stages)
        {
            if (slot?.Pokemon == null)
                return Task.CompletedTask;

            var direction = stages > 0 ? "rose" : "fell";
            var absStages = Math.Abs(stages);
            var level = absStages >= 2 ? "sharply " : "";

            Console.ForegroundColor = stages > 0 ? ConsoleColor.Green : ConsoleColor.Red;
            Console.WriteLine($"    ↕ {slot.Pokemon.DisplayName}'s {statName} {level}{direction}!");
            Console.ResetColor();
            return Task.CompletedTask;
        }

        public Task PlaySwitchOutAnimation(BattleSlot slot)
        {
            if (slot?.Pokemon == null)
                return Task.CompletedTask;

            Console.WriteLine($"    ← {slot.Pokemon.DisplayName} was withdrawn!");
            return Task.CompletedTask;
        }

        public Task PlaySwitchInAnimation(BattleSlot slot)
        {
            if (slot?.Pokemon == null)
                return Task.CompletedTask;

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"    → Go! {slot.Pokemon.DisplayName}!");
            Console.ResetColor();
            return Task.CompletedTask;
        }

        /// <summary>
        /// Captures HP before an action executes for damage calculation display.
        /// </summary>
        public void CaptureHpBefore(BattleAction action)
        {
            _hpBeforeAction.Clear();
            if (action is DamageAction damageAction && damageAction.Target?.Pokemon != null)
            {
                _hpBeforeAction[damageAction.Target] = damageAction.Target.Pokemon.CurrentHP;
            }
        }

        /// <summary>
        /// Displays detailed damage calculation information.
        /// </summary>
        public void DisplayDamageDetails(DamageAction damageAction)
        {
            if (damageAction.Context == null)
                return;

            var context = damageAction.Context;
            var attacker = context.Attacker.Pokemon;
            var defender = context.Defender.Pokemon;
            var move = context.Move;

            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine("\n    [DAMAGE CALCULATION]");
            Console.ResetColor();

            // Basic info
            Console.WriteLine($"      Move: {move.Name} ({move.Type}) | Power: {move.Power} | Category: {move.Category}");
            Console.WriteLine($"      Attacker: {attacker.DisplayName} Lv.{attacker.Level} ({attacker.Species.PrimaryType}" +
                            (attacker.Species.SecondaryType.HasValue ? $"/{attacker.Species.SecondaryType.Value}" : "") + ")");
            Console.WriteLine($"      Defender: {defender.DisplayName} Lv.{defender.Level} ({defender.Species.PrimaryType}" +
                            (defender.Species.SecondaryType.HasValue ? $"/{defender.Species.SecondaryType.Value}" : ")") + ")");

            // Stats
            var attackStat = move.Category == MoveCategory.Physical ? attacker.Attack : attacker.SpAttack;
            var defenseStat = move.Category == MoveCategory.Physical ? defender.Defense : defender.SpDefense;
            Console.WriteLine($"      Stats: Attack={attackStat:F0} | Defense={defenseStat:F0}");

            // Base damage
            Console.WriteLine($"      Base Damage: {context.BaseDamage:F2}");

            // Multipliers breakdown
            Console.Write("      Multipliers: ");
            var multipliers = new List<string>();

            if (context.IsCritical)
            {
                multipliers.Add("CRIT (1.5x)");
            }

            if (context.IsStab)
            {
                multipliers.Add("STAB (1.5x)");
            }

            if (context.TypeEffectiveness != 1.0f)
            {
                var effText = context.TypeEffectiveness switch
                {
                    0.0f => "IMMUNE (0x)",
                    0.25f => "NOT VERY EFFECTIVE (0.25x)",
                    0.5f => "NOT VERY EFFECTIVE (0.5x)",
                    2.0f => "SUPER EFFECTIVE (2x)",
                    4.0f => "SUPER EFFECTIVE (4x)",
                    _ => $"{context.TypeEffectiveness:F2}x"
                };
                multipliers.Add(effText);
            }

            if (context.RandomFactor != 1.0f)
            {
                multipliers.Add($"RANDOM ({context.RandomFactor:F2}x)");
            }

            if (multipliers.Count == 0)
            {
                Console.WriteLine("None");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine(string.Join(" × ", multipliers));
                Console.ResetColor();
            }

            // Total multiplier
            Console.WriteLine($"      Total Multiplier: {context.Multiplier:F2}x");

            // Final damage
            var hpBefore = _hpBeforeAction.ContainsKey(context.Defender) 
                ? _hpBeforeAction[context.Defender] 
                : defender.CurrentHP;
            var hpAfter = defender.CurrentHP;
            var actualDamage = hpBefore - hpAfter;

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"      Final Damage: {context.FinalDamage} HP");
            Console.ResetColor();
            Console.WriteLine($"      HP Change: {hpBefore} → {hpAfter} ({actualDamage} damage)");

            Console.WriteLine();
        }

        /// <summary>
        /// Displays complete battle state with all debug information.
        /// </summary>
        public void DisplayBattleState()
        {
            if (_field == null)
                return;

            Console.WriteLine("\n" + new string('═', 100));
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"TURN {_turnNumber}");
            Console.ResetColor();
            Console.WriteLine(new string('═', 100));

            // Player side
            Console.WriteLine("\n[PLAYER SIDE]");
            DisplaySideInfo(_field.PlayerSide);

            // Enemy side
            Console.WriteLine("\n[ENEMY SIDE]");
            DisplaySideInfo(_field.EnemySide);

            // Field conditions
            if (_field.Weather != Weather.None || _field.Terrain != Terrain.None)
            {
                Console.WriteLine("\n[FIELD CONDITIONS]");
                if (_field.Weather != Weather.None)
                {
                    Console.WriteLine($"  Weather: {_field.Weather}");
                }
                if (_field.Terrain != Terrain.None)
                {
                    Console.WriteLine($"  Terrain: {_field.Terrain}");
                }
            }

            Console.WriteLine();
        }

        private void DisplaySideInfo(BattleSide side)
        {
            for (int i = 0; i < side.Slots.Count; i++)
            {
                var slot = side.Slots[i];
                if (!slot.IsEmpty && slot.Pokemon != null)
                {
                    var pokemon = slot.Pokemon;
                    var hpPercent = (double)pokemon.CurrentHP / pokemon.MaxHP;
                    var barLength = 30;
                    var filled = (int)(barLength * hpPercent);
                    var bar = new string('█', filled) + new string('░', barLength - filled);

                    Console.ForegroundColor = hpPercent > 0.5 ? ConsoleColor.Green :
                                             hpPercent > 0.25 ? ConsoleColor.Yellow : ConsoleColor.Red;
                    Console.Write($"  Slot {i + 1}: {pokemon.DisplayName} Lv.{pokemon.Level} [{bar}] {pokemon.CurrentHP}/{pokemon.MaxHP} HP");
                    Console.ResetColor();

                    // Types
                    Console.Write($" | Types: {pokemon.Species.PrimaryType}");
                    if (pokemon.Species.SecondaryType.HasValue)
                    {
                        Console.Write($"/{pokemon.Species.SecondaryType.Value}");
                    }

                    // Status
                    if (pokemon.Status != PersistentStatus.None)
                    {
                        Console.ForegroundColor = ConsoleColor.Magenta;
                        Console.Write($" | Status: {pokemon.Status}");
                        Console.ResetColor();
                    }

                    // Stats
                    Console.Write($" | Stats: HP={pokemon.CurrentHP}/{pokemon.MaxHP}");
                    Console.Write($" ATK={pokemon.Attack} DEF={pokemon.Defense}");
                    Console.Write($" SPATK={pokemon.SpAttack} SPDEF={pokemon.SpDefense}");
                    Console.Write($" SPD={pokemon.Speed}");

                    // Stat stages
                    var hasStatStages = false;
                    var statStages = new List<string>();
                    for (int stat = 0; stat < 6; stat++)
                    {
                        var stage = slot.GetStatStage((Stat)stat);
                        if (stage != 0)
                        {
                            statStages.Add($"{(Stat)stat}:{stage:+0;-#}");
                            hasStatStages = true;
                        }
                    }
                    if (hasStatStages)
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.Write($" | Stages: [{string.Join(", ", statStages)}]");
                        Console.ResetColor();
                    }

                    Console.WriteLine();

                    // Moves
                    Console.Write("    Moves: ");
                    var moveNames = pokemon.Moves.Select(m => $"{m.Move.Name} (PP:{m.CurrentPP}/{m.MaxPP})");
                    Console.WriteLine(string.Join(" | ", moveNames));
                }
            }
        }

        // Player input stubs (not used in debugger)
        public Task<BattleActionType> SelectActionType(BattleSlot slot)
        {
            throw new NotImplementedException();
        }

        public Task<MoveInstance> SelectMove(IReadOnlyList<MoveInstance> moves)
        {
            throw new NotImplementedException();
        }

        public Task<BattleSlot> SelectTarget(IReadOnlyList<BattleSlot> validTargets)
        {
            throw new NotImplementedException();
        }

        public Task<PokemonInstance> SelectSwitch(IReadOnlyList<PokemonInstance> availablePokemon)
        {
            throw new NotImplementedException();
        }
    }
}

