using System.Collections.Generic;
using System.Linq;
using PokemonUltimate.Combat.Extensions;
using PokemonUltimate.Core.Blueprints;
using PokemonUltimate.Core.Enums;

namespace PokemonUltimate.Combat.Helpers.TargetRedirectionResolvers
{
    /// <summary>
    /// Resolves Follow Me and Rage Powder redirection effects.
    /// These moves redirect all single-target moves to the user.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.5: Combat Actions
    /// **Documentation**: See `docs/features/2-combat-system/2.5-combat-actions/architecture.md`
    /// </remarks>
    public class FollowMeResolver : ITargetRedirectionResolver
    {
        /// <summary>
        /// Attempts to redirect the target using Follow Me or Rage Powder.
        /// </summary>
        public BattleSlot ResolveRedirection(BattleSlot user, IReadOnlyList<BattleSlot> originalTargets, MoveData move, BattleField field)
        {
            if (user == null || originalTargets == null || move == null || field == null)
                return null;

            // Only redirect single-target moves
            if (originalTargets.Count != 1)
                return null;

            // Get opposing side
            var opposingSide = field.GetOppositeSide(user.Side);
            if (opposingSide == null)
                return null;

            // Check for Follow Me or Rage Powder volatile status
            var redirector = opposingSide.GetActiveSlots()
                .FirstOrDefault(slot => slot.IsActive() &&
                    (slot.HasVolatileStatus(VolatileStatus.FollowMe) ||
                     slot.HasVolatileStatus(VolatileStatus.RagePowder)));

            // Only redirect if the redirector is not the original target
            // (Follow Me doesn't redirect moves already targeting the user)
            if (redirector != null && redirector != originalTargets[0])
            {
                return redirector;
            }

            return null;
        }
    }
}
