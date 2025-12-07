using PokemonUltimate.Core.Data.Constants;

namespace PokemonUltimate.Core.Utilities.Extensions
{
    /// <summary>
    /// Extension methods for friendship values.
    /// </summary>
    /// <remarks>
    /// **Feature**: 1: Game Data
    /// **Sub-Feature**: 1.10: Enums & Constants
    /// **Documentation**: See `docs/features/1-game-data/1.10-enums-constants/README.md`
    /// </remarks>
    public static class FriendshipExtensions
    {
        /// <summary>
        /// Clamps friendship value to valid range (0-255).
        /// </summary>
        /// <param name="friendship">The friendship value to clamp.</param>
        /// <returns>The clamped friendship value.</returns>
        public static int ClampFriendship(this int friendship)
        {
            return System.Math.Max(0, System.Math.Min(CoreConstants.MaxFriendship, friendship));
        }
    }
}
