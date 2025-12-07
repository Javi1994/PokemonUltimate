using System.Collections.Generic;
using PokemonUltimate.Core.Data.Blueprints;

namespace PokemonUltimate.Combat.Helpers
{
    /// <summary>
    /// Interface for resolving target redirection effects (Follow Me, Lightning Rod, etc.).
    /// Determines if a move's target should be redirected to a different slot.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.5: Combat Actions
    /// **Documentation**: See `docs/features/2-combat-system/2.5-combat-actions/architecture.md`
    /// </remarks>
    public interface ITargetRedirectionResolver
    {
        /// <summary>
        /// Attempts to redirect the target of a move based on redirection effects.
        /// </summary>
        /// <param name="user">The slot using the move. Cannot be null.</param>
        /// <param name="originalTargets">The original valid targets before redirection. Cannot be null.</param>
        /// <param name="move">The move being used. Cannot be null.</param>
        /// <param name="field">The battlefield. Cannot be null.</param>
        /// <returns>
        /// The redirected target slot if redirection occurs, or null if no redirection.
        /// If multiple redirectors are active, returns the highest priority one.
        /// </returns>
        BattleSlot ResolveRedirection(BattleSlot user, IReadOnlyList<BattleSlot> originalTargets, MoveData move, BattleField field);
    }
}
