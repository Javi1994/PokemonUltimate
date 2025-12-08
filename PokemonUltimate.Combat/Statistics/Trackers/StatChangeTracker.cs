using System.Collections.Generic;
using PokemonUltimate.Combat.Actions;
using PokemonUltimate.Combat.Foundation.Field;
using PokemonUltimate.Combat.Statistics.Definition;

namespace PokemonUltimate.Combat.Statistics.Trackers
{
    /// <summary>
    /// Tracks stat stage modifications.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.20: Statistics System
    /// **Documentation**: See `docs/features/2-combat-system/2.20-statistics-system/architecture.md`
    /// </remarks>
    internal class StatChangeTracker : IStatisticsTracker
    {
        public void TrackAction(BattleAction action, BattleField field, IEnumerable<BattleAction> reactions, BattleStatistics stats)
        {
            if (action == null || stats == null || field == null)
                return;

            // Track stat changes from StatChangeAction
            if (action is StatChangeAction statAction && statAction.Target?.Pokemon != null)
            {
                // Use Species.Name for statistics accumulation across multiple battles
                var pokemonKey = statAction.Target.Pokemon.Species.Name;
                var statName = statAction.Stat.ToString();
                var stageChange = statAction.Change;

                if (stageChange != 0)
                {
                    if (!stats.StatChangeStats.ContainsKey(pokemonKey))
                        stats.StatChangeStats[pokemonKey] = new Dictionary<string, List<int>>();

                    if (!stats.StatChangeStats[pokemonKey].ContainsKey(statName))
                        stats.StatChangeStats[pokemonKey][statName] = new List<int>();

                    stats.StatChangeStats[pokemonKey][statName].Add(stageChange);
                }
            }

            // Also check reactions for stat changes
            if (reactions != null)
            {
                foreach (var reaction in reactions)
                {
                    if (reaction is StatChangeAction reactionStat && reactionStat.Target?.Pokemon != null)
                    {
                        // Use Species.Name for statistics accumulation across multiple battles
                        var pokemonKey = reactionStat.Target.Pokemon.Species.Name;
                        var statName = reactionStat.Stat.ToString();
                        var stageChange = reactionStat.Change;

                        if (stageChange != 0)
                        {
                            if (!stats.StatChangeStats.ContainsKey(pokemonKey))
                                stats.StatChangeStats[pokemonKey] = new Dictionary<string, List<int>>();

                            if (!stats.StatChangeStats[pokemonKey].ContainsKey(statName))
                                stats.StatChangeStats[pokemonKey][statName] = new List<int>();

                            stats.StatChangeStats[pokemonKey][statName].Add(stageChange);
                        }
                    }
                }
            }
        }
    }
}

