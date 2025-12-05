namespace PokemonUltimate.Combat.Constants
{
    /// <summary>
    /// Constants for status condition effects.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.7: Status Conditions
    /// **Documentation**: See `docs/features/2-combat-system/2.7-status-conditions/architecture.md`
    /// </remarks>
    public static class StatusConstants
    {
        /// <summary>
        /// Speed multiplier when paralyzed (50% speed).
        /// </summary>
        public const float ParalysisSpeedMultiplier = 0.5f;

        /// <summary>
        /// Chance to be fully paralyzed when attempting to move (25%).
        /// </summary>
        public const int ParalysisFullParalysisChance = 25;
    }
}
