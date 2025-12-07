using System.Collections.Generic;
using System.Linq;
using PokemonUltimate.Core.Data.Blueprints;
using PokemonUltimate.Core.Data.Enums;

namespace PokemonUltimate.Combat.Helpers.TargetRedirectionResolvers
{
    /// <summary>
    /// Main resolver that coordinates all target redirection effects.
    /// Checks redirection in priority order: Follow Me/Rage Powder > Lightning Rod/Storm Drain.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.5: Combat Actions
    /// **Documentation**: See `docs/features/2-combat-system/2.5-combat-actions/architecture.md`
    /// </remarks>
    public class TargetRedirectionResolver : ITargetRedirectionResolver
    {
        private readonly FollowMeResolver _followMeResolver;
        private readonly LightningRodResolver _lightningRodResolver;

        /// <summary>
        /// Creates a new target redirection resolver.
        /// </summary>
        public TargetRedirectionResolver()
        {
            _followMeResolver = new FollowMeResolver();
            _lightningRodResolver = new LightningRodResolver();
        }

        /// <summary>
        /// Attempts to redirect the target of a move based on redirection effects.
        /// Checks redirection in priority order.
        /// </summary>
        public BattleSlot ResolveRedirection(BattleSlot user, IReadOnlyList<BattleSlot> originalTargets, MoveData move, BattleField field)
        {
            if (user == null || originalTargets == null || move == null || field == null)
                return null;

            // Only redirect single-target moves
            if (originalTargets.Count != 1)
                return null;

            // Priority 1: Follow Me / Rage Powder (affects all single-target moves)
            var followMeTarget = _followMeResolver.ResolveRedirection(user, originalTargets, move, field);
            if (followMeTarget != null)
                return followMeTarget;

            // Priority 2: Lightning Rod / Storm Drain (affects specific types)
            var abilityTarget = _lightningRodResolver.ResolveRedirection(user, originalTargets, move, field);
            if (abilityTarget != null)
                return abilityTarget;

            return null;
        }
    }
}
