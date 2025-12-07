using System.Collections.Generic;
using System.Linq;
using PokemonUltimate.Combat;
using PokemonUltimate.Combat.Actions;
using PokemonUltimate.Combat.Damage;
using PokemonUltimate.Combat.Statistics;
using PokemonUltimate.Core.Enums;

namespace PokemonUltimate.BattleSimulator.Logging
{
    /// <summary>
    /// Observer that logs detailed battle information including damage, moves, Pokemon status, etc.
    /// </summary>
    /// <remarks>
    /// **Feature**: 6: Development Tools
    /// **Sub-Feature**: 6.8: Interactive Battle Simulator
    /// **Documentation**: See `docs/features/6-development-tools/6.8-interactive-battle-simulator/README.md`
    /// </remarks>
    public class DetailedBattleLoggerObserver : IBattleActionObserver
    {
        private readonly UIBattleLogger _logger;

        public DetailedBattleLoggerObserver(UIBattleLogger logger)
        {
            _logger = logger ?? throw new System.ArgumentNullException(nameof(logger));
        }

        public void OnActionExecuted(BattleAction action, BattleField field, IEnumerable<BattleAction> reactions)
        {
            if (action == null || field == null)
                return;

            // Log detailed information based on action type
            switch (action)
            {
                case UseMoveAction moveAction:
                    LogMoveAction(moveAction, field);
                    break;

                case DamageAction damageAction:
                    LogDamageAction(damageAction, field);
                    break;

                case FaintAction faintAction:
                    LogFaintAction(faintAction, field);
                    break;

                case HealAction healAction:
                    LogHealAction(healAction, field);
                    break;

                case StatChangeAction statAction:
                    LogStatChangeAction(statAction, field);
                    break;

                case ApplyStatusAction statusAction:
                    LogStatusAction(statusAction, field);
                    break;
            }

            // Log reactions if any
            if (reactions != null && reactions.Any())
            {
                foreach (var reaction in reactions)
                {
                    if (reaction is FaintAction)
                    {
                        _logger.LogInfo($"  → Reaction: {reaction.GetType().Name} triggered");
                    }
                }
            }
        }

        private void LogMoveAction(UseMoveAction action, BattleField field)
        {
            if (action.User?.Pokemon == null || action.Target?.Pokemon == null)
                return;

            var user = action.User.Pokemon;
            var target = action.Target.Pokemon;
            var move = action.Move;
            var side = action.User.Side.IsPlayer ? "Player" : "Enemy";

            _logger.LogInfo($"[{side}] {user.DisplayName} used {move.Name} on {target.DisplayName}");
            _logger.LogDebug($"  Move Type: {move.Type}, Category: {move.Category}, Power: {move.Power}, Accuracy: {move.Accuracy}%");
            _logger.LogDebug($"  User HP: {user.CurrentHP}/{user.MaxHP}, Target HP: {target.CurrentHP}/{target.MaxHP}");
        }

        private void LogDamageAction(DamageAction action, BattleField field)
        {
            if (action.User?.Pokemon == null || action.Target?.Pokemon == null)
                return;

            var attacker = action.User.Pokemon;
            var defender = action.Target.Pokemon;
            var context = action.Context;
            var move = context.Move;
            var attackerSide = action.User.Side.IsPlayer ? "Player" : "Enemy";
            var defenderSide = action.Target.Side.IsPlayer ? "Player" : "Enemy";

            int hpBefore = defender.CurrentHP + context.FinalDamage;
            int hpAfter = defender.CurrentHP;
            int damageDealt = hpBefore - hpAfter;

            _logger.LogInfo($"[{attackerSide}] {attacker.DisplayName}'s {move.Name} dealt {damageDealt} damage to [{defenderSide}] {defender.DisplayName}");
            _logger.LogDebug($"  HP: {hpBefore} → {hpAfter} ({defender.CurrentHP}/{defender.MaxHP})");
            _logger.LogDebug($"  Base Damage: {context.BaseDamage}, Multiplier: {context.Multiplier:F2}x, Final: {context.FinalDamage}");
            
            if (context.IsCritical)
                _logger.LogInfo($"  ⚡ CRITICAL HIT!");
            
            if (context.TypeEffectiveness != 1.0f)
            {
                string effectiveness = context.TypeEffectiveness switch
                {
                    0.0f => "No effect",
                    0.25f => "Not very effective (x0.25)",
                    0.5f => "Not very effective (x0.5)",
                    2.0f => "Super effective (x2)",
                    4.0f => "Super effective (x4)",
                    _ => $"Effectiveness: {context.TypeEffectiveness:F2}x"
                };
                _logger.LogInfo($"  {effectiveness}");
            }
        }

        private void LogFaintAction(FaintAction action, BattleField field)
        {
            if (action.Target?.Pokemon == null)
                return;

            var fainted = action.Target.Pokemon;
            var side = action.Target.Side.IsPlayer ? "Player" : "Enemy";
            var attacker = action.User?.Pokemon?.DisplayName ?? "Unknown";

            _logger.LogWarning($"[{side}] {fainted.DisplayName} fainted! (Defeated by {attacker})");
            _logger.LogInfo($"  Final HP: {fainted.CurrentHP}/{fainted.MaxHP}");
        }

        private void LogHealAction(HealAction action, BattleField field)
        {
            if (action.Target?.Pokemon == null)
                return;

            var target = action.Target.Pokemon;
            var side = action.Target.Side.IsPlayer ? "Player" : "Enemy";
            var amount = action.Amount;

            _logger.LogInfo($"[{side}] {target.DisplayName} recovered {amount} HP");
            _logger.LogDebug($"  HP: {target.CurrentHP}/{target.MaxHP}");
        }

        private void LogStatChangeAction(StatChangeAction action, BattleField field)
        {
            if (action.Target?.Pokemon == null)
                return;

            var target = action.Target.Pokemon;
            var side = action.Target.Side.IsPlayer ? "Player" : "Enemy";
            var stat = action.Stat;
            var change = action.Change;
            string direction = change > 0 ? "rose" : "fell";

            _logger.LogInfo($"[{side}] {target.DisplayName}'s {stat} {direction} by {System.Math.Abs(change)} stage(s)");
        }

        private void LogStatusAction(ApplyStatusAction action, BattleField field)
        {
            if (action.Target?.Pokemon == null)
                return;

            var target = action.Target.Pokemon;
            var side = action.Target.Side.IsPlayer ? "Player" : "Enemy";
            var status = action.Status;

            _logger.LogInfo($"[{side}] {target.DisplayName} was afflicted with {status}");
        }

        public void OnTurnStart(int turnNumber, BattleField field)
        {
            _logger.LogBattleEvent("TurnStart", $"Turn {turnNumber} starting");
            
            // Log Pokemon status at turn start
            LogPokemonStatus(field);
        }

        public void OnTurnEnd(int turnNumber, BattleField field)
        {
            _logger.LogBattleEvent("TurnEnd", $"Turn {turnNumber} completed");
            
            // Log Pokemon status at turn end
            LogPokemonStatus(field);
        }

        public void OnBattleStart(BattleField field)
        {
            _logger.LogInfo("=== BATTLE STARTED ===");
            LogPokemonStatus(field);
        }

        public void OnBattleEnd(BattleOutcome outcome, BattleField field)
        {
            _logger.LogInfo($"=== BATTLE ENDED: {outcome} ===");
            LogPokemonStatus(field);
        }

        private void LogPokemonStatus(BattleField field)
        {
            // Log Player side
            foreach (var slot in field.PlayerSide.Slots)
            {
                if (!slot.IsEmpty && slot.Pokemon != null)
                {
                    var p = slot.Pokemon;
                    _logger.LogDebug($"[Player] {p.DisplayName} - HP: {p.CurrentHP}/{p.MaxHP}, Status: {p.Status}");
                }
            }

            // Log Enemy side
            foreach (var slot in field.EnemySide.Slots)
            {
                if (!slot.IsEmpty && slot.Pokemon != null)
                {
                    var p = slot.Pokemon;
                    _logger.LogDebug($"[Enemy] {p.DisplayName} - HP: {p.CurrentHP}/{p.MaxHP}, Status: {p.Status}");
                }
            }
        }
    }
}

