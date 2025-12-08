using PokemonUltimate.Combat.Field;

namespace PokemonUltimate.Combat.Utilities.Extensions
{
    /// <summary>
    /// Extension methods for BattleSlot.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.1: Battle Foundation
    /// **Documentation**: See `docs/features/2-combat-system/2.1-battle-foundation/architecture.md`
    /// </remarks>
    public static class BattleSlotExtensions
    {
        /// <summary>
        /// Checks if a slot has an active (non-empty, non-fainted) Pokemon.
        /// </summary>
        /// <param name="slot">The slot to check.</param>
        /// <returns>True if the slot has an active Pokemon, false otherwise.</returns>
        public static bool IsActive(this BattleSlot slot)
        {
            return slot != null && !slot.IsEmpty && !slot.HasFainted;
        }
    }
}
