using System.Collections.Generic;
using PokemonUltimate.Combat.Actions;

namespace PokemonUltimate.Combat.Statistics.Trackers
{
    /// <summary>
    /// Tracks move usage statistics per Pokemon.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.20: Statistics System
    /// **Documentation**: See `docs/features/2-combat-system/2.20-statistics-system/architecture.md`
    /// </remarks>
    internal class MoveUsageTracker : IStatisticsTracker
    {
        public void TrackAction(BattleAction action, BattleField field, IEnumerable<BattleAction> reactions, BattleStatistics stats)
        {
            if (action == null || stats == null || field == null)
                return;

            if (action is UseMoveAction moveAction && moveAction.User?.Pokemon != null)
            {
                var pokemonName = moveAction.User.Pokemon.Species.Name;
                var moveName = moveAction.Move?.Name ?? "Unknown";

                if (!stats.MoveUsageStats.ContainsKey(pokemonName))
                    stats.MoveUsageStats[pokemonName] = new Dictionary<string, int>();

                if (!stats.MoveUsageStats[pokemonName].ContainsKey(moveName))
                    stats.MoveUsageStats[pokemonName][moveName] = 0;

                stats.MoveUsageStats[pokemonName][moveName]++;
            }
        }
    }
}

