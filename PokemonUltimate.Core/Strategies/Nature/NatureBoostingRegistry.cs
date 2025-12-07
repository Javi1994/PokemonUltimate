using System.Collections.Generic;
using PokemonUltimate.Core.Data.Enums;

namespace PokemonUltimate.Core.Strategies.Nature
{
    /// <summary>
    /// Registry that maps stat types to their corresponding nature boosting strategies.
    /// </summary>
    /// <remarks>
    /// **Feature**: 1: Game Data
    /// **Sub-Feature**: 1.12: Factories & Calculators
    /// **Documentation**: See `docs/features/1-game-data/1.12-factories-calculators/README.md`
    /// </remarks>
    public static class NatureBoostingRegistry
    {
        private static readonly Dictionary<Stat, INatureBoostingStrategy> _strategies;

        static NatureBoostingRegistry()
        {
            _strategies = new Dictionary<Stat, INatureBoostingStrategy>
            {
                { Stat.Attack, new AttackNatureBoostingStrategy() },
                { Stat.Defense, new DefenseNatureBoostingStrategy() },
                { Stat.SpAttack, new SpAttackNatureBoostingStrategy() },
                { Stat.SpDefense, new SpDefenseNatureBoostingStrategy() },
                { Stat.Speed, new SpeedNatureBoostingStrategy() }
            };
        }

        /// <summary>
        /// Gets the nature that boosts the specified stat.
        /// Returns Hardy (neutral) for HP or unknown stats.
        /// </summary>
        public static Data.Enums.Nature GetBoostingNature(Stat stat)
        {
            if (_strategies.TryGetValue(stat, out var strategy))
            {
                return strategy.GetBoostingNature();
            }

            // Default to neutral nature for HP or unknown stats
            return new DefaultNatureBoostingStrategy().GetBoostingNature();
        }
    }
}
