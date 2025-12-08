using System.Collections.Generic;
using System.Linq;
using PokemonUltimate.Combat.Field;
using PokemonUltimate.Combat.Utilities.Definition;
using PokemonUltimate.Combat.Utilities.Extensions;
using PokemonUltimate.Core.Data.Blueprints;
using PokemonUltimate.Core.Data.Enums;

namespace PokemonUltimate.Combat.Utilities.TargetRedirectionResolvers
{
    /// <summary>
    /// Resolves Lightning Rod and Storm Drain ability redirection effects.
    /// These abilities redirect Electric and Water moves respectively.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.5: Combat Actions
    /// **Documentation**: See `docs/features/2-combat-system/2.5-combat-actions/architecture.md`
    /// </remarks>
    public class LightningRodResolver : ITargetRedirectionResolver
    {
        /// <summary>
        /// Attempts to redirect the target using Lightning Rod or Storm Drain abilities.
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

            // Check for Lightning Rod (redirects Electric moves)
            if (move.Type == PokemonType.Electric)
            {
                var lightningRodUser = opposingSide.GetActiveSlots()
                    .FirstOrDefault(slot => slot.IsActive() &&
                        slot.Pokemon?.Ability?.Name == "Lightning Rod");

                if (lightningRodUser != null && lightningRodUser != originalTargets[0])
                {
                    return lightningRodUser;
                }
            }

            // Check for Storm Drain (redirects Water moves)
            if (move.Type == PokemonType.Water)
            {
                var stormDrainUser = opposingSide.GetActiveSlots()
                    .FirstOrDefault(slot => slot.IsActive() &&
                        slot.Pokemon?.Ability?.Name == "Storm Drain");

                if (stormDrainUser != null && stormDrainUser != originalTargets[0])
                {
                    return stormDrainUser;
                }
            }

            return null;
        }
    }
}
