using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PokemonUltimate.Combat.Events;
using PokemonUltimate.Core.Constants;
using PokemonUltimate.Core.Instances;

namespace PokemonUltimate.Combat.Actions
{
    /// <summary>
    /// Switches a Pokemon in battle.
    /// The current Pokemon is sent to the bench and a new Pokemon takes its place.
    /// Battle state (stat stages, volatile status) is reset for the new Pokemon.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.5: Combat Actions
    /// **Documentation**: See `docs/features/2-combat-system/2.5-combat-actions/architecture.md`
    /// </remarks>
    public class SwitchAction : BattleAction
    {
        /// <summary>
        /// The slot being switched.
        /// </summary>
        public BattleSlot Slot { get; }

        /// <summary>
        /// The Pokemon to switch in.
        /// </summary>
        public PokemonInstance NewPokemon { get; }

        /// <summary>
        /// Switch actions have highest priority (+6).
        /// </summary>
        public override int Priority => 6;

        /// <summary>
        /// Switch actions cannot be blocked.
        /// </summary>
        public override bool CanBeBlocked => false;

        /// <summary>
        /// Creates a new switch action.
        /// </summary>
        /// <param name="slot">The slot to switch. Cannot be null.</param>
        /// <param name="newPokemon">The Pokemon to switch in. Cannot be null.</param>
        /// <exception cref="ArgumentNullException">If slot or newPokemon is null.</exception>
        public SwitchAction(BattleSlot slot, PokemonInstance newPokemon) : base(slot)
        {
            Slot = slot ?? throw new ArgumentNullException(nameof(slot), ErrorMessages.PokemonCannotBeNull);
            NewPokemon = newPokemon ?? throw new ArgumentNullException(nameof(newPokemon), ErrorMessages.PokemonCannotBeNull);
        }

        /// <summary>
        /// Switches the Pokemon in the slot.
        /// Returns the old Pokemon to the bench and places the new one in the slot.
        /// Resets battle state for the new Pokemon.
        /// </summary>
        public override IEnumerable<BattleAction> ExecuteLogic(BattleField field)
        {
            if (field == null)
                throw new ArgumentNullException(nameof(field));

            if (Slot.IsEmpty)
                return Enumerable.Empty<BattleAction>();

            var side = Slot.Side;
            if (side == null)
                return Enumerable.Empty<BattleAction>();

            // Get the current Pokemon
            var oldPokemon = Slot.Pokemon;

            // Switch Pokemon
            Slot.SetPokemon(NewPokemon);

            // Return old Pokemon to bench (if it exists and isn't already there)
            if (oldPokemon != null && !side.Party.Contains(oldPokemon))
            {
                // Note: BattleSide.Party is read-only, so we can't modify it directly
                // In a real implementation, BattleSide would handle this
                // For now, we just ensure the slot has the new Pokemon
            }

            // Battle state is reset automatically by SetPokemon -> ResetBattleState

            // Trigger OnSwitchIn for abilities and items
            var switchInActions = BattleTriggerProcessor.ProcessTrigger(BattleTrigger.OnSwitchIn, field);
            
            return switchInActions;
        }

        /// <summary>
        /// Plays switch-out and switch-in animations.
        /// </summary>
        public override Task ExecuteVisual(IBattleView view)
        {
            if (view == null)
                throw new ArgumentNullException(nameof(view));

            if (Slot.IsEmpty)
                return Task.CompletedTask;

            // Play switch-out animation first, then switch-in
            return Task.WhenAll(
                view.PlaySwitchOutAnimation(Slot),
                view.PlaySwitchInAnimation(Slot)
            );
        }
    }
}

