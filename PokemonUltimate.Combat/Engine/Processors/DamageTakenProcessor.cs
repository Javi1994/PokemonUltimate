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
        private readonly CombatEffectHandlerRegistry _handlerRegistry;

        /// <summary>
        /// Creates a new DamageTakenProcessor.
        /// </summary>
        /// <param name="handlerRegistry">The handler registry. If null, creates and initializes a new one.</param>
        public DamageTakenProcessor(CombatEffectHandlerRegistry handlerRegistry = null)
        {
            _handlerRegistry = handlerRegistry ?? CombatEffectHandlerRegistry.CreateDefault();
        }

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

            // Process ability using handler registry
            if (pokemon.Ability != null)
            {
                var abilityActions = _handlerRegistry.ProcessAbility(
                    pokemon.Ability, slot, field, AbilityTrigger.OnDamageTaken);
                actions.AddRange(abilityActions);
            }

            // Process item using handler registry
            if (pokemon.HeldItem != null)
            {
                // Items typically don't activate on damage taken, but check anyway
                var itemActions = _handlerRegistry.ProcessItem(
                    pokemon.HeldItem, slot, field, ItemTrigger.OnLowHP);
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
    }
}
