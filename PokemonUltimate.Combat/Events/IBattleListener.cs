using System.Collections.Generic;
using PokemonUltimate.Combat.Actions;

namespace PokemonUltimate.Combat.Events
{
    /// <summary>
    /// Interface for objects that react to battle events (abilities, items, status effects).
    /// Returns actions to be enqueued when triggered.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.9: Abilities & Items
    /// **Documentation**: See `docs/features/2-combat-system/2.9-abilities-items/architecture.md`
    /// </remarks>
    public interface IBattleListener
    {
        /// <summary>
        /// Called when a battle trigger occurs.
        /// Returns actions to be enqueued and processed.
        /// </summary>
        /// <param name="trigger">The battle event that occurred.</param>
        /// <param name="holder">The slot holding this listener (Pokemon with ability/item).</param>
        /// <param name="field">The battlefield context.</param>
        /// <returns>Actions to execute, or empty if this listener doesn't respond to this trigger.</returns>
        IEnumerable<BattleAction> OnTrigger(BattleTrigger trigger, BattleSlot holder, BattleField field);
    }
}

