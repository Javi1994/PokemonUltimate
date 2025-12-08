using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PokemonUltimate.Combat.Actions;
using PokemonUltimate.Combat.Engine.Processors.Definition;
using PokemonUltimate.Combat.Field;
using PokemonUltimate.Combat.Handlers.Registry;
using PokemonUltimate.Combat.Utilities.Extensions;
using PokemonUltimate.Core.Data.Constants;
using PokemonUltimate.Core.Data.Enums;

namespace PokemonUltimate.Combat.Engine.Processors
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
        private readonly CombatEffectHandlerRegistry _handlerRegistry;

        /// <summary>
        /// Creates a new ContactReceivedProcessor.
        /// </summary>
        /// <param name="handlerRegistry">The handler registry. If null, creates and initializes a new one.</param>
        public ContactReceivedProcessor(CombatEffectHandlerRegistry handlerRegistry = null)
        {
            _handlerRegistry = handlerRegistry ?? CombatEffectHandlerRegistry.CreateDefault();
        }

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
                throw new ArgumentNullException(nameof(field), ErrorMessages.FieldCannotBeNull);

            var actions = new List<BattleAction>();

            var pokemon = defender.Pokemon;
            if (pokemon == null)
                return actions;

            // Process ability using handler registry (pass attacker for contact effects)
            if (pokemon.Ability != null)
            {
                var abilityActions = _handlerRegistry.ProcessAbility(
                    pokemon.Ability, defender, field, AbilityTrigger.OnContactReceived, attacker);
                actions.AddRange(abilityActions);
            }

            // Process item using handler registry (pass attacker for contact effects)
            if (pokemon.HeldItem != null)
            {
                var itemActions = _handlerRegistry.ProcessItem(
                    pokemon.HeldItem, defender, field, ItemTrigger.OnContactReceived, attacker);
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
    }
}
