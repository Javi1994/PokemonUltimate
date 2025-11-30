using System;
using PokemonUltimate.Core.Blueprints;
using PokemonUltimate.Core.Constants;
using PokemonUltimate.Core.Enums;

namespace PokemonUltimate.Core.Factories
{
    /// <summary>
    /// Calculates Pokemon stats using the official Gen3+ formula.
    /// By default uses maximum IVs (31) and EVs (252) for a roguelike experience.
    /// </summary>
    public static class StatCalculator
    {
        #region Constants

        /// <summary>
        /// Maximum Individual Value (0-31).
        /// </summary>
        public const int MaxIV = 31;

        /// <summary>
        /// Maximum Effort Value per stat (0-252).
        /// </summary>
        public const int MaxEV = 252;

        /// <summary>
        /// Maximum total EVs across all stats (510).
        /// </summary>
        public const int MaxTotalEV = 510;

        /// <summary>
        /// Default IV used in this game (always max for roguelike).
        /// </summary>
        public const int DefaultIV = MaxIV;

        /// <summary>
        /// Default EV used in this game (always max for roguelike).
        /// </summary>
        public const int DefaultEV = MaxEV;

        /// <summary>
        /// Minimum stat stage (-6).
        /// </summary>
        public const int MinStage = -6;

        /// <summary>
        /// Maximum stat stage (+6).
        /// </summary>
        public const int MaxStage = 6;

        #endregion

        #region HP Calculation

        /// <summary>
        /// Calculates HP stat using full Gen3+ formula with max IVs/EVs.
        /// Formula: floor((2 * Base + IV + floor(EV/4)) * Level / 100) + Level + 10
        /// </summary>
        public static int CalculateHP(int baseHP, int level)
        {
            return CalculateHP(baseHP, level, DefaultIV, DefaultEV);
        }

        /// <summary>
        /// Calculates HP stat with custom IVs/EVs (for testing).
        /// </summary>
        public static int CalculateHP(int baseHP, int level, int iv, int ev)
        {
            ValidateBaseStat(baseHP, nameof(baseHP));
            ValidateLevel(level);
            ValidateIV(iv);
            ValidateEV(ev);

            // HP formula: floor((2 * Base + IV + floor(EV/4)) * Level / 100) + Level + 10
            int evBonus = ev / 4;
            return ((2 * baseHP + iv + evBonus) * level / 100) + level + 10;
        }

        #endregion

        #region Stat Calculation

        /// <summary>
        /// Calculates a non-HP stat using full Gen3+ formula with max IVs/EVs.
        /// Formula: floor((floor((2 * Base + IV + floor(EV/4)) * Level / 100) + 5) * Nature)
        /// </summary>
        public static int CalculateStat(int baseStat, int level, Nature nature, Stat stat)
        {
            return CalculateStat(baseStat, level, nature, stat, DefaultIV, DefaultEV);
        }

        /// <summary>
        /// Calculates a non-HP stat with custom IVs/EVs (for testing).
        /// </summary>
        public static int CalculateStat(int baseStat, int level, Nature nature, Stat stat, int iv, int ev)
        {
            ValidateBaseStat(baseStat, nameof(baseStat));
            ValidateLevel(level);
            ValidateIV(iv);
            ValidateEV(ev);

            // Stat formula: floor((floor((2 * Base + IV + floor(EV/4)) * Level / 100) + 5) * Nature)
            int evBonus = ev / 4;
            int rawStat = ((2 * baseStat + iv + evBonus) * level / 100) + 5;
            float natureMultiplier = NatureData.GetStatMultiplier(nature, stat);

            return (int)(rawStat * natureMultiplier);
        }

        #endregion

        #region Battle Stage Multipliers

        /// <summary>
        /// Gets the stat stage multiplier for battle calculations.
        /// Stages range from -6 to +6.
        /// </summary>
        public static float GetStageMultiplier(int stage)
        {
            stage = Math.Max(MinStage, Math.Min(MaxStage, stage));

            if (stage >= 0)
                return (2f + stage) / 2f;

            return 2f / (2f + Math.Abs(stage));
        }

        /// <summary>
        /// Calculates the effective stat in battle, applying stat stages.
        /// </summary>
        public static int GetEffectiveStat(int calculatedStat, int stage)
        {
            if (calculatedStat < 0)
                throw new ArgumentException(ErrorMessages.StatCannotBeNegative, nameof(calculatedStat));

            float multiplier = GetStageMultiplier(stage);
            return (int)(calculatedStat * multiplier);
        }

        /// <summary>
        /// Gets the accuracy/evasion stage multiplier.
        /// Uses different formula: (3 + stage) / 3 or 3 / (3 + |stage|)
        /// </summary>
        public static float GetAccuracyStageMultiplier(int stage)
        {
            stage = Math.Max(MinStage, Math.Min(MaxStage, stage));

            if (stage >= 0)
                return (3f + stage) / 3f;

            return 3f / (3f + Math.Abs(stage));
        }

        #endregion

        #region Experience Calculation

        /// <summary>
        /// Experience required to reach a level (Medium Fast growth rate).
        /// Formula: Level^3
        /// </summary>
        public static int GetExpForLevel(int level)
        {
            if (level < 1 || level > 100)
                throw new ArgumentException(ErrorMessages.LevelMustBeBetween1And100, nameof(level));

            return level * level * level;
        }

        /// <summary>
        /// Experience required to go from current level to next level.
        /// </summary>
        public static int GetExpToNextLevel(int currentLevel)
        {
            if (currentLevel < 1 || currentLevel >= 100)
                return 0;

            return GetExpForLevel(currentLevel + 1) - GetExpForLevel(currentLevel);
        }

        /// <summary>
        /// Calculates what level a Pokemon should be at given total experience.
        /// </summary>
        public static int GetLevelForExp(int totalExp)
        {
            if (totalExp < 0)
                throw new ArgumentException(ErrorMessages.ExperienceCannotBeNegative, nameof(totalExp));

            for (int level = 100; level >= 1; level--)
            {
                if (totalExp >= GetExpForLevel(level))
                    return level;
            }

            return 1;
        }

        #endregion

        #region Validation Helpers

        private static void ValidateBaseStat(int stat, string paramName)
        {
            if (stat < 0)
                throw new ArgumentException(ErrorMessages.BaseStatCannotBeNegative, paramName);
        }

        private static void ValidateLevel(int level)
        {
            if (level < 1 || level > 100)
                throw new ArgumentException(ErrorMessages.LevelMustBeBetween1And100, nameof(level));
        }

        private static void ValidateIV(int iv)
        {
            if (iv < 0 || iv > MaxIV)
                throw new ArgumentException(ErrorMessages.Format(ErrorMessages.IVMustBeBetween, MaxIV), nameof(iv));
        }

        private static void ValidateEV(int ev)
        {
            if (ev < 0 || ev > MaxEV)
                throw new ArgumentException(ErrorMessages.Format(ErrorMessages.EVMustBeBetween, MaxEV), nameof(ev));
        }

        #endregion
    }
}
