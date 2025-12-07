using System.Collections.Generic;

namespace PokemonUltimate.Combat.Foundation.Constants
{
    /// <summary>
    /// Constants for move-related mechanics.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.5: Combat Actions
    /// **Documentation**: See `docs/features/2-combat-system/2.5-combat-actions/architecture.md`
    /// </remarks>
    public static class MoveConstants
    {
        /// <summary>
        /// Names of moves that make the user semi-invulnerable (Dig, Dive, Fly, etc.).
        /// Used for checking if a move can hit semi-invulnerable targets.
        /// </summary>
        public static readonly HashSet<string> SemiInvulnerableMoveNames = new HashSet<string>(System.StringComparer.OrdinalIgnoreCase)
        {
            "dig",
            "dive",
            "fly",
            "bounce",
            "shadow force",
            "phantom force"
        };

        /// <summary>
        /// Names of moves that can hit Dig users.
        /// </summary>
        public static readonly HashSet<string> DigCounterMoveNames = new HashSet<string>(System.StringComparer.OrdinalIgnoreCase)
        {
            "earthquake",
            "magnitude"
        };

        /// <summary>
        /// Names of moves that can hit Dive users.
        /// </summary>
        public static readonly HashSet<string> DiveCounterMoveNames = new HashSet<string>(System.StringComparer.OrdinalIgnoreCase)
        {
            "surf",
            "whirlpool"
        };
    }
}
