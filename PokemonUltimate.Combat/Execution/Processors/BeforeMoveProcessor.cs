using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PokemonUltimate.Combat.Actions;
using PokemonUltimate.Combat.Execution.Battle;
using PokemonUltimate.Combat.Execution.Processors.Definition;
using PokemonUltimate.Combat.Foundation.Field;
using PokemonUltimate.Core.Data.Blueprints;
using PokemonUltimate.Core.Data.Constants;
using PokemonUltimate.Core.Data.Enums;

namespace PokemonUltimate.Combat.Execution.Processors
{
    /// <summary>
    /// Processes abilities and items that activate before a Pokemon uses a move.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.6: Combat Engine
    /// **Documentation**: See `docs/features/2-combat-system/2.6-combat-engine/architecture.md`
    /// </remarks>
    public class BeforeMoveProcessor : IActionGeneratingPhaseProcessor
    {
        /// <summary>
        /// Gets the phase this processor handles.
        /// </summary>
        public BattlePhase Phase => BattlePhase.BeforeMove;

        /// <summary>
        /// Processes abilities and items that activate before a move.
        /// </summary>
        /// <param name="slot">The slot using the move.</param>
        /// <param name="field">The battlefield.</param>
        /// <returns>List of actions generated.</returns>
        public List<BattleAction> ProcessBeforeMove(BattleSlot slot, BattleField field)
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
        /// Processes the before-move phase (required by interface).
        /// </summary>
        /// <param name="field">The battlefield.</param>
        /// <returns>Empty list (use ProcessBeforeMove instead).</returns>
        public async Task<List<BattleAction>> ProcessAsync(BattleField field)
        {
            return await Task.FromResult(new List<BattleAction>());
        }

        /// <summary>
        /// Processes an ability for before-move effects.
        /// </summary>
        private List<BattleAction> ProcessAbility(AbilityData ability, BattleSlot slot, BattleField field)
        {
            var actions = new List<BattleAction>();

            if (!ability.ListensTo(AbilityTrigger.OnBeforeMove))
                return actions;

            switch (ability.Effect)
            {
                case AbilityEffect.SkipTurn:
                    // Example: Truant
                    actions.AddRange(ProcessSkipTurn(ability, slot, field));
                    break;

                    // Add other ability effects as needed
            }

            return actions;
        }

        /// <summary>
        /// Processes an item for before-move effects.
        /// </summary>
        private List<BattleAction> ProcessItem(ItemData item, BattleSlot slot, BattleField field)
        {
            var actions = new List<BattleAction>();

            // Items typically don't activate before moves
            // Add item effects as needed

            return actions;
        }

        /// <summary>
        /// Processes SkipTurn ability effect (e.g., Truant).
        /// </summary>
        private List<BattleAction> ProcessSkipTurn(AbilityData ability, BattleSlot slot, BattleField field)
        {
            var actions = new List<BattleAction>();

            // Implementation would check Truant state and potentially block the move
            // This is a simplified version - full implementation would track state

            return actions;
        }
    }
}
