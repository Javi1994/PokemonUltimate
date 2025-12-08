using System.Collections.Generic;
using System.Threading.Tasks;
using PokemonUltimate.Combat.Actions;
using PokemonUltimate.Combat.Field;
using PokemonUltimate.Core.Domain.Instances.Pokemon;

namespace PokemonUltimate.Combat.Infrastructure.Providers.Definition
{
    /// <summary>
    /// Base class for action providers with default implementation for automatic switches.
    /// Provides a virtual method for selecting Pokemon during automatic switches (when Pokemon faint).
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.7: Integration
    /// </remarks>
    public abstract class ActionProviderBase : IActionProvider
    {
        /// <summary>
        /// Gets the action the Pokemon wants to perform this turn.
        /// </summary>
        /// <param name="field">The current battlefield state.</param>
        /// <param name="mySlot">The slot requesting an action.</param>
        /// <returns>The action to perform this turn.</returns>
        public abstract Task<BattleAction> GetAction(BattleField field, BattleSlot mySlot);

        /// <summary>
        /// Selects a Pokemon to switch in when the current Pokemon faints or slot is empty.
        /// This is called for mandatory switches (automatic switches), not strategic switches.
        /// </summary>
        /// <param name="field">The current battlefield state.</param>
        /// <param name="mySlot">The slot that needs a switch (empty or contains fainted Pokemon).</param>
        /// <param name="availablePokemon">List of available Pokemon that can be switched in. Already filtered to exclude reserved Pokemon.</param>
        /// <returns>The Pokemon to switch in, or null if no selection can be made (triggers random selection).</returns>
        /// <remarks>
        /// Default implementation returns null, which triggers random selection in FaintedPokemonCheckStep.
        /// Override this method to provide custom selection logic for automatic switches.
        /// </remarks>
        public virtual Task<PokemonInstance> SelectAutoSwitch(BattleField field, BattleSlot mySlot, IReadOnlyList<PokemonInstance> availablePokemon)
        {
            // Default: return null to use random selection
            return Task.FromResult<PokemonInstance>(null);
        }
    }
}
