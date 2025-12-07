using System.Collections.Generic;
using System.Linq;
using PokemonUltimate.Combat.Actions;
using PokemonUltimate.Combat.Events;

namespace PokemonUltimate.Combat.Statistics.Trackers
{
    /// <summary>
    /// Tracks team battle statistics including Pokemon faints, switches, and team status.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.20: Statistics System
    /// **Documentation**: See `docs/features/2-combat-system/2.20-statistics-system/architecture.md`
    /// </remarks>
    internal class TeamBattleTracker : IStatisticsTracker
    {
        public void TrackAction(BattleAction action, BattleField field, IEnumerable<BattleAction> reactions, BattleStatistics stats)
        {
            if (action == null || stats == null || field == null)
                return;

            // Track Pokemon faints
            if (action is FaintAction faintAction && faintAction.Target?.Pokemon != null && faintAction.Target.Side != null)
            {
                var side = faintAction.Target.Side.IsPlayer;
                // Use Species.Name for statistics accumulation across multiple battles
                var pokemonKey = faintAction.Target.Pokemon.Species.Name;

                if (!stats.FaintedPokemon.ContainsKey(side))
                    stats.FaintedPokemon[side] = new List<string>();

                if (!stats.FaintedPokemon[side].Contains(pokemonKey))
                    stats.FaintedPokemon[side].Add(pokemonKey);

                // Track kill history (who killed whom)
                // Only track kills where the killer is different from the victim (exclude self-inflicted deaths from status effects)
                if (faintAction.User?.Pokemon != null && faintAction.User.Side != null && faintAction.User != faintAction.Target)
                {
                    var killerName = faintAction.User.Pokemon.DisplayName;
                    var victimName = faintAction.Target.Pokemon.DisplayName;
                    var killerIsPlayer = faintAction.User.Side.IsPlayer;
                    stats.KillHistory.Add((killerName, victimName, killerIsPlayer));
                }

                // Update team status snapshot
                UpdateTeamStatusSnapshot(field, stats, side);
            }

            // Track Pokemon switches
            if (action is SwitchAction switchAction && switchAction.Slot?.Side != null)
            {
                var side = switchAction.Slot.Side.IsPlayer;

                if (!stats.SwitchCount.ContainsKey(side))
                    stats.SwitchCount[side] = 0;

                stats.SwitchCount[side]++;
            }
        }

        /// <summary>
        /// Updates team status snapshot for statistics.
        /// </summary>
        private void UpdateTeamStatusSnapshot(BattleField field, BattleStatistics stats, bool isPlayerSide)
        {
            var side = isPlayerSide ? field.PlayerSide : field.EnemySide;
            if (side?.Party == null)
                return;

            var totalPokemon = side.Party.Count;
            var faintedCount = side.Party.Count(p => p.IsFainted);
            var remainingCount = totalPokemon - faintedCount;

            // Store snapshot (could be enhanced to track per-turn)
            // For now, we'll track the latest state
            if (!stats.TeamStatusHistory.ContainsKey(0))
                stats.TeamStatusHistory[0] = new Dictionary<bool, TeamStatusSnapshot>();

            if (!stats.TeamStatusHistory[0].ContainsKey(isPlayerSide))
                stats.TeamStatusHistory[0][isPlayerSide] = new TeamStatusSnapshot();

            stats.TeamStatusHistory[0][isPlayerSide] = new TeamStatusSnapshot
            {
                RemainingPokemon = remainingCount,
                TotalPokemon = totalPokemon,
                FaintedCount = faintedCount
            };
        }
    }
}
