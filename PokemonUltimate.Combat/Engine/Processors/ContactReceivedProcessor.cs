using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PokemonUltimate.Combat.Actions;
using PokemonUltimate.Combat.Extensions;
using PokemonUltimate.Content.Extensions;
using PokemonUltimate.Core.Blueprints;
using PokemonUltimate.Core.Enums;
using PokemonUltimate.Core.Extensions;
using PokemonUltimate.Core.Instances;

namespace PokemonUltimate.Combat.Processors.Phases
{
    /// <summary>
    /// Processes abilities and items that activate when a Pokemon receives contact from a move.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.6: Combat Engine
    /// **Documentation**: See `docs/features/2-combat-system/2.6-combat-engine/architecture.md`
    /// </remarks>
    public class ContactReceivedProcessor : IActionGeneratingPhaseProcessor
    {
        /// <summary>
        /// Gets the phase this processor handles.
        /// </summary>
        public BattlePhase Phase => BattlePhase.ContactReceived;

        /// <summary>
        /// Processes abilities and items that activate when contact is received.
        /// </summary>
        /// <param name="defender">The slot receiving contact.</param>
        /// <param name="attacker">The slot making contact.</param>
        /// <param name="field">The battlefield.</param>
        /// <returns>List of actions generated.</returns>
        public List<BattleAction> ProcessContactReceived(BattleSlot defender, BattleSlot attacker, BattleField field)
        {
            if (defender == null)
                throw new ArgumentNullException(nameof(defender));
            if (attacker == null)
                throw new ArgumentNullException(nameof(attacker));
            if (field == null)
                throw new ArgumentNullException(nameof(field), Core.Constants.ErrorMessages.FieldCannotBeNull);

            var actions = new List<BattleAction>();

            var pokemon = defender.Pokemon;
            if (pokemon == null)
                return actions;

            // Process ability
            if (pokemon.Ability != null)
            {
                var abilityActions = ProcessAbility(pokemon.Ability, defender, attacker, field);
                actions.AddRange(abilityActions);
            }

            // Process item
            if (pokemon.HeldItem != null)
            {
                var itemActions = ProcessItem(pokemon.HeldItem, defender, attacker, field);
                actions.AddRange(itemActions);
            }

            return actions;
        }

        /// <summary>
        /// Processes the contact-received phase (required by interface).
        /// </summary>
        /// <param name="field">The battlefield.</param>
        /// <returns>Empty list (use ProcessContactReceived instead).</returns>
        public async Task<List<BattleAction>> ProcessAsync(BattleField field)
        {
            return await Task.FromResult(new List<BattleAction>());
        }

        /// <summary>
        /// Processes an ability for contact-received effects.
        /// </summary>
        private List<BattleAction> ProcessAbility(AbilityData ability, BattleSlot defender, BattleSlot attacker, BattleField field)
        {
            var actions = new List<BattleAction>();

            if (!ability.ListensTo(AbilityTrigger.OnContactReceived))
                return actions;

            switch (ability.Effect)
            {
                case AbilityEffect.ChanceToStatusOnContact:
                    // Example: Static, Poison Point
                    actions.AddRange(ProcessChanceToStatusOnContact(ability, defender, attacker, field));
                    break;

                case AbilityEffect.DamageOnContact:
                    // Example: Rough Skin, Iron Barbs
                    actions.AddRange(ProcessDamageOnContact(ability, defender, attacker, field));
                    break;

                    // Add other ability effects as needed
            }

            return actions;
        }

        /// <summary>
        /// Processes an item for contact-received effects.
        /// </summary>
        private List<BattleAction> ProcessItem(ItemData item, BattleSlot defender, BattleSlot attacker, BattleField field)
        {
            var actions = new List<BattleAction>();

            if (!item.ListensTo(ItemTrigger.OnContactReceived))
                return actions;

            // Rocky Helmet deals contact damage
            actions.AddRange(ProcessContactDamage(item, defender, attacker, field));

            return actions;
        }

        /// <summary>
        /// Processes ChanceToStatusOnContact ability effect (e.g., Static).
        /// </summary>
        private List<BattleAction> ProcessChanceToStatusOnContact(AbilityData ability, BattleSlot defender, BattleSlot attacker, BattleField field)
        {
            var actions = new List<BattleAction>();

            if (ability.StatusEffect == null || attacker == null || !attacker.IsActive())
                return actions;

            // Check if attacker already has a status condition
            if (attacker.Pokemon.Status != PersistentStatus.None)
                return actions;

            // Calculate chance (default 30% for Static)
            float chance = ability.EffectChance > 0 ? ability.EffectChance : 0.30f;

            // Use random check (would need IRandomProvider injected)
            var random = new System.Random();
            if (random.NextDouble() >= chance)
                return actions;

            // Message for ability activation
            var provider = Core.Localization.LocalizationManager.Instance;
            var abilityName = ability.GetDisplayName(provider);
            actions.Add(new MessageAction(
                provider.GetString(Core.Localization.LocalizationKey.AbilityActivated, defender.Pokemon.DisplayName, abilityName)));

            // Apply status to attacker
            actions.Add(new ApplyStatusAction(defender, attacker, ability.StatusEffect.Value));

            return actions;
        }

        /// <summary>
        /// Processes DamageOnContact ability effect (e.g., Rough Skin).
        /// </summary>
        private List<BattleAction> ProcessDamageOnContact(AbilityData ability, BattleSlot defender, BattleSlot attacker, BattleField field)
        {
            var actions = new List<BattleAction>();

            if (attacker == null || !attacker.IsActive())
                return actions;

            // Calculate damage (default 1/8 of attacker's max HP for Rough Skin)
            float damageMultiplier = ability.Multiplier > 0 ? ability.Multiplier : 0.125f;
            int damage = (int)(attacker.Pokemon.MaxHP * damageMultiplier);
            if (damage < 1)
                damage = 1;

            // Message for ability activation
            var provider = Core.Localization.LocalizationManager.Instance;
            var abilityName = ability.GetDisplayName(provider);
            actions.Add(new MessageAction(
                provider.GetString(Core.Localization.LocalizationKey.AbilityActivated, defender.Pokemon.DisplayName, abilityName)));

            // Apply damage using ContactDamageAction
            actions.Add(new ContactDamageAction(attacker, damage, abilityName));

            return actions;
        }

        /// <summary>
        /// Processes ContactDamage item effect (e.g., Rocky Helmet).
        /// </summary>
        private List<BattleAction> ProcessContactDamage(ItemData item, BattleSlot defender, BattleSlot attacker, BattleField field)
        {
            var actions = new List<BattleAction>();

            if (attacker == null || attacker.IsEmpty || attacker.HasFainted)
                return actions;

            // Rocky Helmet deals 1/6 of attacker's max HP
            int damage = attacker.Pokemon.MaxHP / 6;
            if (damage < 1)
                damage = 1;

            // Message for item activation
            var provider = Core.Localization.LocalizationManager.Instance;
            var itemName = item.GetLocalizedName(provider);
            actions.Add(new MessageAction(
                provider.GetString(Core.Localization.LocalizationKey.ItemActivated, defender.Pokemon.DisplayName, itemName)));

            // Apply damage using ContactDamageAction
            actions.Add(new ContactDamageAction(attacker, damage, itemName));

            return actions;
        }
    }
}
