using System;
using System.Collections.Generic;
using PokemonUltimate.Core.Blueprints;
using PokemonUltimate.Core.Constants;
using PokemonUltimate.Core.Enums;
using PokemonUltimate.Core.Factories;

namespace PokemonUltimate.UnifiedDebuggerUI.Runners
{
    /// <summary>
    /// Calculates Pokemon stats with different configurations for the Stat Calculator Debugger.
    /// </summary>
    /// <remarks>
    /// **Feature**: 6: Development Tools
    /// **Sub-Feature**: 6.1: Stat Calculator Debugger
    /// **Documentation**: See `docs/features/6-development-tools/6.1-stat-calculator-debugger/README.md`
    /// </remarks>
    public class StatCalculatorRunner
    {
        /// <summary>
        /// Configuration for stat calculation.
        /// </summary>
        public class StatCalculationConfig
        {
            public PokemonSpeciesData PokemonSpecies { get; set; }
            public int Level { get; set; } = 50;
            public Nature Nature { get; set; } = Nature.Hardy;
            public int IV_HP { get; set; } = CoreConstants.DefaultIV;
            public int IV_Attack { get; set; } = CoreConstants.DefaultIV;
            public int IV_Defense { get; set; } = CoreConstants.DefaultIV;
            public int IV_SpAttack { get; set; } = CoreConstants.DefaultIV;
            public int IV_SpDefense { get; set; } = CoreConstants.DefaultIV;
            public int IV_Speed { get; set; } = CoreConstants.DefaultIV;
            public int EV_HP { get; set; } = CoreConstants.DefaultEV;
            public int EV_Attack { get; set; } = CoreConstants.DefaultEV;
            public int EV_Defense { get; set; } = CoreConstants.DefaultEV;
            public int EV_SpAttack { get; set; } = CoreConstants.DefaultEV;
            public int EV_SpDefense { get; set; } = CoreConstants.DefaultEV;
            public int EV_Speed { get; set; } = CoreConstants.DefaultEV;

            public int TotalEVs => EV_HP + EV_Attack + EV_Defense + EV_SpAttack + EV_SpDefense + EV_Speed;
        }

        /// <summary>
        /// Detailed stat breakdown showing how each component contributes.
        /// </summary>
        public class StatBreakdown
        {
            public Stat Stat { get; set; }
            public int BaseStat { get; set; }
            public int IV { get; set; }
            public int EV { get; set; }
            public int EVBonus { get; set; } // floor(EV/4)
            public int Level { get; set; }
            public float NatureMultiplier { get; set; }
            public int RawStat { get; set; } // Before nature multiplier
            public int FinalStat { get; set; } // After nature multiplier
        }

        /// <summary>
        /// Complete stat calculation result.
        /// </summary>
        public class StatCalculationResult
        {
            public StatBreakdown HP { get; set; }
            public StatBreakdown Attack { get; set; }
            public StatBreakdown Defense { get; set; }
            public StatBreakdown SpAttack { get; set; }
            public StatBreakdown SpDefense { get; set; }
            public StatBreakdown Speed { get; set; }
            public int TotalEVs { get; set; }
            public bool IsValidEVTotal => TotalEVs <= CoreConstants.MaxTotalEV;
        }

        private readonly IStatCalculator _statCalculator;

        public StatCalculatorRunner()
        {
            _statCalculator = StatCalculator.Default;
        }

        /// <summary>
        /// Calculates stats for the given configuration.
        /// </summary>
        public StatCalculationResult CalculateStats(StatCalculationConfig config)
        {
            if (config == null)
                throw new ArgumentNullException(nameof(config));
            if (config.PokemonSpecies == null)
                throw new ArgumentException("PokemonSpecies cannot be null", nameof(config));

            var result = new StatCalculationResult
            {
                TotalEVs = config.TotalEVs
            };

            var baseStats = config.PokemonSpecies.BaseStats;

            // Calculate HP (different formula)
            result.HP = CalculateHPBreakdown(baseStats.HP, config);

            // Calculate other stats
            result.Attack = CalculateStatBreakdown(baseStats.Attack, config, Stat.Attack);
            result.Defense = CalculateStatBreakdown(baseStats.Defense, config, Stat.Defense);
            result.SpAttack = CalculateStatBreakdown(baseStats.SpAttack, config, Stat.SpAttack);
            result.SpDefense = CalculateStatBreakdown(baseStats.SpDefense, config, Stat.SpDefense);
            result.Speed = CalculateStatBreakdown(baseStats.Speed, config, Stat.Speed);

            return result;
        }

        /// <summary>
        /// Compares two configurations side-by-side.
        /// </summary>
        public (StatCalculationResult First, StatCalculationResult Second) CompareConfigurations(
            StatCalculationConfig config1, StatCalculationConfig config2)
        {
            var result1 = CalculateStats(config1);
            var result2 = CalculateStats(config2);
            return (result1, result2);
        }

        private StatBreakdown CalculateHPBreakdown(int baseHP, StatCalculationConfig config)
        {
            int iv = config.IV_HP;
            int ev = config.EV_HP;
            int evBonus = ev / CoreConstants.EVBonusDivisor;

            // HP formula: floor((2 * Base + IV + floor(EV/4)) * Level / 100) + Level + 10
            int rawStat = ((CoreConstants.StatFormulaBase * baseHP + iv + evBonus) * config.Level / CoreConstants.StatFormulaDivisor) + config.Level + CoreConstants.HPFormulaBonus;

            return new StatBreakdown
            {
                Stat = Stat.HP,
                BaseStat = baseHP,
                IV = iv,
                EV = ev,
                EVBonus = evBonus,
                Level = config.Level,
                NatureMultiplier = 1.0f, // HP is not affected by Nature
                RawStat = rawStat,
                FinalStat = rawStat
            };
        }

        private StatBreakdown CalculateStatBreakdown(int baseStat, StatCalculationConfig config, Stat stat)
        {
            int iv = GetIVForStat(config, stat);
            int ev = GetEVForStat(config, stat);
            int evBonus = ev / CoreConstants.EVBonusDivisor;

            // Stat formula: floor((floor((2 * Base + IV + floor(EV/4)) * Level / 100) + 5) * Nature)
            int rawStat = ((CoreConstants.StatFormulaBase * baseStat + iv + evBonus) * config.Level / CoreConstants.StatFormulaDivisor) + CoreConstants.StatFormulaBonus;
            float natureMultiplier = NatureData.GetStatMultiplier(config.Nature, stat);
            
            // Verify calculation matches StatCalculator
            int verifiedStat = _statCalculator.CalculateStat(baseStat, config.Level, config.Nature, stat, iv, ev);
            int finalStat = (int)(rawStat * natureMultiplier);
            
            // Ensure our calculation matches StatCalculator (for debugging)
            if (finalStat != verifiedStat)
            {
                // If mismatch, use StatCalculator's result (it's the source of truth)
                finalStat = verifiedStat;
            }

            return new StatBreakdown
            {
                Stat = stat,
                BaseStat = baseStat,
                IV = iv,
                EV = ev,
                EVBonus = evBonus,
                Level = config.Level,
                NatureMultiplier = natureMultiplier,
                RawStat = rawStat,
                FinalStat = finalStat
            };
        }

        private int GetIVForStat(StatCalculationConfig config, Stat stat)
        {
            return stat switch
            {
                Stat.Attack => config.IV_Attack,
                Stat.Defense => config.IV_Defense,
                Stat.SpAttack => config.IV_SpAttack,
                Stat.SpDefense => config.IV_SpDefense,
                Stat.Speed => config.IV_Speed,
                _ => 0
            };
        }

        private int GetEVForStat(StatCalculationConfig config, Stat stat)
        {
            return stat switch
            {
                Stat.Attack => config.EV_Attack,
                Stat.Defense => config.EV_Defense,
                Stat.SpAttack => config.EV_SpAttack,
                Stat.SpDefense => config.EV_SpDefense,
                Stat.Speed => config.EV_Speed,
                _ => 0
            };
        }
    }
}

