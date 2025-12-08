using System.Collections.Generic;
using PokemonUltimate.Combat.Field;
using PokemonUltimate.Combat.Infrastructure.Constants;

namespace PokemonUltimate.Combat.Infrastructure.Statistics.Definition
{
    /// <summary>
    /// Interface for collecting battle statistics.
    /// Provides structured data about what happened during the battle.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.6: Combat Engine
    /// **Documentation**: See `docs/features/2-combat-system/2.6-combat-engine/architecture.md`
    /// </remarks>
    public interface IBattleStatisticsCollector
    {
        /// <summary>
        /// Gets the collected statistics.
        /// </summary>
        BattleStatistics GetStatistics();

        /// <summary>
        /// Resets all statistics.
        /// </summary>
        void Reset();
    }
}
