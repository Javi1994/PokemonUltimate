using System;
using System.Collections.Generic;
using PokemonUltimate.Combat.Actions;
using PokemonUltimate.Combat.Extensions;
using PokemonUltimate.Content.Extensions;
using PokemonUltimate.Core.Blueprints;
using PokemonUltimate.Core.Constants;
using PokemonUltimate.Core.Enums;
using PokemonUltimate.Core.Extensions;
using PokemonUltimate.Core.Localization;
using ContactDamageAction = PokemonUltimate.Combat.Actions.ContactDamageAction;
using PersistentStatus = PokemonUltimate.Core.Enums.PersistentStatus;

namespace PokemonUltimate.Combat.Events
{
    /// <summary>
    /// Adapts AbilityData to IBattleListener interface.
    /// Converts ability triggers to battle actions based on ability configuration.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.9: Abilities & Items
    /// **Documentation**: See `docs/features/2-combat-system/2.9-abilities-items/architecture.md`
    /// </remarks>
    public class AbilityListener : IBattleListener
    {
        private readonly AbilityData _abilityData;

        // Static dictionary to track Truant state per slot (temporary solution)
        // Key: Slot reference, Value: true if next turn should be skipped
        private static readonly Dictionary<object, bool> _truantState = new Dictionary<object, bool>();

        // Dictionary mapping BattleTrigger to AbilityTrigger for efficient lookup
        private static readonly Dictionary<BattleTrigger, Core.Enums.AbilityTrigger> TriggerMapping = new Dictionary<BattleTrigger, Core.Enums.AbilityTrigger>
        {
            { BattleTrigger.OnSwitchIn, Core.Enums.AbilityTrigger.OnSwitchIn },
            { BattleTrigger.OnTurnEnd, Core.Enums.AbilityTrigger.OnTurnEnd },
            { BattleTrigger.OnDamageTaken, Core.Enums.AbilityTrigger.OnDamageTaken },
            { BattleTrigger.OnBeforeMove, Core.Enums.AbilityTrigger.OnBeforeMove },
            { BattleTrigger.OnAfterMove, Core.Enums.AbilityTrigger.OnAfterMove },
            { BattleTrigger.OnWeatherChange, Core.Enums.AbilityTrigger.OnWeatherChange },
            { BattleTrigger.OnContactReceived, Core.Enums.AbilityTrigger.OnContactReceived },
            { BattleTrigger.OnWouldFaint, Core.Enums.AbilityTrigger.OnWouldFaint }
        };

        /// <summary>
        /// Creates a new ability listener for the given ability data.
        /// </summary>
        /// <param name="abilityData">The ability data to listen with. Cannot be null.</param>
        /// <exception cref="ArgumentNullException">If abilityData is null.</exception>
        public AbilityListener(AbilityData abilityData)
        {
            _abilityData = abilityData ?? throw new ArgumentNullException(nameof(abilityData), ErrorMessages.AbilityCannotBeNull);
        }

        /// <summary>
        /// Called when a battle trigger occurs.
        /// Returns actions to be enqueued and processed.
        /// </summary>
        public IEnumerable<BattleAction> OnTrigger(BattleTrigger trigger, BattleSlot holder, BattleField field)
        {
            return OnTrigger(trigger, holder, field, attacker: null);
        }

        /// <summary>
        /// Called when a battle trigger occurs with additional context (e.g., attacker for contact moves).
        /// Returns actions to be enqueued and processed.
        /// </summary>
        public IEnumerable<BattleAction> OnTrigger(BattleTrigger trigger, BattleSlot holder, BattleField field, BattleSlot attacker)
        {
            if (holder == null)
                throw new ArgumentNullException(nameof(holder), ErrorMessages.SlotCannotBeNull);
            if (field == null)
                throw new ArgumentNullException(nameof(field), ErrorMessages.FieldCannotBeNull);

            // Check if this ability listens to this trigger
            if (!ShouldRespondToTrigger(trigger))
                yield break;

            // Process based on ability effect and trigger
            foreach (var action in ProcessAbilityEffect(trigger, holder, field, attacker))
            {
                yield return action;
            }
        }

        /// <summary>
        /// Checks if this ability should respond to the given trigger.
        /// Uses dictionary lookup for efficient trigger mapping.
        /// </summary>
        private bool ShouldRespondToTrigger(BattleTrigger trigger)
        {
            if (TriggerMapping.TryGetValue(trigger, out var abilityTrigger))
            {
                return _abilityData.ListensTo(abilityTrigger);
            }

            return false;
        }

        /// <summary>
        /// Processes ability effects and returns actions.
        /// </summary>
        private IEnumerable<BattleAction> ProcessAbilityEffect(BattleTrigger trigger, BattleSlot holder, BattleField field, BattleSlot attacker = null)
        {
            switch (_abilityData.Effect)
            {
                case AbilityEffect.LowerOpponentStat:
                    // Only on switch-in
                    if (trigger == BattleTrigger.OnSwitchIn)
                    {
                        foreach (var action in ProcessLowerOpponentStat(holder, field))
                            yield return action;
                    }
                    break;

                case AbilityEffect.RaiseOwnStat:
                    // Only on turn end (e.g., Speed Boost)
                    if (trigger == BattleTrigger.OnTurnEnd)
                    {
                        foreach (var action in ProcessRaiseOwnStat(holder, field))
                            yield return action;
                    }
                    break;

                case AbilityEffect.SkipTurn:
                    // Only on before move (e.g., Truant)
                    if (trigger == BattleTrigger.OnBeforeMove)
                    {
                        foreach (var action in ProcessSkipTurn(holder, field))
                            yield return action;
                    }
                    break;

                case AbilityEffect.RaiseStatOnKO:
                    // Only on after move (e.g., Moxie)
                    if (trigger == BattleTrigger.OnAfterMove)
                    {
                        foreach (var action in ProcessRaiseStatOnKO(holder, field))
                            yield return action;
                    }
                    break;

                case AbilityEffect.ChanceToStatusOnContact:
                    // Only on contact received (e.g., Static)
                    if (trigger == BattleTrigger.OnContactReceived && attacker != null)
                    {
                        foreach (var action in ProcessChanceToStatusOnContact(holder, attacker, field))
                            yield return action;
                    }
                    break;

                case AbilityEffect.DamageOnContact:
                    // Only on contact received (e.g., Rough Skin, Iron Barbs)
                    if (trigger == BattleTrigger.OnContactReceived && attacker != null)
                    {
                        foreach (var action in ProcessDamageOnContact(holder, attacker, field))
                            yield return action;
                    }
                    break;

                case AbilityEffect.SpeedBoostInWeather:
                    // Only on weather change (e.g., Swift Swim, Chlorophyll)
                    if (trigger == BattleTrigger.OnWeatherChange)
                    {
                        // Speed boost is handled passively via stat modifiers
                        // This trigger is just for activation messages if needed
                        yield break;
                    }
                    break;

                case AbilityEffect.TypePowerBoostWhenLowHP:
                    // Deferred - requires HP check and move type check (handled in damage pipeline)
                    break;

                default:
                    // Other effects deferred to future phases
                    break;
            }
        }

        /// <summary>
        /// Processes LowerOpponentStat effect (e.g., Intimidate).
        /// </summary>
        private IEnumerable<BattleAction> ProcessLowerOpponentStat(BattleSlot holder, BattleField field)
        {
            if (_abilityData.TargetStat == null)
                yield break;

            var opposingSide = field.GetOppositeSide(holder.Side);

            // Message for ability activation
            var provider = LocalizationManager.Instance;
            var abilityName = _abilityData.GetDisplayName(provider);
            yield return new MessageAction(provider.GetString(LocalizationKey.AbilityActivated, holder.Pokemon.DisplayName, abilityName));

            // Lower stat for all opposing active Pokemon
            foreach (var enemySlot in opposingSide.GetActiveSlots())
            {
                if (enemySlot.IsActive())
                {
                    yield return new StatChangeAction(holder, enemySlot, _abilityData.TargetStat.Value, _abilityData.StatStages);
                }
            }
        }

        /// <summary>
        /// Processes RaiseOwnStat effect (e.g., Speed Boost).
        /// </summary>
        private IEnumerable<BattleAction> ProcessRaiseOwnStat(BattleSlot holder, BattleField field)
        {
            if (_abilityData.TargetStat == null)
                yield break;

            var pokemon = holder.Pokemon;
            var currentStatStage = holder.GetStatStage(_abilityData.TargetStat.Value);

            // Check if stat is already maxed (+6 stages)
            if (currentStatStage >= 6)
                yield break;

            // Message for ability activation
            var provider = LocalizationManager.Instance;
            var abilityName = _abilityData.GetDisplayName(provider);
            yield return new MessageAction(provider.GetString(LocalizationKey.AbilityActivated, pokemon.DisplayName, abilityName));

            // Raise own stat
            yield return new StatChangeAction(holder, holder, _abilityData.TargetStat.Value, _abilityData.StatStages);
        }

        /// <summary>
        /// Processes SkipTurn effect (e.g., Truant).
        /// Blocks the move if it's an even turn.
        /// </summary>
        private IEnumerable<BattleAction> ProcessSkipTurn(BattleSlot holder, BattleField field)
        {
            // Track Truant state: alternate between allowing and blocking moves
            // Use slot reference as key
            bool shouldSkip = _truantState.TryGetValue(holder, out var skipNext) && skipNext;

            if (shouldSkip)
            {
                // Even turn - block move
                _truantState[holder] = false; // Reset for next turn
                var provider = LocalizationManager.Instance;
                yield return new MessageAction(provider.GetString(LocalizationKey.TruantLoafing, holder.Pokemon.DisplayName));
                // Note: The move will be blocked by returning actions that indicate failure
                // UseMoveAction should check for TruantLoafing message and return early
            }
            else
            {
                // Odd turn - allow move, mark next turn to skip
                _truantState[holder] = true;
                // Don't yield any actions - move proceeds normally
            }
        }

        /// <summary>
        /// Processes RaiseStatOnKO effect (e.g., Moxie).
        /// Raises stat when knocking out an opponent.
        /// </summary>
        private IEnumerable<BattleAction> ProcessRaiseStatOnKO(BattleSlot holder, BattleField field)
        {
            if (_abilityData.TargetStat == null)
                yield break;

            // Check if any opponent Pokemon fainted this turn
            // We need to check all opposing slots (not just active ones) to see if any are fainted
            var opposingSide = field.GetOppositeSide(holder.Side);
            bool anyFainted = false;

            // Check all slots, not just active ones, since we're looking for fainted Pokemon
            foreach (var enemySlot in opposingSide.Slots)
            {
                if (!enemySlot.IsEmpty && enemySlot.Pokemon.IsFainted)
                {
                    anyFainted = true;
                    break;
                }
            }

            if (!anyFainted)
                yield break;

            // Message for ability activation
            var provider = LocalizationManager.Instance;
            var abilityName = _abilityData.GetDisplayName(provider);
            yield return new MessageAction(provider.GetString(LocalizationKey.AbilityActivated, holder.Pokemon.DisplayName, abilityName));

            // Raise own stat
            yield return new StatChangeAction(holder, holder, _abilityData.TargetStat.Value, _abilityData.StatStages);
        }

        /// <summary>
        /// Processes ChanceToStatusOnContact effect (e.g., Static, Poison Point).
        /// </summary>
        private IEnumerable<BattleAction> ProcessChanceToStatusOnContact(BattleSlot holder, BattleSlot attacker, BattleField field)
        {
            if (_abilityData.StatusEffect == null || attacker == null || !attacker.IsActive())
                yield break;

            // Check if attacker already has a status condition
            if (attacker.Pokemon.Status != PersistentStatus.None)
                yield break;

            // Calculate chance using EffectChance (default 30% for Static)
            float chance = _abilityData.EffectChance > 0 ? _abilityData.EffectChance : 0.30f;

            // Use random provider (we'll need to inject this or use a static one)
            // For now, use a simple random check
            var random = new System.Random();
            if (random.NextDouble() >= chance)
                yield break;

            // Message for ability activation
            var provider = LocalizationManager.Instance;
            var abilityName = _abilityData.GetDisplayName(provider);
            yield return new MessageAction(provider.GetString(LocalizationKey.AbilityActivated, holder.Pokemon.DisplayName, abilityName));

            // Apply status to attacker
            yield return new ApplyStatusAction(holder, attacker, _abilityData.StatusEffect.Value);
        }

        /// <summary>
        /// Processes DamageOnContact effect (e.g., Rough Skin, Iron Barbs).
        /// </summary>
        private IEnumerable<BattleAction> ProcessDamageOnContact(BattleSlot holder, BattleSlot attacker, BattleField field)
        {
            if (attacker == null || !attacker.IsActive())
                yield break;

            // Calculate damage using Multiplier (default 0.125 = 1/8 of attacker's max HP for Rough Skin)
            float damageMultiplier = _abilityData.Multiplier > 0 ? _abilityData.Multiplier : 0.125f;
            int damage = (int)(attacker.Pokemon.MaxHP * damageMultiplier);

            // Minimum 1 HP damage
            if (damage < 1)
                damage = 1;

            // Message for ability activation
            var provider = LocalizationManager.Instance;
            var abilityName = _abilityData.GetDisplayName(provider);
            yield return new MessageAction(provider.GetString(LocalizationKey.AbilityActivated, holder.Pokemon.DisplayName, abilityName));

            // Apply damage using ContactDamageAction (executes when action is executed, not when it's created)
            yield return new ContactDamageAction(attacker, damage, abilityName);
        }
    }
}

