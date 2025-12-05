using System.Collections.Generic;
using PokemonUltimate.Core.Enums;

namespace PokemonUltimate.Core.Blueprints.Strategies
{
    /// <summary>
    /// Registry that maps stat types to their getter strategies.
    /// </summary>
    public static class StatGetterRegistry
    {
        private static readonly Dictionary<Stat, IStatGetterStrategy> _strategies;

        static StatGetterRegistry()
        {
            _strategies = new Dictionary<Stat, IStatGetterStrategy>
            {
                { Stat.HP, new HPStatGetterStrategy() },
                { Stat.Attack, new AttackStatGetterStrategy() },
                { Stat.Defense, new DefenseStatGetterStrategy() },
                { Stat.SpAttack, new SpAttackStatGetterStrategy() },
                { Stat.SpDefense, new SpDefenseStatGetterStrategy() },
                { Stat.Speed, new SpeedStatGetterStrategy() }
            };
        }

        /// <summary>
        /// Gets the stat value for the specified stat type from BaseStats.
        /// Returns 0 for Accuracy/Evasion or unknown stats.
        /// </summary>
        public static int GetStat(BaseStats baseStats, Stat stat)
        {
            if (_strategies.TryGetValue(stat, out var strategy))
            {
                return strategy.GetStat(baseStats);
            }

            return 0; // Accuracy/Evasion or unknown stats
        }
    }
}
