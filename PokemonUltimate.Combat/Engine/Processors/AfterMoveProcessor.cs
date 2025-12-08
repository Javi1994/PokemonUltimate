using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PokemonUltimate.Combat.Actions;
using PokemonUltimate.Combat.Engine.Processors.Definition;
using PokemonUltimate.Combat.Field;
using PokemonUltimate.Combat.Handlers.Registry;
using PokemonUltimate.Core.Data.Constants;
using PokemonUltimate.Core.Data.Enums;

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
        private readonly CombatEffectHandlerRegistry _handlerRegistry;

        /// <summary>
        /// Creates a new AfterMoveProcessor.
        /// </summary>
        /// <param name="handlerRegistry">The handler registry. If null, creates and initializes a new one.</param>
        public AfterMoveProcessor(CombatEffectHandlerRegistry handlerRegistry = null)
        {
            _handlerRegistry = handlerRegistry ?? CombatEffectHandlerRegistry.CreateDefault();
        }

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

            // Process ability using handler registry
            if (pokemon.Ability != null)
            {
                var abilityActions = _handlerRegistry.ProcessAbility(
                    pokemon.Ability, slot, field, AbilityTrigger.OnAfterMove);
                actions.AddRange(abilityActions);
            }

            // Process item using handler registry
            if (pokemon.HeldItem != null)
            {
                var itemActions = _handlerRegistry.ProcessItem(
                    pokemon.HeldItem, slot, field, ItemTrigger.OnDamageDealt);
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
    }
}
