using System.Collections.Generic;
using PokemonUltimate.Core.Blueprints;

namespace PokemonUltimate.Combat.Helpers
{
    /// <summary>
    /// Interface for resolving valid targets for moves based on TargetScope and battlefield state.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.5: Combat Actions
    /// **Documentation**: See `docs/features/2-combat-system/2.5-combat-actions/architecture.md`
    /// </remarks>
    public interface ITargetResolver
    {
        /// <summary>
        /// Gets all valid targets for a move based on its TargetScope.
        /// </summary>
        /// <param name="user">The slot using the move. Cannot be null.</param>
        /// <param name="move">The move data. Cannot be null.</param>
        /// <param name="field">The battlefield. Cannot be null.</param>
        /// <returns>List of valid target slots. May be empty if no valid targets.</returns>
        List<BattleSlot> GetValidTargets(BattleSlot user, MoveData move, BattleField field);
    }
}
