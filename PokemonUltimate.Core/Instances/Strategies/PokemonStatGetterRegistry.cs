using System.Collections.Generic;
using PokemonUltimate.Core.Enums;

namespace PokemonUltimate.Core.Instances.Strategies
{
    /// <summary>
    /// Registry that maps stat types to their getter strategies for PokemonInstance.
    /// </summary>
    public static class PokemonStatGetterRegistry
    {
        private static readonly Dictionary<Stat, IPokemonStatGetterStrategy> _strategies;

        static PokemonStatGetterRegistry()
        {
            _strategies = new Dictionary<Stat, IPokemonStatGetterStrategy>
            {
                { Stat.HP, new MaxHPStatGetterStrategy() },
                { Stat.Attack, new AttackStatGetterStrategy() },
                { Stat.Defense, new DefenseStatGetterStrategy() },
                { Stat.SpAttack, new SpAttackStatGetterStrategy() },
                { Stat.SpDefense, new SpDefenseStatGetterStrategy() },
                { Stat.Speed, new SpeedStatGetterStrategy() },
                { Stat.Accuracy, new AccuracyStatGetterStrategy() },
                { Stat.Evasion, new EvasionStatGetterStrategy() }
            };
        }

        /// <summary>
        /// Gets the stat value for the specified stat type from PokemonInstance.
        /// Returns 0 for unknown stats.
        /// </summary>
        public static int GetStat(PokemonInstance pokemon, Stat stat)
        {
            if (_strategies.TryGetValue(stat, out var strategy))
            {
                return strategy.GetStat(pokemon);
            }

            return 0;
        }
    }
}
