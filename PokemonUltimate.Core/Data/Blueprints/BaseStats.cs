using System.Collections.Generic;
using System.Linq;
using PokemonUltimate.Core.Data.Enums;


namespace PokemonUltimate.Core.Data.Blueprints
{
    /// <summary>
    /// Base stats for a Pokemon species. These are the foundation for calculating
    /// actual stats based on level, IVs, EVs, and nature.
    /// Values typically range from 1-255 in official games.
    /// </summary>
    /// <remarks>
    /// **Feature**: 1: Game Data
    /// **Sub-Feature**: 1.1: Pokemon Data
    /// **Documentation**: See `docs/features/1-game-data/1.1-pokemon-data/README.md`
    /// </remarks>
    public class BaseStats
    {
        public const int MinStatValue = 1;
        public const int MaxStatValue = 255;

        public int HP { get; set; }
        public int Attack { get; set; }
        public int Defense { get; set; }
        public int SpAttack { get; set; }
        public int SpDefense { get; set; }
        public int Speed { get; set; }

        #region Computed Properties

        /// <summary>
        /// Base Stat Total - sum of all stats, used for comparing Pokemon power.
        /// </summary>
        public int Total => HP + Attack + Defense + SpAttack + SpDefense + Speed;

        /// <summary>
        /// Returns true if all stats are within valid range (1-255).
        /// </summary>
        public bool IsValid =>
            HP >= MinStatValue && HP <= MaxStatValue &&
            Attack >= MinStatValue && Attack <= MaxStatValue &&
            Defense >= MinStatValue && Defense <= MaxStatValue &&
            SpAttack >= MinStatValue && SpAttack <= MaxStatValue &&
            SpDefense >= MinStatValue && SpDefense <= MaxStatValue &&
            Speed >= MinStatValue && Speed <= MaxStatValue;

        #endregion

        #region Aliases for Clarity

        /// <summary>Alias for Attack.</summary>
        public int PhysicalAttack => Attack;

        /// <summary>Alias for Defense.</summary>
        public int PhysicalDefense => Defense;

        /// <summary>Alias for SpAttack.</summary>
        public int SpecialAttack => SpAttack;

        /// <summary>Alias for SpDefense.</summary>
        public int SpecialDefense => SpDefense;

        #endregion

        #region Stat Analysis

        /// <summary>
        /// Sum of HP, Defense, and SpDefense.
        /// </summary>
        public int DefensiveTotal => HP + Defense + SpDefense;

        /// <summary>
        /// Sum of Attack and SpAttack.
        /// </summary>
        public int OffensiveTotal => Attack + SpAttack;

        /// <summary>
        /// True if Attack is higher than SpAttack.
        /// </summary>
        public bool IsPhysicalAttacker => Attack > SpAttack;

        /// <summary>
        /// True if SpAttack is higher than Attack.
        /// </summary>
        public bool IsSpecialAttacker => SpAttack > Attack;

        /// <summary>
        /// True if Attack equals SpAttack.
        /// </summary>
        public bool IsMixedAttacker => Attack == SpAttack;

        /// <summary>
        /// Returns the stat type with the highest value (excluding HP).
        /// In case of tie, returns the first in order (Attack, Defense, SpAttack, SpDefense, Speed).
        /// </summary>
        public Stat HighestStat
        {
            get
            {
                var stats = GetStatValues();
                return stats.OrderByDescending(s => s.Value).First().Key;
            }
        }

        /// <summary>
        /// Returns the highest stat value.
        /// </summary>
        public int HighestStatValue
        {
            get
            {
                var stats = GetStatValues();
                return stats.Values.Max();
            }
        }

        /// <summary>
        /// Returns the stat type with the lowest value (excluding HP for battle stats).
        /// In case of tie, returns the first in order.
        /// </summary>
        public Stat LowestStat
        {
            get
            {
                var stats = GetStatValues();
                return stats.OrderBy(s => s.Value).First().Key;
            }
        }

        /// <summary>
        /// Returns the lowest stat value.
        /// </summary>
        public int LowestStatValue
        {
            get
            {
                var stats = GetStatValues();
                return stats.Values.Min();
            }
        }

        #endregion

        #region Constructors

        public BaseStats() { }

        public BaseStats(int hp, int attack, int defense, int spAttack, int spDefense, int speed)
        {
            HP = hp;
            Attack = attack;
            Defense = defense;
            SpAttack = spAttack;
            SpDefense = spDefense;
            Speed = speed;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Returns a dictionary of all stats (including HP) for analysis.
        /// </summary>
        private Dictionary<Stat, int> GetStatValues()
        {
            return new Dictionary<Stat, int>
            {
                { Stat.HP, HP },
                { Stat.Attack, Attack },
                { Stat.Defense, Defense },
                { Stat.SpAttack, SpAttack },
                { Stat.SpDefense, SpDefense },
                { Stat.Speed, Speed }
            };
        }

        #endregion
    }
}
