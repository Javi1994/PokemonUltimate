using System.Collections.Generic;
using PokemonUltimate.Combat.Actions;

namespace PokemonUltimate.Combat.Statistics.Trackers
{
    /// <summary>
    /// Tracks persistent status effect applications.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.20: Statistics System
    /// **Documentation**: See `docs/features/2-combat-system/2.20-statistics-system/architecture.md`
    /// </remarks>
    internal class StatusEffectTracker : IStatisticsTracker
    {
        public void TrackAction(BattleAction action, BattleField field, IEnumerable<BattleAction> reactions, BattleStatistics stats)
        {
            if (action == null || stats == null || field == null)
                return;

            // Track status from ApplyStatusAction
            if (action is ApplyStatusAction statusAction && statusAction.Target?.Pokemon != null)
            {
                var pokemonName = statusAction.Target.Pokemon.Species.Name;
                var statusName = statusAction.Status.ToString();

                if (!string.IsNullOrEmpty(statusName) && statusName != "None")
                {
                    if (!stats.StatusEffectStats.ContainsKey(pokemonName))
                        stats.StatusEffectStats[pokemonName] = new Dictionary<string, int>();

                    if (!stats.StatusEffectStats[pokemonName].ContainsKey(statusName))
                        stats.StatusEffectStats[pokemonName][statusName] = 0;

                    stats.StatusEffectStats[pokemonName][statusName]++;
                }
            }

            // Also check reactions for status applications
            if (reactions != null)
            {
                foreach (var reaction in reactions)
                {
                    if (reaction is ApplyStatusAction reactionStatus && reactionStatus.Target?.Pokemon != null)
                    {
                        var pokemonName = reactionStatus.Target.Pokemon.Species.Name;
                        var statusName = reactionStatus.Status.ToString();

                        if (!string.IsNullOrEmpty(statusName) && statusName != "None")
                        {
                            if (!stats.StatusEffectStats.ContainsKey(pokemonName))
                                stats.StatusEffectStats[pokemonName] = new Dictionary<string, int>();

                            if (!stats.StatusEffectStats[pokemonName].ContainsKey(statusName))
                                stats.StatusEffectStats[pokemonName][statusName] = 0;

                            stats.StatusEffectStats[pokemonName][statusName]++;
                        }
                    }
                }
            }
        }
    }
}

