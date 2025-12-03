using System;
using System.Collections.Generic;
using PokemonUltimate.Combat.Actions;
using PokemonUltimate.Core.Blueprints;
using PokemonUltimate.Core.Constants;
using PokemonUltimate.Core.Enums;

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
        /// Processes battle triggers and generates actions based on ability configuration.
        /// </summary>
        public IEnumerable<BattleAction> OnTrigger(BattleTrigger trigger, BattleSlot holder, BattleField field)
        {
            if (holder == null)
                throw new ArgumentNullException(nameof(holder), ErrorMessages.SlotCannotBeNull);
            if (field == null)
                throw new ArgumentNullException(nameof(field), ErrorMessages.FieldCannotBeNull);

            // Check if this ability listens to this trigger
            if (!ShouldRespondToTrigger(trigger))
                yield break;

            // Process based on ability effect
            foreach (var action in ProcessAbilityEffect(holder, field))
            {
                yield return action;
            }
        }

        /// <summary>
        /// Checks if this ability should respond to the given trigger.
        /// </summary>
        private bool ShouldRespondToTrigger(BattleTrigger trigger)
        {
            switch (trigger)
            {
                case BattleTrigger.OnSwitchIn:
                    return _abilityData.ListensTo(AbilityTrigger.OnSwitchIn);
                case BattleTrigger.OnTurnEnd:
                    return _abilityData.ListensTo(AbilityTrigger.OnTurnEnd);
                case BattleTrigger.OnDamageTaken:
                    return _abilityData.ListensTo(AbilityTrigger.OnDamageTaken);
                case BattleTrigger.OnBeforeMove:
                    return _abilityData.ListensTo(AbilityTrigger.OnBeforeMove);
                case BattleTrigger.OnAfterMove:
                    return _abilityData.ListensTo(AbilityTrigger.OnAfterMove);
                case BattleTrigger.OnWeatherChange:
                    return _abilityData.ListensTo(AbilityTrigger.OnWeatherChange);
                default:
                    return false;
            }
        }

        /// <summary>
        /// Processes ability effects and returns actions.
        /// </summary>
        private IEnumerable<BattleAction> ProcessAbilityEffect(BattleSlot holder, BattleField field)
        {
            switch (_abilityData.Effect)
            {
                case AbilityEffect.LowerOpponentStat:
                    foreach (var action in ProcessLowerOpponentStat(holder, field))
                        yield return action;
                    break;

                case AbilityEffect.TypePowerBoostWhenLowHP:
                    // Deferred - requires HP check and move type check
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
            yield return new MessageAction(string.Format(GameMessages.AbilityActivated, holder.Pokemon.DisplayName, _abilityData.Name));

            // Lower stat for all opposing active Pokemon
            foreach (var enemySlot in opposingSide.GetActiveSlots())
            {
                if (!enemySlot.IsEmpty && !enemySlot.HasFainted)
                {
                    yield return new StatChangeAction(holder, enemySlot, _abilityData.TargetStat.Value, _abilityData.StatStages);
                }
            }
        }
    }
}

