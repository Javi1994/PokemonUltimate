using System.Collections.Generic;
using PokemonUltimate.Combat.Actions;
using PokemonUltimate.Core.Localization;

namespace PokemonUltimate.Combat.Statistics.Trackers
{
    /// <summary>
    /// Tracks damage statistics including damage values, critical hits, and misses.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.20: Statistics System
    /// **Documentation**: See `docs/features/2-combat-system/2.20-statistics-system/architecture.md`
    /// </remarks>
    internal class DamageTracker : IStatisticsTracker
    {
        public void TrackAction(BattleAction action, BattleField field, IEnumerable<BattleAction> reactions, BattleStatistics stats)
        {
            if (action == null || stats == null || field == null)
                return;

            if (action is DamageAction damageAction && damageAction.Target?.Pokemon != null && damageAction.Context != null)
            {
                var pokemonName = damageAction.Target.Pokemon.Species.Name;
                var damage = damageAction.Context.FinalDamage;

                if (damage > 0)
                {
                    if (!stats.DamageStats.ContainsKey(pokemonName))
                        stats.DamageStats[pokemonName] = new List<int>();

                    stats.DamageStats[pokemonName].Add(damage);

                    // Track critical hits if applicable
                    if (damageAction.Context.IsCritical)
                    {
                        stats.CriticalHits++;
                    }
                }
            }

            // Track misses from UseMoveAction
            if (action is UseMoveAction moveAction && moveAction.User?.Pokemon != null)
            {
                // Check if move missed by looking at reactions or action state
                // This is a simplified check - actual implementation may need more context
                if (reactions != null)
                {
                    foreach (var reaction in reactions)
                    {
                        if (reaction is MessageAction msgAction)
                        {
                            // Check if message corresponds to "missed" localization key
                            // This is a simplified check - in a more robust implementation,
                            // we could track message keys instead of formatted messages
                            var provider = LocalizationManager.Instance;
                            var missedMessage = provider.GetString(LocalizationKey.BattleMissed);
                            if (msgAction.Message.Contains(missedMessage) ||
                                msgAction.Message.Contains("missed") ||
                                msgAction.Message.Contains("Miss"))
                            {
                                stats.Misses++;
                                break;
                            }
                        }
                    }
                }
            }
        }
    }
}

