using System;
using System.Collections.Generic;
using System.Linq;
using PokemonUltimate.Combat.Actions;
using PokemonUltimate.Combat.Field;
using PokemonUltimate.Combat.Infrastructure.Events;
using PokemonUltimate.Combat.Infrastructure.Logging.Definition;
using PokemonUltimate.Core.Data.Enums;
using PokemonUltimate.Core.Domain.Instances.Pokemon;

namespace PokemonUltimate.BattleSimulator.Logging
{
    /// <summary>
    /// Listens to battle events and logs damage messages in a readable format.
    /// Generates messages like: "[Enemy] Haunter's Lengüetazo → [Player] Mew: 36 damage"
    /// </summary>
    /// <remarks>
    /// **Feature**: 6: Development Tools
    /// **Sub-Feature**: 6.8: Interactive Battle Simulator
    /// **Documentation**: See `docs/features/6-development-tools/6.8-interactive-battle-simulator/README.md`
    /// </remarks>
    public class BattleDamageLogger
    {
        private readonly IBattleLogger _logger;
        private Dictionary<string, string>? _pokemonNameMapping;

        /// <summary>
        /// Creates a new battle damage logger.
        /// </summary>
        /// <param name="logger">The logger to write messages to.</param>
        public BattleDamageLogger(IBattleLogger logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            // Subscribe to battle events
            BattleEventManager.BattleStart += OnBattleStart;
            BattleEventManager.TurnStart += OnTurnStart;
            BattleEventManager.ActionExecuted += OnActionExecuted;
        }

        /// <summary>
        /// Sets the Pokemon name mapping for displaying unique identifiers in logs.
        /// </summary>
        /// <param name="mapping">Dictionary mapping InstanceId to display name.</param>
        public void SetPokemonNameMapping(Dictionary<string, string>? mapping)
        {
            _pokemonNameMapping = mapping;
        }

        /// <summary>
        /// Handles BattleStart events and logs battle information.
        /// </summary>
        private void OnBattleStart(object? sender, BattleStartEventArgs e)
        {
            if (e.Field != null)
            {
                LogBattleInfo(e.Field);
            }
        }

        /// <summary>
        /// Logs detailed battle information at the start of the battle.
        /// </summary>
        private void LogBattleInfo(BattleField field)
        {
            try
            {
                var rules = field.Rules;

                // Determine battle mode name
                string battleMode = "Custom";
                if (rules.PlayerSlots == 1 && rules.EnemySlots == 1)
                    battleMode = "Singles";
                else if (rules.PlayerSlots == 2 && rules.EnemySlots == 2)
                    battleMode = "Doubles";
                else if (rules.PlayerSlots == 3 && rules.EnemySlots == 3)
                    battleMode = "Triples";
                else if (rules.PlayerSlots == 6 && rules.EnemySlots == 6)
                    battleMode = "Full Team";

                // Get team sizes
                int playerTeamSize = field.PlayerSide?.Party?.Count ?? 0;
                int enemyTeamSize = field.EnemySide?.Party?.Count ?? 0;

                // Log battle information
                _logger.LogInfo("═══════════════════════════════════════════════════════════════");
                _logger.LogInfo($"BATTLE INFORMATION - Mode: {battleMode}");
                _logger.LogInfo("═══════════════════════════════════════════════════════════════");
                _logger.LogInfo($"Battle Slots: Player={rules.PlayerSlots}, Enemy={rules.EnemySlots}");
                _logger.LogInfo($"Team Size: Player={playerTeamSize} Pokemon, Enemy={enemyTeamSize} Pokemon");
                _logger.LogInfo("");

                // Log Player Team
                _logger.LogInfo("═══════════════════════════════════════════════════════════════");
                _logger.LogInfo("PLAYER TEAM");
                _logger.LogInfo("═══════════════════════════════════════════════════════════════");

                if (field.PlayerSide?.Party != null && field.PlayerSide.Party.Count > 0)
                {
                    _logger.LogInfo($"Total Pokemon in Party: {field.PlayerSide.Party.Count}");
                    _logger.LogInfo("");

                    // List all Pokemon in party
                    for (int i = 0; i < field.PlayerSide.Party.Count; i++)
                    {
                        var pokemon = field.PlayerSide.Party[i];
                        var pokemonName = GetPokemonDisplayName(pokemon);
                        _logger.LogInfo($"  {i + 1}. {pokemonName} Lv.{pokemon.Level} (HP: {pokemon.CurrentHP}/{pokemon.MaxHP})");
                    }
                    _logger.LogInfo("");
                }

                // Log Player Slots
                if (field.PlayerSide?.Slots != null)
                {
                    _logger.LogInfo("Player Active Slots:");
                    for (int i = 0; i < field.PlayerSide.Slots.Count; i++)
                    {
                        var slot = field.PlayerSide.Slots[i];
                        if (!slot.IsEmpty && slot.Pokemon != null)
                        {
                            var pokemonName = GetPokemonDisplayName(slot.Pokemon);
                            _logger.LogInfo($"  Slot {i + 1}: {pokemonName} Lv.{slot.Pokemon.Level} (HP: {slot.Pokemon.CurrentHP}/{slot.Pokemon.MaxHP})");
                        }
                        else
                        {
                            _logger.LogInfo($"  Slot {i + 1}: Empty");
                        }
                    }
                    _logger.LogInfo("");
                }

                // Log Enemy Team
                _logger.LogInfo("═══════════════════════════════════════════════════════════════");
                _logger.LogInfo("ENEMY TEAM");
                _logger.LogInfo("═══════════════════════════════════════════════════════════════");

                if (field.EnemySide?.Party != null && field.EnemySide.Party.Count > 0)
                {
                    _logger.LogInfo($"Total Pokemon in Party: {field.EnemySide.Party.Count}");
                    _logger.LogInfo("");

                    // List all Pokemon in party
                    for (int i = 0; i < field.EnemySide.Party.Count; i++)
                    {
                        var pokemon = field.EnemySide.Party[i];
                        var pokemonName = GetPokemonDisplayName(pokemon);
                        _logger.LogInfo($"  {i + 1}. {pokemonName} Lv.{pokemon.Level} (HP: {pokemon.CurrentHP}/{pokemon.MaxHP})");
                    }
                    _logger.LogInfo("");
                }

                // Log Enemy Slots
                if (field.EnemySide?.Slots != null)
                {
                    _logger.LogInfo("Enemy Active Slots:");
                    for (int i = 0; i < field.EnemySide.Slots.Count; i++)
                    {
                        var slot = field.EnemySide.Slots[i];
                        if (!slot.IsEmpty && slot.Pokemon != null)
                        {
                            var pokemonName = GetPokemonDisplayName(slot.Pokemon);
                            _logger.LogInfo($"  Slot {i + 1}: {pokemonName} Lv.{slot.Pokemon.Level} (HP: {slot.Pokemon.CurrentHP}/{slot.Pokemon.MaxHP})");
                        }
                        else
                        {
                            _logger.LogInfo($"  Slot {i + 1}: Empty");
                        }
                    }
                    _logger.LogInfo("");
                }

                _logger.LogInfo("═══════════════════════════════════════════════════════════════");
                _logger.LogInfo("BATTLE STARTING...");
                _logger.LogInfo("═══════════════════════════════════════════════════════════════");
                _logger.LogInfo("");
            }
            catch (Exception)
            {
                // Silently ignore errors to avoid disrupting battle flow
            }
        }

        /// <summary>
        /// Handles TurnStart events and logs Pokemon status.
        /// </summary>
        private void OnTurnStart(object? sender, TurnEventArgs e)
        {
            if (e.Field != null)
            {
                LogTurnStatus(e.TurnNumber, e.Field);
            }
        }

        /// <summary>
        /// Logs Pokemon status at the start of each turn.
        /// Shows complete team status, not just active slots.
        /// </summary>
        private void LogTurnStatus(int turnNumber, BattleField field)
        {
            try
            {
                _logger.LogInfo("");
                _logger.LogInfo($"═══════════════════════════════════════════════════════════════");
                _logger.LogInfo($"TURN {turnNumber} - Complete Team Status");
                _logger.LogInfo($"═══════════════════════════════════════════════════════════════");

                // Log Player Team (Complete)
                _logger.LogInfo("═══════════════════════════════════════════════════════════════");
                _logger.LogInfo("PLAYER TEAM");
                _logger.LogInfo("═══════════════════════════════════════════════════════════════");

                if (field.PlayerSide?.Party != null && field.PlayerSide.Party.Count > 0)
                {
                    for (int i = 0; i < field.PlayerSide.Party.Count; i++)
                    {
                        var pokemon = field.PlayerSide.Party[i];
                        var pokemonName = GetPokemonDisplayName(pokemon);

                        // Find if this Pokemon is in an active slot
                        var activeSlot = field.PlayerSide.Slots.FirstOrDefault(s => s.Pokemon == pokemon);
                        bool isActive = activeSlot != null && !activeSlot.IsEmpty;

                        string statusText = isActive ? GetPokemonStatusText(pokemon, activeSlot) : GetPokemonStatusText(pokemon, null);
                        string activeIndicator = isActive ? "[ACTIVE]" : "[BENCH]";

                        _logger.LogInfo($"  {i + 1}. {activeIndicator} {pokemonName} Lv.{pokemon.Level} - HP: {pokemon.CurrentHP}/{pokemon.MaxHP} {statusText}");

                        // If active, show stat stages
                        if (isActive)
                        {
                            var statStages = GetStatStagesText(activeSlot);
                            if (!string.IsNullOrEmpty(statStages))
                            {
                                _logger.LogInfo($"      Stat Stages: {statStages}");
                            }
                        }
                    }
                }
                _logger.LogInfo("");

                // Log Enemy Team (Complete)
                _logger.LogInfo("═══════════════════════════════════════════════════════════════");
                _logger.LogInfo("ENEMY TEAM");
                _logger.LogInfo("═══════════════════════════════════════════════════════════════");

                if (field.EnemySide?.Party != null && field.EnemySide.Party.Count > 0)
                {
                    for (int i = 0; i < field.EnemySide.Party.Count; i++)
                    {
                        var pokemon = field.EnemySide.Party[i];
                        var pokemonName = GetPokemonDisplayName(pokemon);

                        // Find if this Pokemon is in an active slot
                        var activeSlot = field.EnemySide.Slots.FirstOrDefault(s => s.Pokemon == pokemon);
                        bool isActive = activeSlot != null && !activeSlot.IsEmpty;

                        string statusText = isActive ? GetPokemonStatusText(pokemon, activeSlot) : GetPokemonStatusText(pokemon, null);
                        string activeIndicator = isActive ? "[ACTIVE]" : "[BENCH]";

                        _logger.LogInfo($"  {i + 1}. {activeIndicator} {pokemonName} Lv.{pokemon.Level} - HP: {pokemon.CurrentHP}/{pokemon.MaxHP} {statusText}");

                        // If active, show stat stages
                        if (isActive)
                        {
                            var statStages = GetStatStagesText(activeSlot);
                            if (!string.IsNullOrEmpty(statStages))
                            {
                                _logger.LogInfo($"      Stat Stages: {statStages}");
                            }
                        }
                    }
                }
                _logger.LogInfo("");

                _logger.LogInfo($"═══════════════════════════════════════════════════════════════");
                _logger.LogInfo("");
            }
            catch (Exception)
            {
                // Silently ignore errors to avoid disrupting battle flow
            }
        }

        /// <summary>
        /// Gets a text representation of the Pokemon's status.
        /// </summary>
        private string GetPokemonStatusText(PokemonInstance pokemon, BattleSlot? slot)
        {
            var statusParts = new List<string>();

            // Add persistent status (Burn, Poison, etc.)
            if (pokemon.Status != PersistentStatus.None)
            {
                statusParts.Add($"Status: {pokemon.Status}");
            }

            // Add volatile status from slot (only if slot is not null and Pokemon is active)
            if (slot != null && slot.VolatileStatus != VolatileStatus.None)
            {
                var volatileStatusNames = GetVolatileStatusNames(slot.VolatileStatus);
                if (!string.IsNullOrEmpty(volatileStatusNames))
                {
                    statusParts.Add($"Volatile: {volatileStatusNames}");
                }
            }

            if (statusParts.Count == 0)
            {
                return "Status: None";
            }

            return string.Join(", ", statusParts);
        }

        /// <summary>
        /// Gets display names for volatile status flags.
        /// </summary>
        private string GetVolatileStatusNames(VolatileStatus status)
        {
            if (status == VolatileStatus.None)
                return string.Empty;

            var names = new List<string>();
            foreach (VolatileStatus flag in Enum.GetValues<VolatileStatus>())
            {
                if (flag != VolatileStatus.None && status.HasFlag(flag))
                {
                    names.Add(flag.ToString());
                }
            }

            return names.Count > 0 ? string.Join(", ", names) : string.Empty;
        }

        /// <summary>
        /// Handles ActionExecuted events and logs various action messages in green.
        /// </summary>
        private void OnActionExecuted(object? sender, ActionExecutedEventArgs e)
        {
            if (e.Action == null || e.Field == null)
                return;

            try
            {
                switch (e.Action)
                {
                    case DamageAction damageAction:
                        LogDamageMessage(damageAction, e.Field);
                        break;
                    case StatChangeAction statChangeAction:
                        LogStatChangeMessage(statChangeAction, e.Field);
                        break;
                    case ApplyStatusAction statusAction:
                        LogStatusMessage(statusAction, e.Field);
                        break;
                    case SwitchAction switchAction:
                        LogSwitchMessage(switchAction, e.Field);
                        break;
                    case HealAction healAction:
                        LogHealMessage(healAction, e.Field);
                        break;
                    case FaintAction faintAction:
                        LogFaintMessage(faintAction, e.Field);
                        break;
                }
            }
            catch (Exception)
            {
                // Silently ignore errors to avoid disrupting battle flow
            }
        }

        /// <summary>
        /// Logs a damage message in the format: "[Team] Attacker's Move → [Team] Target: X damage"
        /// Also logs detailed damage calculation in red.
        /// </summary>
        private void LogDamageMessage(DamageAction damageAction, BattleField field)
        {
            try
            {
                // Get attacker and target
                var attacker = damageAction.User?.Pokemon;
                var target = damageAction.Target?.Pokemon;
                var move = damageAction.Context?.Move;

                if (attacker == null || target == null || move == null || damageAction.Context == null)
                    return;

                // Determine team labels
                bool attackerIsPlayer = field.PlayerSide?.Slots?.Contains(damageAction.User) ?? false;
                bool targetIsPlayer = field.PlayerSide?.Slots?.Contains(damageAction.Target) ?? false;

                string attackerTeam = attackerIsPlayer ? "[Player]" : "[Enemy]";
                string targetTeam = targetIsPlayer ? "[Player]" : "[Enemy]";

                // Get Pokemon names (use mapping if available)
                string attackerName = GetPokemonDisplayName(attacker);
                string targetName = GetPokemonDisplayName(target);

                // Get move name
                string moveName = move.Name;

                // Get damage context
                var context = damageAction.Context;

                // Log detailed damage calculation in red (using LogError for red color)
                var criticalText = context.IsCritical ? "Yes (×1.5)" : "No";
                var stabText = context.IsStab ? "Yes (×1.5)" : "No";
                var typeEffectivenessText = context.TypeEffectiveness switch
                {
                    0f => "Immune (×0)",
                    0.25f => "Not Very Effective (×0.25)",
                    0.5f => "Not Very Effective (×0.5)",
                    1f => "Normal (×1)",
                    2f => "Super Effective (×2)",
                    4f => "Super Effective (×4)",
                    _ => $"({context.TypeEffectiveness:F2}x)"
                };

                var damageCalcMessage = $"DAMAGE CALCULATION: {attackerTeam} {attackerName}'s {moveName} → {targetTeam} {targetName}\n" +
                    $"  Base Damage: {context.BaseDamage:F2}\n" +
                    $"  Critical Hit: {criticalText}\n" +
                    $"  STAB: {stabText}\n" +
                    $"  Type Effectiveness: {typeEffectivenessText}\n" +
                    $"  Random Factor: {context.RandomFactor:F3} (range: 0.85-1.0)\n" +
                    $"  Final Multiplier: {context.Multiplier:F4}x\n" +
                    $"  Final Damage: {context.FinalDamage} HP";

                _logger.LogError(damageCalcMessage);

                // Format and log the damage message in green (using LogInfo)
                // Format: "[Team] AttackerName's MoveName → [Team] TargetName: X damage"
                string message = $"{attackerTeam} {attackerName}'s {moveName} → {targetTeam} {targetName}: {context.FinalDamage} damage";
                _logger.LogInfo(message);
            }
            catch (Exception)
            {
                // Silently ignore errors to avoid disrupting battle flow
            }
        }

        /// <summary>
        /// Gets the display name for a Pokemon, using the mapping if available.
        /// </summary>
        private string GetPokemonDisplayName(PokemonInstance pokemon)
        {
            if (_pokemonNameMapping != null && _pokemonNameMapping.TryGetValue(pokemon.InstanceId, out var mappedName))
            {
                return mappedName;
            }
            return pokemon.DisplayName;
        }

        /// <summary>
        /// Gets a text representation of stat stages for a slot.
        /// </summary>
        private string GetStatStagesText(BattleSlot? slot)
        {
            if (slot == null || slot.IsEmpty)
                return string.Empty;

            var statNames = new[] { "ATK", "DEF", "SPA", "SPD", "SPE", "ACC", "EVA" };
            var statEnums = new[] { Stat.Attack, Stat.Defense, Stat.SpAttack, Stat.SpDefense, Stat.Speed, Stat.Accuracy, Stat.Evasion };

            var stages = new List<string>();
            for (int i = 0; i < statNames.Length; i++)
            {
                int stage = slot.GetStatStage(statEnums[i]);
                if (stage != 0)
                {
                    string sign = stage > 0 ? "+" : "";
                    stages.Add($"{statNames[i]}: {sign}{stage}");
                }
            }

            return stages.Count > 0 ? string.Join(", ", stages) : "None";
        }

        /// <summary>
        /// Logs a stat change message.
        /// </summary>
        private void LogStatChangeMessage(StatChangeAction action, BattleField field)
        {
            if (action.Target?.Pokemon == null || action.Change == 0)
                return;

            bool userIsPlayer = field.PlayerSide?.Slots?.Contains(action.User) ?? false;
            bool targetIsPlayer = field.PlayerSide?.Slots?.Contains(action.Target) ?? false;

            string userTeam = userIsPlayer ? "[Player]" : "[Enemy]";
            string targetTeam = targetIsPlayer ? "[Player]" : "[Enemy]";

            string userName = action.User?.Pokemon != null ? GetPokemonDisplayName(action.User.Pokemon) : "System";
            string targetName = GetPokemonDisplayName(action.Target.Pokemon);
            string statName = action.Stat.ToString();
            string changeText = action.Change > 0 ? $"+{action.Change}" : action.Change.ToString();

            // If user is the same as target (self-buff/debuff), simplify the message
            string message;
            if (action.User != null && action.User == action.Target)
            {
                message = $"{userTeam} {userName}: {statName} {changeText}";
            }
            else
            {
                message = $"{userTeam} {userName} → {targetTeam} {targetName}: {statName} {changeText}";
            }

            _logger.LogInfo(message);
        }

        /// <summary>
        /// Logs a status application message.
        /// </summary>
        private void LogStatusMessage(ApplyStatusAction action, BattleField field)
        {
            if (action.Target?.Pokemon == null || action.Status == PersistentStatus.None)
                return;

            bool userIsPlayer = field.PlayerSide?.Slots?.Contains(action.User) ?? false;
            bool targetIsPlayer = field.PlayerSide?.Slots?.Contains(action.Target) ?? false;

            string userTeam = userIsPlayer ? "[Player]" : "[Enemy]";
            string targetTeam = targetIsPlayer ? "[Player]" : "[Enemy]";

            string userName = action.User?.Pokemon != null ? GetPokemonDisplayName(action.User.Pokemon) : "System";
            string targetName = GetPokemonDisplayName(action.Target.Pokemon);
            string statusName = action.Status.ToString();

            string message = $"{userTeam} {userName} → {targetTeam} {targetName}: Applied {statusName}";
            _logger.LogInfo(message);
        }

        /// <summary>
        /// Logs a switch message.
        /// </summary>
        private void LogSwitchMessage(SwitchAction action, BattleField field)
        {
            if (action.Slot == null || action.NewPokemon == null)
                return;

            bool isPlayer = field.PlayerSide?.Slots?.Contains(action.Slot) ?? false;
            string team = isPlayer ? "[Player]" : "[Enemy]";

            string oldName = action.OldPokemon != null ? GetPokemonDisplayName(action.OldPokemon) : "Empty";
            string newName = GetPokemonDisplayName(action.NewPokemon);

            string message = $"{team} Switched: {oldName} → {newName}";
            _logger.LogInfo(message);
        }

        /// <summary>
        /// Logs a heal message.
        /// </summary>
        private void LogHealMessage(HealAction action, BattleField field)
        {
            if (action.Target?.Pokemon == null || action.Amount <= 0)
                return;

            bool userIsPlayer = field.PlayerSide?.Slots?.Contains(action.User) ?? false;
            bool targetIsPlayer = field.PlayerSide?.Slots?.Contains(action.Target) ?? false;

            string userTeam = userIsPlayer ? "[Player]" : "[Enemy]";
            string targetTeam = targetIsPlayer ? "[Player]" : "[Enemy]";

            string userName = action.User?.Pokemon != null ? GetPokemonDisplayName(action.User.Pokemon) : "System";
            string targetName = GetPokemonDisplayName(action.Target.Pokemon);

            // If user is the same as target (self-heal), simplify the message
            string message;
            if (action.User != null && action.User == action.Target)
            {
                message = $"{userTeam} {userName}: Healed {action.Amount} HP";
            }
            else
            {
                message = $"{userTeam} {userName} → {targetTeam} {targetName}: Healed {action.Amount} HP";
            }

            _logger.LogInfo(message);
        }

        /// <summary>
        /// Logs a faint message.
        /// </summary>
        private void LogFaintMessage(FaintAction action, BattleField field)
        {
            if (action.Target?.Pokemon == null)
                return;

            bool targetIsPlayer = field.PlayerSide?.Slots?.Contains(action.Target) ?? false;
            string targetTeam = targetIsPlayer ? "[Player]" : "[Enemy]";

            string targetName = GetPokemonDisplayName(action.Target.Pokemon);
            string userText = action.User?.Pokemon != null ? $" (by {GetPokemonDisplayName(action.User.Pokemon)})" : "";

            string message = $"{targetTeam} {targetName} fainted{userText}";
            _logger.LogInfo(message);
        }

        /// <summary>
        /// Unsubscribes from events. Call this when disposing.
        /// </summary>
        public void Unsubscribe()
        {
            BattleEventManager.BattleStart -= OnBattleStart;
            BattleEventManager.TurnStart -= OnTurnStart;
            BattleEventManager.ActionExecuted -= OnActionExecuted;
        }
    }
}
