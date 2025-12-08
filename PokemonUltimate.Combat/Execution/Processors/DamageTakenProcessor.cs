using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PokemonUltimate.Combat.Actions;
using PokemonUltimate.Combat.Execution.Battle;
using PokemonUltimate.Combat.Execution.Processors.Definition;
using PokemonUltimate.Combat.Extensions;
using PokemonUltimate.Combat.Foundation.Field;
using PokemonUltimate.Core.Data.Blueprints;
using PokemonUltimate.Core.Data.Constants;
using PokemonUltimate.Core.Data.Enums;
using PokemonUltimate.Core.Utilities.Extensions;
using PokemonUltimate.Localization.Constants;
using PokemonUltimate.Localization.Extensions;
using PokemonUltimate.Localization.Services;

namespace PokemonUltimate.Combat.Execution.Processors
{
    /// <summary>
    /// Processes abilities and items that activate when a Pokemon takes damage.
    /// Examples: Anger Point, Berserk, etc.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.6: Combat Engine
    /// **Documentation**: See `docs/features/2-combat-system/2.6-combat-engine/architecture.md`
    /// </remarks>
    public class DamageTakenProcessor : IActionGeneratingPhaseProcessor
    {
        /// <summary>
        /// Gets the phase this processor handles.
        /// </summary>
        public BattlePhase Phase => BattlePhase.DamageTaken;

        /// <summary>
        /// Processes abilities and items that activate when a Pokemon takes damage.
        /// </summary>
        /// <param name="slot">The slot that took damage.</param>
        /// <param name="field">The battlefield.</param>
        /// <returns>List of actions generated.</returns>
        public List<BattleAction> ProcessDamageTaken(BattleSlot slot, BattleField field)
        {
            if (slot == null)
                throw new ArgumentNullException(nameof(slot));
            if (field == null)
                throw new ArgumentNullException(nameof(field), ErrorMessages.FieldCannotBeNull);

            var actions = new List<BattleAction>();

            var pokemon = slot.Pokemon;
            if (pokemon == null || !slot.IsActive())
                return actions;

            // Process ability
            if (pokemon.Ability != null)
            {
                var abilityActions = ProcessAbility(pokemon.Ability, slot, field);
                actions.AddRange(abilityActions);
            }

            // Process item
            if (pokemon.HeldItem != null)
            {
                var itemActions = ProcessItem(pokemon.HeldItem, slot, field);
                actions.AddRange(itemActions);
            }

            return actions;
        }

        /// <summary>
        /// Processes the damage-taken phase (required by interface).
        /// </summary>
        /// <param name="field">The battlefield.</param>
        /// <returns>Empty list (use ProcessDamageTaken instead).</returns>
        public async Task<List<BattleAction>> ProcessAsync(BattleField field)
        {
            return await Task.FromResult(new List<BattleAction>());
        }

        /// <summary>
        /// Processes an ability for damage-taken effects.
        /// </summary>
        private List<BattleAction> ProcessAbility(AbilityData ability, BattleSlot slot, BattleField field)
        {
            var actions = new List<BattleAction>();

            if (!ability.ListensTo(AbilityTrigger.OnDamageTaken))
                return actions;

            switch (ability.Effect)
            {
                default:
                    // No ability effects implemented yet for OnDamageTaken
                    break;
            }

            return actions;
        }

        /// <summary>
        /// Processes an item for damage-taken effects.
        /// </summary>
        private List<BattleAction> ProcessItem(ItemData item, BattleSlot slot, BattleField field)
        {
            var actions = new List<BattleAction>();

            // Items typically don't activate on damage taken
            // Add item effects as needed

            return actions;
        }

        /// <summary>
        /// Processes RaiseStatOnDamage ability effect (e.g., Anger Point).
        /// </summary>
        private List<BattleAction> ProcessRaiseStatOnDamage(AbilityData ability, BattleSlot slot, BattleField field)
        {
            var actions = new List<BattleAction>();

            if (ability.TargetStat == null)
                return actions;

            // Check if this was a critical hit (Anger Point only activates on crit)
            // Note: This would need to be tracked in DamageContext or slot state
            // For now, this is a placeholder implementation
            bool wasCriticalHit = false; // TODO: Get from damage context or slot state

            if (!wasCriticalHit)
                return actions;

            var pokemon = slot.Pokemon;
            var currentStatStage = slot.GetStatStage(ability.TargetStat.Value);

            // Check if stat is already maxed (+6 stages)
            if (currentStatStage >= 6)
                return actions;

            // Message for ability activation
            var provider = LocalizationService.Instance;
            var abilityName = ability.GetDisplayName(provider);
            actions.Add(new MessageAction(
                provider.GetString(LocalizationKey.AbilityActivated, pokemon.DisplayName, abilityName)));

            // Raise stat to maximum (+6 stages)
            int stagesToRaise = 6 - currentStatStage;
            actions.Add(new StatChangeAction(slot, slot, ability.TargetStat.Value, stagesToRaise));

            return actions;
        }

        /// <summary>
        /// Processes RaiseStatOnLowHP ability effect (e.g., Berserk).
        /// </summary>
        private List<BattleAction> ProcessRaiseStatOnLowHP(AbilityData ability, BattleSlot slot, BattleField field)
        {
            var actions = new List<BattleAction>();

            if (ability.TargetStat == null)
                return actions;

            var pokemon = slot.Pokemon;
            float hpPercent = (float)pokemon.CurrentHP / pokemon.MaxHP;

            // Berserk activates when HP drops below 50%
            float threshold = ability.Multiplier > 0 ? ability.Multiplier : 0.5f;
            if (hpPercent >= threshold)
                return actions;

            var currentStatStage = slot.GetStatStage(ability.TargetStat.Value);

            // Check if stat is already maxed (+6 stages)
            if (currentStatStage >= 6)
                return actions;

            // Message for ability activation
            var provider = LocalizationService.Instance;
            var abilityName = ability.GetDisplayName(provider);
            actions.Add(new MessageAction(
                provider.GetString(LocalizationKey.AbilityActivated, pokemon.DisplayName, abilityName)));

            // Raise own stat
            actions.Add(new StatChangeAction(slot, slot, ability.TargetStat.Value, ability.StatStages));

            return actions;
        }
    }
}
