using System.Collections.Generic;
using PokemonUltimate.Combat.Actions;

namespace PokemonUltimate.Combat.Statistics.Trackers
{
    /// <summary>
    /// Tracks action type counts.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.20: Statistics System
    /// **Documentation**: See `docs/features/2-combat-system/2.20-statistics-system/architecture.md`
    /// </remarks>
    internal class ActionTypeTracker : IStatisticsTracker
    {
        public void TrackAction(BattleAction action, BattleField field, IEnumerable<BattleAction> reactions, BattleStatistics stats)
        {
            if (action == null || stats == null)
                return;

            var actionTypeName = action.GetType().Name;
            if (!stats.ActionTypeStats.ContainsKey(actionTypeName))
                stats.ActionTypeStats[actionTypeName] = 0;
            
            stats.ActionTypeStats[actionTypeName]++;
        }
    }
}

