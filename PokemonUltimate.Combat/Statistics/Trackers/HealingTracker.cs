using System.Collections.Generic;
using PokemonUltimate.Combat.Actions;

namespace PokemonUltimate.Combat.Statistics.Trackers
{
    /// <summary>
    /// Tracks healing actions and amounts.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.20: Statistics System
    /// **Documentation**: See `docs/features/2-combat-system/2.20-statistics-system/architecture.md`
    /// </remarks>
    internal class HealingTracker : IStatisticsTracker
    {
        public void TrackAction(BattleAction action, BattleField field, IEnumerable<BattleAction> reactions, BattleStatistics stats)
        {
            if (action == null || stats == null || field == null)
                return;

            // Track healing from HealAction
            if (action is HealAction healAction && healAction.Target?.Pokemon != null)
            {
                // Use Species.Name for statistics accumulation across multiple battles
                var pokemonKey = healAction.Target.Pokemon.Species.Name;
                var healAmount = healAction.Amount;

                if (healAmount > 0)
                {
                    if (!stats.HealingStats.ContainsKey(pokemonKey))
                        stats.HealingStats[pokemonKey] = new List<int>();

                    stats.HealingStats[pokemonKey].Add(healAmount);
                }
            }

            // Also check reactions for healing
            if (reactions != null)
            {
                foreach (var reaction in reactions)
                {
                    if (reaction is HealAction reactionHeal && reactionHeal.Target?.Pokemon != null)
                    {
                        // Use Species.Name for statistics accumulation across multiple battles
                        var pokemonKey = reactionHeal.Target.Pokemon.Species.Name;
                        var healAmount = reactionHeal.Amount;

                        if (healAmount > 0)
                        {
                            if (!stats.HealingStats.ContainsKey(pokemonKey))
                                stats.HealingStats[pokemonKey] = new List<int>();

                            stats.HealingStats[pokemonKey].Add(healAmount);
                        }
                    }
                }
            }
        }
    }
}

