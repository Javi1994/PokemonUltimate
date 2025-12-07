using System.Collections.Generic;
using System.Linq;
using PokemonUltimate.Combat.Actions;
using PokemonUltimate.Combat.Statistics.Trackers;

namespace PokemonUltimate.Combat.Statistics
{
    /// <summary>
    /// Collects battle statistics by observing action execution.
    /// Uses modular tracker pattern for easy extensibility.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.20: Statistics System
    /// **Documentation**: See `docs/features/2-combat-system/2.20-statistics-system/architecture.md`
    /// </remarks>
    public class BattleStatisticsCollector : IBattleActionObserver, IBattleStatisticsCollector
    {
        private readonly BattleStatistics _statistics = new BattleStatistics();
        private readonly List<IStatisticsTracker> _trackers = new List<IStatisticsTracker>();

        /// <summary>
        /// Creates a new battle statistics collector with default trackers.
        /// </summary>
        public BattleStatisticsCollector()
        {
            RegisterDefaultTrackers();
        }

        /// <summary>
        /// Registers a custom tracker for additional statistics.
        /// </summary>
        /// <param name="tracker">The tracker to register.</param>
        public void RegisterTracker(IStatisticsTracker tracker)
        {
            if (tracker != null)
            {
                _trackers.Add(tracker);
            }
        }

        /// <summary>
        /// Called when an action's logic is executed.
        /// </summary>
        public void OnActionExecuted(BattleAction action, BattleField field, IEnumerable<BattleAction> reactions)
        {
            if (action == null || field == null)
                return;

            // Process through all registered trackers
            foreach (var tracker in _trackers)
            {
                tracker.TrackAction(action, field, reactions ?? Enumerable.Empty<BattleAction>(), _statistics);
            }
        }

        /// <summary>
        /// Called when a battle turn starts.
        /// </summary>
        public void OnTurnStart(int turnNumber, BattleField field)
        {
            if (field == null)
                return;

            _statistics.TotalTurns++;
        }

        /// <summary>
        /// Called when a battle turn ends.
        /// </summary>
        public void OnTurnEnd(int turnNumber, BattleField field)
        {
            // Currently no tracking needed for turn end
            // Can be extended in the future
        }

        /// <summary>
        /// Called when a battle starts.
        /// By default, this resets statistics. For accumulating statistics across multiple battles,
        /// call Reset() manually before starting the batch and don't call OnBattleStart for each battle.
        /// </summary>
        public void OnBattleStart(BattleField field)
        {
            // Reset statistics for new battle
            // Note: If you want to accumulate statistics across multiple battles,
            // call Reset() manually before the batch and skip calling OnBattleStart for each battle
            Reset();
        }

        /// <summary>
        /// Called when a battle ends.
        /// </summary>
        public void OnBattleEnd(BattleOutcome outcome, BattleField field)
        {
            if (field == null)
                return;

            switch (outcome)
            {
                case BattleOutcome.Victory:
                    _statistics.PlayerWins++;
                    break;
                case BattleOutcome.Defeat:
                    _statistics.EnemyWins++;
                    break;
                case BattleOutcome.Draw:
                    _statistics.Draws++;
                    break;
            }
        }

        /// <summary>
        /// Gets the collected statistics.
        /// </summary>
        public BattleStatistics GetStatistics()
        {
            return _statistics;
        }

        /// <summary>
        /// Resets all statistics.
        /// </summary>
        public void Reset()
        {
            _statistics.PlayerWins = 0;
            _statistics.EnemyWins = 0;
            _statistics.Draws = 0;
            _statistics.MoveUsageStats.Clear();
            _statistics.StatusEffectStats.Clear();
            _statistics.VolatileStatusStats.Clear();
            _statistics.DamageStats.Clear();
            _statistics.CriticalHits = 0;
            _statistics.Misses = 0;
            _statistics.WeatherChanges.Clear();
            _statistics.TerrainChanges.Clear();
            _statistics.SideConditionStats.Clear();
            _statistics.HazardStats.Clear();
            _statistics.StatChangeStats.Clear();
            _statistics.HealingStats.Clear();
            _statistics.ActionTypeStats.Clear();
            _statistics.EffectGenerationStats.Clear();
            _statistics.AbilityActivationStats.Clear();
            _statistics.ItemActivationStats.Clear();
            _statistics.TotalTurns = 0;
            _statistics.TurnDurations.Clear();
            _statistics.FaintedPokemon.Clear();
            _statistics.SwitchCount.Clear();
            _statistics.AIDecisions.Clear();
            _statistics.TeamStatusHistory.Clear();
        }

        /// <summary>
        /// Registers default trackers for common statistics.
        /// </summary>
        private void RegisterDefaultTrackers()
        {
            _trackers.Add(new ActionTypeTracker());
            _trackers.Add(new MoveUsageTracker());
            _trackers.Add(new DamageTracker());
            _trackers.Add(new StatusEffectTracker());
            _trackers.Add(new FieldEffectTracker());
            _trackers.Add(new HealingTracker());
            _trackers.Add(new StatChangeTracker());
            _trackers.Add(new TeamBattleTracker());
        }
    }
}

