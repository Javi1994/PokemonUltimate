using System;
using PokemonUltimate.Core.Blueprints;
using PokemonUltimate.Core.Constants;
using PokemonUltimate.Core.Enums;
using PokemonUltimate.Core.Instances;

namespace PokemonUltimate.Core.Factories
{
    /// <summary>
    /// Calculates Pokemon stats using the official Gen3+ formula.
    /// By default uses maximum IVs (31) and EVs (252) for a roguelike experience.
    /// </summary>
    /// <remarks>
    /// **Feature**: 1: Game Data
    /// **Sub-Feature**: 1.12: Factories & Calculators
    /// **Documentation**: See `docs/features/1-game-data/1.12-factories-calculators/README.md`
    /// </remarks>
    public class StatCalculator : IStatCalculator
    {
        private static readonly IStatCalculator _defaultInstance = new StatCalculator();

        /// <summary>
        /// Gets the default instance for static method compatibility.
        /// </summary>
        public static IStatCalculator Default => _defaultInstance;

        #region Constants

        /// <summary>
        /// Maximum Individual Value (0-31).
        /// </summary>
        [Obsolete("Use CoreConstants.MaxIV instead")]
        public const int MaxIV = CoreConstants.MaxIV;

        /// <summary>
        /// Maximum Effort Value per stat (0-252).
        /// </summary>
        [Obsolete("Use CoreConstants.MaxEV instead")]
        public const int MaxEV = CoreConstants.MaxEV;

        /// <summary>
        /// Maximum total EVs across all stats (510).
        /// </summary>
        [Obsolete("Use CoreConstants.MaxTotalEV instead")]
        public const int MaxTotalEV = CoreConstants.MaxTotalEV;

        /// <summary>
        /// Default IV used in this game (always max for roguelike).
        /// </summary>
        [Obsolete("Use CoreConstants.DefaultIV instead")]
        public const int DefaultIV = CoreConstants.DefaultIV;

        /// <summary>
        /// Default EV used in this game (always max for roguelike).
        /// </summary>
        [Obsolete("Use CoreConstants.DefaultEV instead")]
        public const int DefaultEV = CoreConstants.DefaultEV;

        /// <summary>
        /// Minimum stat stage (-6).
        /// </summary>
        [Obsolete("Use CoreConstants.MinStatStage instead")]
        public const int MinStage = CoreConstants.MinStatStage;

        /// <summary>
        /// Maximum stat stage (+6).
        /// </summary>
        [Obsolete("Use CoreConstants.MaxStatStage instead")]
        public const int MaxStage = CoreConstants.MaxStatStage;

        #endregion

        #region HP Calculation

        /// <summary>
        /// Calculates HP stat using full Gen3+ formula with max IVs/EVs.
        /// Formula: floor((2 * Base + IV + floor(EV/4)) * Level / 100) + Level + 10
        /// </summary>
        int IStatCalculator.CalculateHP(int baseHP, int level)
        {
            return CalculateHPInternal(baseHP, level, CoreConstants.DefaultIV, CoreConstants.DefaultEV);
        }

        /// <summary>
        /// Calculates HP stat with custom IVs/EVs (for testing).
        /// </summary>
        int IStatCalculator.CalculateHP(int baseHP, int level, int iv, int ev)
        {
            return CalculateHPInternal(baseHP, level, iv, ev);
        }

        /// <summary>
        /// Calculates HP stat using IVSet and EVSet from PokemonInstance.
        /// </summary>
        int IStatCalculator.CalculateHP(int baseHP, int level, IVSet ivs, EVSet evs)
        {
            if (ivs == null)
                throw new ArgumentNullException(nameof(ivs));
            if (evs == null)
                throw new ArgumentNullException(nameof(evs));

            return CalculateHPInternal(baseHP, level, ivs.HP, evs.HP);
        }

        /// <summary>
        /// Internal implementation of HP calculation.
        /// </summary>
        private int CalculateHPInternal(int baseHP, int level, int iv, int ev)
        {
            ValidateBaseStat(baseHP, nameof(baseHP));
            CoreValidators.ValidateLevel(level);
            CoreValidators.ValidateIV(iv);
            CoreValidators.ValidateEV(ev);

            // HP formula: floor((StatFormulaBase * Base + IV + floor(EV/EVBonusDivisor)) * Level / StatFormulaDivisor) + Level + HPFormulaBonus
            int evBonus = ev / CoreConstants.EVBonusDivisor;
            return ((CoreConstants.StatFormulaBase * baseHP + iv + evBonus) * level / CoreConstants.StatFormulaDivisor) + level + CoreConstants.HPFormulaBonus;
        }

        #endregion

        #region Stat Calculation

        /// <summary>
        /// Calculates a non-HP stat using full Gen3+ formula with max IVs/EVs.
        /// Formula: floor((floor((StatFormulaBase * Base + IV + floor(EV/EVBonusDivisor)) * Level / StatFormulaDivisor) + StatFormulaBonus) * Nature)
        /// </summary>
        int IStatCalculator.CalculateStat(int baseStat, int level, Nature nature, Stat stat)
        {
            return CalculateStatInternal(baseStat, level, nature, stat, CoreConstants.DefaultIV, CoreConstants.DefaultEV);
        }

        /// <summary>
        /// Calculates a non-HP stat with custom IVs/EVs (for testing).
        /// </summary>
        int IStatCalculator.CalculateStat(int baseStat, int level, Nature nature, Stat stat, int iv, int ev)
        {
            return CalculateStatInternal(baseStat, level, nature, stat, iv, ev);
        }

        /// <summary>
        /// Calculates a non-HP stat using IVSet and EVSet from PokemonInstance.
        /// </summary>
        int IStatCalculator.CalculateStat(int baseStat, int level, Nature nature, Stat stat, IVSet ivs, EVSet evs)
        {
            if (ivs == null)
                throw new ArgumentNullException(nameof(ivs));
            if (evs == null)
                throw new ArgumentNullException(nameof(evs));

            int iv = ivs.GetIV(stat);
            int ev = evs.GetEV(stat);
            return CalculateStatInternal(baseStat, level, nature, stat, iv, ev);
        }

        /// <summary>
        /// Internal implementation of stat calculation.
        /// </summary>
        private int CalculateStatInternal(int baseStat, int level, Nature nature, Stat stat, int iv, int ev)
        {
            ValidateBaseStat(baseStat, nameof(baseStat));
            CoreValidators.ValidateLevel(level);
            CoreValidators.ValidateIV(iv);
            CoreValidators.ValidateEV(ev);

            // Stat formula: floor((floor((StatFormulaBase * Base + IV + floor(EV/EVBonusDivisor)) * Level / StatFormulaDivisor) + StatFormulaBonus) * Nature)
            int evBonus = ev / CoreConstants.EVBonusDivisor;
            int rawStat = ((CoreConstants.StatFormulaBase * baseStat + iv + evBonus) * level / CoreConstants.StatFormulaDivisor) + CoreConstants.StatFormulaBonus;
            float natureMultiplier = NatureData.GetStatMultiplier(nature, stat);

            return (int)(rawStat * natureMultiplier);
        }

        #endregion

        #region Battle Stage Multipliers

        /// <summary>
        /// Gets the stat stage multiplier for battle calculations.
        /// Stages range from -6 to +6.
        /// </summary>
        float IStatCalculator.GetStageMultiplier(int stage)
        {
            stage = Math.Max(CoreConstants.MinStatStage, Math.Min(CoreConstants.MaxStatStage, stage));

            if (stage >= 0)
                return (2f + stage) / 2f;

            return 2f / (2f + Math.Abs(stage));
        }

        /// <summary>
        /// Calculates the effective stat in battle, applying stat stages.
        /// </summary>
        int IStatCalculator.GetEffectiveStat(int calculatedStat, int stage)
        {
            if (calculatedStat < 0)
                throw new ArgumentException(ErrorMessages.StatCannotBeNegative, nameof(calculatedStat));

            float multiplier = ((IStatCalculator)this).GetStageMultiplier(stage);
            return (int)(calculatedStat * multiplier);
        }

        /// <summary>
        /// Gets the accuracy/evasion stage multiplier.
        /// Uses different formula: (3 + stage) / 3 or 3 / (3 + |stage|)
        /// </summary>
        float IStatCalculator.GetAccuracyStageMultiplier(int stage)
        {
            stage = Math.Max(CoreConstants.MinStatStage, Math.Min(CoreConstants.MaxStatStage, stage));

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
        int IStatCalculator.GetExpForLevel(int level)
        {
            CoreValidators.ValidateLevel(level);

            return level * level * level;
        }

        /// <summary>
        /// Experience required to go from current level to next level.
        /// </summary>
        int IStatCalculator.GetExpToNextLevel(int currentLevel)
        {
            if (currentLevel < 1 || currentLevel >= 100)
                return 0;

            return ((IStatCalculator)this).GetExpForLevel(currentLevel + 1) - ((IStatCalculator)this).GetExpForLevel(currentLevel);
        }

        /// <summary>
        /// Calculates what level a Pokemon should be at given total experience.
        /// </summary>
        int IStatCalculator.GetLevelForExp(int totalExp)
        {
            if (totalExp < 0)
                throw new ArgumentException(ErrorMessages.ExperienceCannotBeNegative, nameof(totalExp));

            for (int level = 100; level >= 1; level--)
            {
                if (totalExp >= ((IStatCalculator)this).GetExpForLevel(level))
                    return level;
            }

            return 1;
        }

        #endregion

        #region Static Compatibility Methods

        /// <summary>
        /// Calculates HP stat using full Gen3+ formula with max IVs/EVs (static wrapper for compatibility).
        /// </summary>
        public static int CalculateHP(int baseHP, int level)
        {
            return ((StatCalculator)_defaultInstance).CalculateHPInternal(baseHP, level, CoreConstants.DefaultIV, CoreConstants.DefaultEV);
        }

        /// <summary>
        /// Calculates HP stat with custom IVs/EVs (static wrapper for compatibility).
        /// </summary>
        public static int CalculateHP(int baseHP, int level, int iv, int ev)
        {
            return ((StatCalculator)_defaultInstance).CalculateHPInternal(baseHP, level, iv, ev);
        }

        /// <summary>
        /// Calculates a non-HP stat using full Gen3+ formula with max IVs/EVs (static wrapper for compatibility).
        /// </summary>
        public static int CalculateStat(int baseStat, int level, Nature nature, Stat stat)
        {
            return ((StatCalculator)_defaultInstance).CalculateStatInternal(baseStat, level, nature, stat, CoreConstants.DefaultIV, CoreConstants.DefaultEV);
        }

        /// <summary>
        /// Calculates a non-HP stat with custom IVs/EVs (static wrapper for compatibility).
        /// </summary>
        public static int CalculateStat(int baseStat, int level, Nature nature, Stat stat, int iv, int ev)
        {
            return ((StatCalculator)_defaultInstance).CalculateStatInternal(baseStat, level, nature, stat, iv, ev);
        }

        /// <summary>
        /// Gets the stat stage multiplier for battle calculations (static wrapper for compatibility).
        /// </summary>
        public static float GetStageMultiplier(int stage)
        {
            return ((IStatCalculator)_defaultInstance).GetStageMultiplier(stage);
        }

        /// <summary>
        /// Calculates the effective stat in battle, applying stat stages (static wrapper for compatibility).
        /// </summary>
        public static int GetEffectiveStat(int calculatedStat, int stage)
        {
            return ((IStatCalculator)_defaultInstance).GetEffectiveStat(calculatedStat, stage);
        }

        /// <summary>
        /// Gets the accuracy/evasion stage multiplier (static wrapper for compatibility).
        /// </summary>
        public static float GetAccuracyStageMultiplier(int stage)
        {
            return ((IStatCalculator)_defaultInstance).GetAccuracyStageMultiplier(stage);
        }

        /// <summary>
        /// Experience required to reach a level (static wrapper for compatibility).
        /// </summary>
        public static int GetExpForLevel(int level)
        {
            return ((IStatCalculator)_defaultInstance).GetExpForLevel(level);
        }

        /// <summary>
        /// Experience required to go from current level to next level (static wrapper for compatibility).
        /// </summary>
        public static int GetExpToNextLevel(int currentLevel)
        {
            return ((IStatCalculator)_defaultInstance).GetExpToNextLevel(currentLevel);
        }

        /// <summary>
        /// Calculates what level a Pokemon should be at given total experience (static wrapper for compatibility).
        /// </summary>
        public static int GetLevelForExp(int totalExp)
        {
            return ((IStatCalculator)_defaultInstance).GetLevelForExp(totalExp);
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
            CoreValidators.ValidateLevel(level);
        }

        private static void ValidateIV(int iv)
        {
            CoreValidators.ValidateIV(iv);
        }

        private static void ValidateEV(int ev)
        {
            CoreValidators.ValidateEV(ev);
        }

        #endregion
    }
}
