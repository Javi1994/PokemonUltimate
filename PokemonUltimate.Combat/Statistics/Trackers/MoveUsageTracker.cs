using System.Collections.Generic;
using PokemonUltimate.Combat.Actions;
using PokemonUltimate.Combat.Field;
using PokemonUltimate.Combat.Statistics.Definition;

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
                // Use Species.Name for statistics accumulation across multiple battles
                // InstanceId is only used for display purposes in single-battle scenarios
                var pokemonKey = moveAction.User.Pokemon.Species.Name;
                var moveName = moveAction.Move?.Name ?? "Unknown";

                if (!stats.MoveUsageStats.ContainsKey(pokemonKey))
                    stats.MoveUsageStats[pokemonKey] = new Dictionary<string, int>();

                if (!stats.MoveUsageStats[pokemonKey].ContainsKey(moveName))
                    stats.MoveUsageStats[pokemonKey][moveName] = 0;

                stats.MoveUsageStats[pokemonKey][moveName]++;
            }
        }
    }
}

