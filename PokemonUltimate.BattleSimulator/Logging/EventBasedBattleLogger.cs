using System;
using System.Collections.Generic;
using System.Linq;
using PokemonUltimate.BattleSimulator.Helpers;
using PokemonUltimate.Combat;
using PokemonUltimate.Combat.Events;
using PokemonUltimate.Combat.Logging;
using PokemonUltimate.Core.Infrastructure.Localization;
using PokemonUltimate.Core.Domain.Instances;
using PokemonUltimate.Core.Services;
using PokemonInstance = PokemonUltimate.Core.Domain.Instances.Pokemon.PokemonInstance;

namespace PokemonUltimate.BattleSimulator.Logging
{
    /// <summary>
    /// Logger that converts battle events into log messages.
    /// Replaces hardcoded log messages with event-driven logging.
    /// </summary>
    /// <remarks>
    /// **Feature**: 6: Development Tools
    /// **Sub-Feature**: 6.8: Interactive Battle Simulator
    /// **Documentation**: See `docs/features/6-development-tools/6.8-interactive-battle-simulator/README.md`
    /// </remarks>
    public class EventBasedBattleLogger : IBattleEventSubscriber
    {
        private readonly IBattleLogger _logger;
        private readonly ILocalizationProvider _localizationProvider;
        private int _currentTurn = 0;
        private Dictionary<string, string>? _pokemonNameMapping;

        public EventBasedBattleLogger(IBattleLogger logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _localizationProvider = LocalizationService.Instance;

            // Try to get name mapping from UIBattleLogger if available
            if (logger is UIBattleLogger uiLogger)
            {
                _pokemonNameMapping = uiLogger.GetPokemonNameMapping();
            }
        }

        private string GetPokemonDisplayName(PokemonInstance? pokemon)
        {
            if (pokemon == null)
                return "Unknown";

            return PokemonNameMapper.GetDisplayName(pokemon, _pokemonNameMapping);
        }

        public void OnBattleEvent(BattleEvent @event)
        {
            if (@event == null)
                return;

            _currentTurn = @event.TurnNumber;

            switch (@event.EventType)
            {
                case BattleEventType.PokemonFainted:
                    LogPokemonFainted(@event);
                    break;

                case BattleEventType.PokemonSwitched:
                    LogPokemonSwitched(@event);
                    break;

                case BattleEventType.AIDecisionMade:
                    LogAIDecision(@event);
                    break;

                case BattleEventType.MoveUsed:
                    LogMoveUsed(@event);
                    break;

                case BattleEventType.DamageDealt:
                    LogDamageDealt(@event);
                    break;

                case BattleEventType.CriticalHit:
                    _logger.LogInfo("  ⚡ CRITICAL HIT!");
                    break;

                case BattleEventType.TurnStarted:
                    _logger.LogBattleEvent("TurnStart", $"Turn {@event.TurnNumber} starting");
                    break;

                case BattleEventType.TurnEnded:
                    _logger.LogBattleEvent("TurnEnd", $"Turn {@event.TurnNumber} completed");
                    break;

                case BattleEventType.BattleStarted:
                    LogBattleStarted(@event);
                    break;

                case BattleEventType.BattleEnded:
                    LogBattleEnded(@event);
                    break;

                case BattleEventType.ActionCollected:
                    LogActionCollected(@event);
                    break;

                case BattleEventType.ActionsSorted:
                    LogActionsSorted(@event);
                    break;

                case BattleEventType.ActionsGenerated:
                    LogActionsGenerated(@event);
                    break;

                case BattleEventType.MoveMissed:
                    LogMoveMissed(@event);
                    break;

                case BattleEventType.PokemonStatusApplied:
                    LogStatusApplied(@event);
                    break;
            }
        }

        private void LogPokemonFainted(BattleEvent @event)
        {
            if (@event.Pokemon == null)
                return;

            var sideName = @event.IsPlayerSide ? "Player" : "Enemy";
            var attacker = GetPokemonDisplayName(@event.Data.DefeatedBy);

            _logger.LogWarning($"[{sideName}] {GetPokemonDisplayName(@event.Pokemon)} fainted! (Defeated by {attacker})");
            _logger.LogInfo($"  Final HP: {@event.Pokemon.CurrentHP}/{@event.Pokemon.MaxHP}");

            // Log team status from event data
            if (@event.Data.RemainingPokemon >= 0 && @event.Data.TotalPokemon > 0)
            {
                _logger.LogInfo($"  [{sideName} Team Status] {@event.Data.RemainingPokemon}/{@event.Data.TotalPokemon} Pokemon remaining ({@event.Data.FaintedCount} fainted)");
            }
        }

        private void LogPokemonSwitched(BattleEvent @event)
        {
            var sideName = @event.IsPlayerSide ? "Player" : "Enemy";
            var oldPokemon = @event.Data.SwitchedOut;
            var newPokemon = @event.Data.SwitchedIn;

            // Build switch message with more detail
            string switchMessage;
            if (oldPokemon == null)
            {
                // Empty slot being filled (initial send out or mandatory switch)
                if (@event.TurnNumber == 0)
                {
                    // Initial send out - show slot info
                    var slotInfo = @event.Data.SlotIndex >= 0 ? $" (Slot {@event.Data.SlotIndex + 1})" : "";
                    switchMessage = $"[{sideName}] Sent out {GetPokemonDisplayName(newPokemon)}{slotInfo}";
                }
                else
                {
                    switchMessage = $"[{sideName}] Sent out {GetPokemonDisplayName(newPokemon)}";
                }
            }
            else if (newPokemon == null)
            {
                // Pokemon being withdrawn (shouldn't happen in normal gameplay)
                switchMessage = $"[{sideName}] Withdrew {GetPokemonDisplayName(oldPokemon)}";
            }
            else
            {
                // Normal switch: use unique names from mapping
                var oldName = GetPokemonDisplayName(oldPokemon);
                var newName = GetPokemonDisplayName(newPokemon);
                switchMessage = $"[{sideName}] Switched {oldName} out, {newName} in";
            }

            _logger.LogInfo(switchMessage);

            // Log team status if available (but not for initial send out at turn 0)
            if (@event.TurnNumber > 0 && @event.Data.RemainingPokemon >= 0 && @event.Data.TotalPokemon > 0)
            {
                _logger.LogDebug($"  [{sideName} Team Status] {@event.Data.RemainingPokemon}/{@event.Data.TotalPokemon} Pokemon remaining ({@event.Data.FaintedCount} fainted)");
            }
        }

        private void LogAIDecision(BattleEvent @event)
        {
            if (@event.Pokemon == null)
                return;

            var sideName = @event.IsPlayerSide ? "Player" : "Enemy";
            var pokemonName = GetPokemonDisplayName(@event.Pokemon);
            var decisionType = @event.Data.DecisionType ?? "UNKNOWN";
            var reason = @event.Data.DecisionReason ?? "";

            _logger.LogDebug($"[AI Decision] [{sideName}] {pokemonName}: {decisionType} - {reason}");
        }

        private void LogMoveUsed(BattleEvent @event)
        {
            if (@event.Pokemon == null)
                return;

            var sideName = @event.IsPlayerSide ? "Player" : "Enemy";
            var moveName = @event.Data.MoveName ?? "Unknown";
            var targetName = GetPokemonDisplayName(@event.Data.TargetPokemon);

            _logger.LogInfo($"[{sideName}] {GetPokemonDisplayName(@event.Pokemon)} used {moveName} on {targetName}");
        }

        private void LogDamageDealt(BattleEvent @event)
        {
            if (@event.Pokemon == null)
                return;

            var attackerSide = @event.IsPlayerSide ? "Player" : "Enemy";
            var attackerName = GetPokemonDisplayName(@event.Pokemon);
            var moveName = @event.Data.MoveName ?? "Unknown";
            var targetName = GetPokemonDisplayName(@event.Data.TargetPokemon);
            var damage = @event.Data.DamageAmount;

            // Determine target side: for self-damage (status damage, recoil, etc.), target is same as attacker
            // For regular moves, target is on opposite side
            string targetSide;
            if (@event.Data.IsSelfDamage)
            {
                // Same Pokemon (status damage, recoil, etc.) - same side
                targetSide = attackerSide;
            }
            else
            {
                // Different Pokemon - opposite side (for regular moves)
                targetSide = !@event.IsPlayerSide ? "Player" : "Enemy";
            }

            // Improved format: clearly show attacker, move, target, and damage
            // Format: [Attacker Side] Attacker's Move → [Target Side] Target: damage
            _logger.LogInfo($"[{attackerSide}] {attackerName}'s {moveName} → [{targetSide}] {targetName}: {damage} damage");

            // Log detailed damage calculation (debug level)
            // Include attacker, move, and target information
            if (@event.Data.HpBefore > 0 || @event.Data.HpAfter >= 0)
            {
                _logger.LogDebug($"  [{attackerSide}] {attackerName}'s {moveName} → [{targetSide}] {targetName}: HP {@event.Data.HpBefore} → {@event.Data.HpAfter} ({@event.Data.TargetPokemon?.CurrentHP}/{@event.Data.TargetPokemon?.MaxHP})");
            }

            if (@event.Data.BaseDamage > 0)
            {
                _logger.LogDebug($"  [{attackerSide}] {attackerName}'s {moveName} → [{targetSide}] {targetName}: Base Damage {@event.Data.BaseDamage:F1}, Multiplier {@event.Data.Multiplier:F2}x, Final {damage}");
            }

            if (@event.Data.TypeEffectiveness != 1.0f)
            {
                string effectiveness = @event.Data.TypeEffectiveness switch
                {
                    0.0f => "No effect",
                    0.25f => "Not very effective (x0.25)",
                    0.5f => "Not very effective (x0.5)",
                    2.0f => "Super effective (x2)",
                    4.0f => "Super effective (x4)",
                    _ => $"Effectiveness: {@event.Data.TypeEffectiveness:F2}x"
                };
                _logger.LogInfo($"  {effectiveness}");
            }
        }

        private void LogBattleEnded(BattleEvent @event)
        {
            _logger.LogInfo($"Battle ended after {@event.Data.TotalTurns} turns. Outcome: {@event.Data.Outcome}");

            // Log team statistics if available
            if (@event.Data.PlayerTotal > 0 || @event.Data.EnemyTotal > 0)
            {
                _logger.LogInfo($"Player team: {@event.Data.PlayerFainted}/{@event.Data.PlayerTotal} Pokemon fainted");
                _logger.LogInfo($"Enemy team: {@event.Data.EnemyFainted}/{@event.Data.EnemyTotal} Pokemon fainted");
            }
        }

        private void LogActionCollected(BattleEvent @event)
        {
            if (@event.Pokemon == null)
                return;

            var actionType = @event.Data.ActionType ?? "Unknown";
            var priority = @event.Data.Priority;
            var speed = @event.Data.Speed;
            var slotIndex = @event.Data.SlotIndex;
            var pokemonName = GetPokemonDisplayName(@event.Pokemon);
            var executionOrder = @event.Data.ActionCount; // Used to store execution order
            var sideName = @event.IsPlayerSide ? "Player" : "Enemy";

            if (executionOrder > 0)
            {
                _logger.LogDebug($"  {executionOrder}. [{sideName}] {pokemonName} - Priority: {priority}, Speed: {speed:F1}");
            }
            else
            {
                _logger.LogDebug($"Collected action: [{sideName}] {pokemonName} - {actionType} from slot {slotIndex} (Priority: {priority}, Speed: {speed:F1})");
            }
        }

        private void LogActionsSorted(BattleEvent @event)
        {
            var actionCount = @event.Data.ActionCount;
            if (actionCount > 0)
            {
                _logger.LogDebug($"Actions sorted. Execution order:");

                // Show detailed information for each action
                if (@event.Data.SortedActionsDetails != null && @event.Data.SortedActionsDetails.Count > 0)
                {
                    foreach (var actionDetail in @event.Data.SortedActionsDetails)
                    {
                        _logger.LogDebug($"  {actionDetail}");
                    }
                }
            }
        }

        private void LogActionsGenerated(BattleEvent @event)
        {
            if (@event.Pokemon == null)
                return;

            var sideName = @event.IsPlayerSide ? "Player" : "Enemy";
            var pokemonName = GetPokemonDisplayName(@event.Pokemon);
            var moveName = @event.Data.MoveName ?? "Unknown";
            var actionTypes = @event.Data.GeneratedActionTypes ?? "None";
            var actionCount = @event.Data.GeneratedActionCount;

            _logger.LogDebug($"[{sideName}] {pokemonName}'s {moveName} generated {actionCount} action(s): {actionTypes}");
        }

        private void LogBattleStarted(BattleEvent @event)
        {
            var battleMode = @event.Data.BattleMode ?? "Unknown";
            var playerSlots = @event.Data.PlayerSlots;
            var enemySlots = @event.Data.EnemySlots;
            var playerPartySize = @event.Data.PlayerPartySize;
            var enemyPartySize = @event.Data.EnemyPartySize;
            var initialPlayerActive = @event.Data.RemainingPokemon; // Reused for initial active count
            var initialEnemyActive = @event.Data.TotalPokemon; // Reused for initial active count

            _logger.LogInfo($"Battle started - Mode: {battleMode}");
            _logger.LogInfo($"  Battle Slots: Player={playerSlots}, Enemy={enemySlots}");
            _logger.LogInfo($"  Team Size: Player={playerPartySize} Pokemon, Enemy={enemyPartySize} Pokemon");
            _logger.LogInfo($"  Initial Active: Player={initialPlayerActive} Pokemon, Enemy={initialEnemyActive} Pokemon");
        }

        private void LogMoveMissed(BattleEvent @event)
        {
            if (@event.Pokemon == null)
                return;

            var sideName = @event.IsPlayerSide ? "Player" : "Enemy";
            var pokemonName = GetPokemonDisplayName(@event.Pokemon);
            var moveName = @event.Data.MoveName ?? "Unknown";
            var targetName = GetPokemonDisplayName(@event.Data.TargetPokemon);

            _logger.LogInfo($"[{sideName}] {pokemonName}'s {moveName} missed {targetName}!");
        }

        private void LogStatusApplied(BattleEvent @event)
        {
            if (@event.Pokemon == null)
                return;

            var sideName = @event.IsPlayerSide ? "Player" : "Enemy";
            var pokemonName = GetPokemonDisplayName(@event.Pokemon);
            var statusName = @event.Data.StatusName ?? "Unknown";

            _logger.LogInfo($"[{sideName}] {pokemonName} was afflicted with {statusName}!");
        }
    }
}
