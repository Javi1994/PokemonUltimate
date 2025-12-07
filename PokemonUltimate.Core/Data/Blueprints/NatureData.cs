using System.Collections.Generic;
using PokemonUltimate.Core.Data.Enums;

namespace PokemonUltimate.Core.Data.Blueprints
{
    /// <summary>
    /// Provides stat modifier information for Pokemon natures.
    /// Each nature gives +10% to one stat and -10% to another.
    /// Neutral natures have no effect.
    /// </summary>
    /// <remarks>
    /// **Feature**: 1: Game Data
    /// **Sub-Feature**: 1.14: Enums & Constants
    /// **Documentation**: See `docs/features/1-game-data/1.14-enums-constants/README.md`
    /// </remarks>
    public static class NatureData
    {
        /// <summary>
        /// Stat modifier multiplier for boosted stat.
        /// </summary>
        public const float BoostMultiplier = 1.1f;

        /// <summary>
        /// Stat modifier multiplier for reduced stat.
        /// </summary>
        public const float ReduceMultiplier = 0.9f;

        /// <summary>
        /// No modification multiplier.
        /// </summary>
        public const float NeutralMultiplier = 1.0f;

        private static readonly Dictionary<Nature, (Stat? increased, Stat? decreased)> _modifiers;

        static NatureData()
        {
            _modifiers = new Dictionary<Nature, (Stat?, Stat?)>
            {
                // Neutral natures
                { Nature.Hardy, (null, null) },
                { Nature.Docile, (null, null) },
                { Nature.Serious, (null, null) },
                { Nature.Bashful, (null, null) },
                { Nature.Quirky, (null, null) },

                // Attack boosting
                { Nature.Lonely, (Stat.Attack, Stat.Defense) },
                { Nature.Brave, (Stat.Attack, Stat.Speed) },
                { Nature.Adamant, (Stat.Attack, Stat.SpAttack) },
                { Nature.Naughty, (Stat.Attack, Stat.SpDefense) },

                // Defense boosting
                { Nature.Bold, (Stat.Defense, Stat.Attack) },
                { Nature.Relaxed, (Stat.Defense, Stat.Speed) },
                { Nature.Impish, (Stat.Defense, Stat.SpAttack) },
                { Nature.Lax, (Stat.Defense, Stat.SpDefense) },

                // Speed boosting
                { Nature.Timid, (Stat.Speed, Stat.Attack) },
                { Nature.Hasty, (Stat.Speed, Stat.Defense) },
                { Nature.Jolly, (Stat.Speed, Stat.SpAttack) },
                { Nature.Naive, (Stat.Speed, Stat.SpDefense) },

                // Special Attack boosting
                { Nature.Modest, (Stat.SpAttack, Stat.Attack) },
                { Nature.Mild, (Stat.SpAttack, Stat.Defense) },
                { Nature.Quiet, (Stat.SpAttack, Stat.Speed) },
                { Nature.Rash, (Stat.SpAttack, Stat.SpDefense) },

                // Special Defense boosting
                { Nature.Calm, (Stat.SpDefense, Stat.Attack) },
                { Nature.Gentle, (Stat.SpDefense, Stat.Defense) },
                { Nature.Sassy, (Stat.SpDefense, Stat.Speed) },
                { Nature.Careful, (Stat.SpDefense, Stat.SpAttack) }
            };
        }

        /// <summary>
        /// Gets the stat that is increased by this nature (or null for neutral natures).
        /// </summary>
        public static Stat? GetIncreasedStat(Nature nature)
        {
            return _modifiers[nature].increased;
        }

        /// <summary>
        /// Gets the stat that is decreased by this nature (or null for neutral natures).
        /// </summary>
        public static Stat? GetDecreasedStat(Nature nature)
        {
            return _modifiers[nature].decreased;
        }

        /// <summary>
        /// Returns true if this nature is neutral (no stat changes).
        /// </summary>
        public static bool IsNeutral(Nature nature)
        {
            return _modifiers[nature].increased == null;
        }

        /// <summary>
        /// Gets the stat multiplier for a specific stat and nature.
        /// Returns 1.1 for boosted stat, 0.9 for reduced stat, 1.0 otherwise.
        /// </summary>
        public static float GetStatMultiplier(Nature nature, Stat stat)
        {
            var (increased, decreased) = _modifiers[nature];

            if (increased == stat)
                return BoostMultiplier;
            if (decreased == stat)
                return ReduceMultiplier;

            return NeutralMultiplier;
        }
    }
}

