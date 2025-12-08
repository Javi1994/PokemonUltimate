namespace PokemonUltimate.Combat.Statistics.Definition
{
    /// <summary>
    /// Interface for collecting battle statistics.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.20: Statistics System
    /// **Documentation**: See `docs/features/2-combat-system/2.20-statistics-system/architecture.md`
    /// </remarks>
    public interface IBattleStatisticsCollector
    {
        /// <summary>
        /// Gets collected statistics.
        /// </summary>
        /// <returns>The collected statistics.</returns>
        BattleStatistics GetStatistics();

        /// <summary>
        /// Resets all statistics.
        /// </summary>
        void Reset();
    }
}

