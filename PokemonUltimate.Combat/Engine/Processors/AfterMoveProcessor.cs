using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PokemonUltimate.Combat.Actions;
using PokemonUltimate.Combat.Engine.Processors.Definition;
using PokemonUltimate.Combat.Field;
using PokemonUltimate.Core.Data.Blueprints;
using PokemonUltimate.Core.Data.Constants;
using PokemonUltimate.Core.Data.Enums;
using PokemonUltimate.Localization.Constants;
using PokemonUltimate.Localization.Extensions;
using PokemonUltimate.Localization.Services;

namespace PokemonUltimate.Combat.Engine.Processors
{
    /// <summary>
    /// Processes abilities and items that activate after a Pokemon uses a move.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.6: Combat Engine
    /// **Documentation**: See `docs/features/2-combat-system/2.6-combat-engine/architecture.md`
    /// </remarks>
    public class AfterMoveProcessor : IActionGeneratingPhaseProcessor
    {
        /// <summary>
        /// Gets the phase this processor handles.
        /// </summary>
        public BattlePhase Phase => BattlePhase.AfterMove;

        /// <summary>
        /// Processes abilities and items that activate after a move.
        /// </summary>
        /// <param name="slot">The slot that used the move.</param>
        /// <param name="field">The battlefield.</param>
        /// <returns>List of actions generated.</returns>
        public List<BattleAction> ProcessAfterMove(BattleSlot slot, BattleField field)
        {
            if (slot == null)
                throw new ArgumentNullException(nameof(slot));
            if (field == null)
                throw new ArgumentNullException(nameof(field), ErrorMessages.FieldCannotBeNull);

            var actions = new List<BattleAction>();

            var pokemon = slot.Pokemon;
            if (pokemon == null)
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
        /// Processes the after-move phase (required by interface).
        /// </summary>
        /// <param name="field">The battlefield.</param>
        /// <returns>Empty list (use ProcessAfterMove instead).</returns>
        public async Task<List<BattleAction>> ProcessAsync(BattleField field)
        {
            return await Task.FromResult(new List<BattleAction>());
        }

        /// <summary>
        /// Processes an ability for after-move effects.
        /// </summary>
        private List<BattleAction> ProcessAbility(AbilityData ability, BattleSlot slot, BattleField field)
        {
            var actions = new List<BattleAction>();

            if (!ability.ListensTo(AbilityTrigger.OnAfterMove))
                return actions;

            switch (ability.Effect)
            {
                case AbilityEffect.RaiseStatOnKO:
                    // Example: Moxie
                    actions.AddRange(ProcessRaiseStatOnKO(ability, slot, field));
                    break;

                    // Add other ability effects as needed
            }

            return actions;
        }

        /// <summary>
        /// Processes an item for after-move effects.
        /// </summary>
        private List<BattleAction> ProcessItem(ItemData item, BattleSlot slot, BattleField field)
        {
            var actions = new List<BattleAction>();

            if (!item.ListensTo(ItemTrigger.OnDamageDealt))
                return actions;

            // Check if item has recoil (Life Orb)
            if (item.RecoilPercent > 0)
            {
                actions.AddRange(ProcessRecoilDamage(item, slot, field));
            }

            return actions;
        }

        /// <summary>
        /// Processes RaiseStatOnKO ability effect (e.g., Moxie).
        /// </summary>
        private List<BattleAction> ProcessRaiseStatOnKO(AbilityData ability, BattleSlot slot, BattleField field)
        {
            var actions = new List<BattleAction>();

            if (ability.TargetStat == null)
                return actions;

            // Check if any opponent Pokemon fainted this turn
            var opposingSide = field.GetOppositeSide(slot.Side);
            bool anyFainted = opposingSide.Slots.Any(s => !s.IsEmpty && s.Pokemon.IsFainted);

            if (!anyFainted)
                return actions;

            // Message for ability activation
            var provider = LocalizationService.Instance;
            var abilityName = ability.GetDisplayName(provider);
            actions.Add(new MessageAction(
                provider.GetString(LocalizationKey.AbilityActivated, slot.Pokemon.DisplayName, abilityName)));

            // Raise own stat
            actions.Add(new StatChangeAction(slot, slot, ability.TargetStat.Value, ability.StatStages));

            return actions;
        }

        /// <summary>
        /// Processes RecoilDamage item effect (e.g., Life Orb).
        /// </summary>
        private List<BattleAction> ProcessRecoilDamage(ItemData item, BattleSlot slot, BattleField field)
        {
            var actions = new List<BattleAction>();

            // Check if damage was dealt this turn
            var opposingSide = field.GetOppositeSide(slot.Side);
            bool damageWasDealt = opposingSide.GetActiveSlots()
                .Any(s => s.PhysicalDamageTakenThisTurn > 0 || s.SpecialDamageTakenThisTurn > 0);

            if (!damageWasDealt)
                return actions;

            // Calculate recoil damage (typically 10% of max HP for Life Orb)
            int recoilDamage = slot.Pokemon.MaxHP / 10;
            if (recoilDamage < 1)
                recoilDamage = 1;

            // Message for item activation
            var provider = LocalizationService.Instance;
            var itemName = item.GetDisplayName(provider);
            actions.Add(new MessageAction(
                provider.GetString(LocalizationKey.ItemActivated, slot.Pokemon.DisplayName, itemName)));

            // Apply recoil damage directly
            slot.Pokemon.TakeDamage(recoilDamage);
            actions.Add(new MessageAction(
                provider.GetString(LocalizationKey.HurtByRecoil, slot.Pokemon.DisplayName)));

            return actions;
        }
    }
}
