using PokemonUltimate.Core.Enums;
using PokemonUltimate.Core.Instances;

namespace PokemonUltimate.Core.Factories
{
    /// <summary>
    /// Calculates Pokemon stats using official formulas.
    /// </summary>
    /// <remarks>
    /// **Feature**: 1: Game Data
    /// **Sub-Feature**: 1.12: Factories & Calculators
    /// **Documentation**: See `docs/features/1-game-data/1.12-factories-calculators/README.md`
    /// </remarks>
    public interface IStatCalculator
    {
        /// <summary>
        /// Calculates HP stat using full Gen3+ formula with max IVs/EVs.
        /// Formula: floor((StatFormulaBase * Base + IV + floor(EV/EVBonusDivisor)) * Level / StatFormulaDivisor) + Level + HPFormulaBonus
        /// </summary>
        int CalculateHP(int baseHP, int level);

        /// <summary>
        /// Calculates HP stat with custom IVs/EVs (for testing).
        /// </summary>
        int CalculateHP(int baseHP, int level, int iv, int ev);

        /// <summary>
        /// Calculates HP stat using IVSet and EVSet from PokemonInstance.
        /// </summary>
        int CalculateHP(int baseHP, int level, IVSet ivs, EVSet evs);

        /// <summary>
        /// Calculates a non-HP stat using full Gen3+ formula with max IVs/EVs.
        /// Formula: floor((floor((StatFormulaBase * Base + IV + floor(EV/EVBonusDivisor)) * Level / StatFormulaDivisor) + StatFormulaBonus) * Nature)
        /// </summary>
        int CalculateStat(int baseStat, int level, Nature nature, Stat stat);

        /// <summary>
        /// Calculates a non-HP stat with custom IVs/EVs (for testing).
        /// </summary>
        int CalculateStat(int baseStat, int level, Nature nature, Stat stat, int iv, int ev);

        /// <summary>
        /// Calculates a non-HP stat using IVSet and EVSet from PokemonInstance.
        /// </summary>
        int CalculateStat(int baseStat, int level, Nature nature, Stat stat, IVSet ivs, EVSet evs);

        /// <summary>
        /// Gets the stat stage multiplier for battle calculations.
        /// Stages range from -6 to +6.
        /// </summary>
        float GetStageMultiplier(int stage);

        /// <summary>
        /// Calculates the effective stat in battle, applying stat stages.
        /// </summary>
        int GetEffectiveStat(int calculatedStat, int stage);

        /// <summary>
        /// Gets the accuracy/evasion stage multiplier.
        /// Uses different formula: (3 + stage) / 3 or 3 / (3 + |stage|)
        /// </summary>
        float GetAccuracyStageMultiplier(int stage);

        /// <summary>
        /// Experience required to reach a level (Medium Fast growth rate).
        /// Formula: Level^3
        /// </summary>
        int GetExpForLevel(int level);

        /// <summary>
        /// Experience required to go from current level to next level.
        /// </summary>
        int GetExpToNextLevel(int currentLevel);

        /// <summary>
        /// Calculates what level a Pokemon should be at given total experience.
        /// </summary>
        int GetLevelForExp(int totalExp);
    }
}
