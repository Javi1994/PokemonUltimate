namespace PokemonUltimate.Core.Extensions
{
    /// <summary>
    /// Extension methods for level validation and operations.
    /// </summary>
    /// <remarks>
    /// **Feature**: 1: Game Data
    /// **Sub-Feature**: 1.10: Enums & Constants
    /// **Documentation**: See `docs/features/1-game-data/1.10-enums-constants/README.md`
    /// </remarks>
    public static class LevelExtensions
    {
        /// <summary>
        /// Checks if level is valid (between 1 and 100).
        /// </summary>
        /// <param name="level">The level to check.</param>
        /// <returns>True if level is valid, false otherwise.</returns>
        public static bool IsValidLevel(this int level)
        {
            return level >= 1 && level <= 100;
        }
    }
}
