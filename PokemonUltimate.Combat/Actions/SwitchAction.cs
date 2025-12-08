using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PokemonUltimate.Combat.Foundation.Field;
using PokemonUltimate.Combat.Integration.View;
using PokemonUltimate.Combat.Integration.View.Definition;
using PokemonUltimate.Core.Data.Blueprints;
using PokemonUltimate.Core.Data.Constants;
using PokemonUltimate.Core.Data.Enums;
using PokemonUltimate.Core.Domain.Instances;
using PokemonUltimate.Core.Domain.Instances.Pokemon;

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
        /// The Pokemon that was switched out (set during ExecuteLogic).
        /// Null if slot was empty or action hasn't executed yet.
        /// </summary>
        public PokemonInstance OldPokemon { get; private set; }

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
        public SwitchAction(
            BattleSlot slot,
            PokemonInstance newPokemon) : base(slot)
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

            // Mark Pokemon as switching out (for Pursuit detection)
            Slot.AddVolatileStatus(VolatileStatus.SwitchingOut);

            // Get the current Pokemon and store it for observers/logging
            OldPokemon = Slot.Pokemon;

            // Switch Pokemon
            // Note: SetPokemon automatically resets battle state for the new Pokemon
            Slot.SetPokemon(NewPokemon);

            // Note: The party management is handled externally by BattleField initialization.
            // BattleSide.Party is a read-only reference to the party provided during initialization.
            // When switching, we simply replace the Pokemon in the slot. The old Pokemon
            // remains in the party list (if it was there) and can be switched back in later.
            // If the old Pokemon is not in the party, it means it was a temporary instance
            // or the party structure is managed elsewhere in the system.

            // Battle state is reset automatically by SetPokemon -> ResetBattleState

            // Note: Switch-in effects (entry hazards + abilities + items) are processed by ActionProcessorObserver
            // This keeps actions simple and decoupled from processors

            return Enumerable.Empty<BattleAction>();
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

