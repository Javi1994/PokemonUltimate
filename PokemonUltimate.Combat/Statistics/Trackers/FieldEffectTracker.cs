using System.Collections.Generic;
using PokemonUltimate.Combat.Actions;
using PokemonUltimate.Core.Data.Enums;

namespace PokemonUltimate.Combat.Statistics.Trackers
{
    /// <summary>
    /// Tracks field effects (weather, terrain, hazards, side conditions).
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.20: Statistics System
    /// **Documentation**: See `docs/features/2-combat-system/2.20-statistics-system/architecture.md`
    /// </remarks>
    internal class FieldEffectTracker : IStatisticsTracker
    {
        public void TrackAction(BattleAction action, BattleField field, IEnumerable<BattleAction> reactions, BattleStatistics stats)
        {
            if (action == null || stats == null || field == null)
                return;

            // Track weather changes
            if (action is SetWeatherAction weatherAction)
            {
                var weatherName = weatherAction.Weather.ToString();
                if (!string.IsNullOrEmpty(weatherName) && weatherName != "None")
                {
                    if (!stats.WeatherChanges.ContainsKey(weatherName))
                        stats.WeatherChanges[weatherName] = 0;
                    stats.WeatherChanges[weatherName]++;
                }
            }

            // Track terrain changes
            if (action is SetTerrainAction terrainAction)
            {
                var terrainName = terrainAction.Terrain.ToString();
                if (!string.IsNullOrEmpty(terrainName) && terrainName != "None")
                {
                    if (!stats.TerrainChanges.ContainsKey(terrainName))
                        stats.TerrainChanges[terrainName] = 0;
                    stats.TerrainChanges[terrainName]++;
                }
            }

            // Track side conditions
            if (action is SetSideConditionAction sideConditionAction && sideConditionAction.TargetSide != null)
            {
                var side = sideConditionAction.TargetSide.IsPlayer ? "Player" : "Enemy";
                var conditionName = sideConditionAction.Condition.ToString();

                if (!string.IsNullOrEmpty(conditionName) && sideConditionAction.Condition != SideCondition.None)
                {
                    if (!stats.SideConditionStats.ContainsKey(side))
                        stats.SideConditionStats[side] = new Dictionary<string, int>();

                    if (!stats.SideConditionStats[side].ContainsKey(conditionName))
                        stats.SideConditionStats[side][conditionName] = 0;

                    stats.SideConditionStats[side][conditionName]++;
                }
            }
        }
    }
}

