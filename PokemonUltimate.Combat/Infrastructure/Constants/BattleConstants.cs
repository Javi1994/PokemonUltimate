namespace PokemonUltimate.Combat.Infrastructure.Constants
{
    /// <summary>
    /// Constants for battle system limits and thresholds.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.6: Combat Engine
    /// **Documentation**: See `docs/features/2-combat-system/2.6-combat-engine/architecture.md`
    /// </remarks>
    public static class BattleConstants
    {
        /// <summary>
        /// Maximum number of turns before battle is considered infinite loop.
        /// </summary>
        public const int MaxTurns = 1000;

        /// <summary>
        /// Maximum number of queue iterations before considering infinite loop.
        /// </summary>
        public const int MaxQueueIterations = 1000;

        /// <summary>
        /// Number of consecutive turns without HP changes before battle is considered stuck (infinite loop).
        /// </summary>
        public const int MaxTurnsWithoutHPChange = 10;
    }
}
