using System;
using System.Collections.Generic;
using PokemonUltimate.Core.Constants;
using PokemonUltimate.Core.Enums;

namespace PokemonUltimate.Core.Factories
{
    /// <summary>
    /// Calculates type effectiveness for damage calculations.
    /// Implements the complete Gen6+ type chart.
    /// </summary>
    /// <remarks>
    /// **Feature**: 1: Game Data
    /// **Sub-Feature**: 1.8: Type Effectiveness Table
    /// **Documentation**: See `docs/features/1-game-data/1.8-type-effectiveness-table/README.md`
    /// </remarks>
    public class TypeEffectiveness : ITypeEffectiveness
    {
        #region Constants

        /// <summary>
        /// Super effective damage multiplier (2x).
        /// </summary>
        public const float SuperEffective = 2.0f;

        /// <summary>
        /// Normal effectiveness (1x).
        /// </summary>
        public const float Normal = 1.0f;

        /// <summary>
        /// Not very effective damage multiplier (0.5x).
        /// </summary>
        public const float NotVeryEffective = 0.5f;

        /// <summary>
        /// No effect / immune (0x).
        /// </summary>
        public const float Immune = 0.0f;

        /// <summary>
        /// STAB (Same Type Attack Bonus) multiplier.
        /// </summary>
        public const float STAB = 1.5f;

        #endregion

        #region Type Chart

        private static readonly ITypeEffectiveness _defaultInstance = new TypeEffectiveness();

        // Type chart: attackingType -> defendingType -> effectiveness
        private readonly Dictionary<PokemonType, Dictionary<PokemonType, float>> _typeChart;

        /// <summary>
        /// Gets the default instance for static method compatibility.
        /// </summary>
        public static ITypeEffectiveness Default => _defaultInstance;

        /// <summary>
        /// Creates a new TypeEffectiveness instance.
        /// </summary>
        public TypeEffectiveness()
        {
            _typeChart = InitializeTypeChart();
        }

        private Dictionary<PokemonType, Dictionary<PokemonType, float>> InitializeTypeChart()
        {
            var chart = new Dictionary<PokemonType, Dictionary<PokemonType, float>>();

            // Initialize all types with normal effectiveness
            foreach (PokemonType attacker in Enum.GetValues(typeof(PokemonType)))
            {
                chart[attacker] = new Dictionary<PokemonType, float>();
                foreach (PokemonType defender in Enum.GetValues(typeof(PokemonType)))
                {
                    chart[attacker][defender] = Normal;
                }
            }

            // Normal
            SetEffectiveness(chart, PokemonType.Normal, PokemonType.Rock, NotVeryEffective);
            SetEffectiveness(chart, PokemonType.Normal, PokemonType.Steel, NotVeryEffective);
            SetEffectiveness(chart, PokemonType.Normal, PokemonType.Ghost, Immune);

            // Fire
            SetEffectiveness(chart, PokemonType.Fire, PokemonType.Grass, SuperEffective);
            SetEffectiveness(chart, PokemonType.Fire, PokemonType.Ice, SuperEffective);
            SetEffectiveness(chart, PokemonType.Fire, PokemonType.Bug, SuperEffective);
            SetEffectiveness(chart, PokemonType.Fire, PokemonType.Steel, SuperEffective);
            SetEffectiveness(chart, PokemonType.Fire, PokemonType.Fire, NotVeryEffective);
            SetEffectiveness(chart, PokemonType.Fire, PokemonType.Water, NotVeryEffective);
            SetEffectiveness(chart, PokemonType.Fire, PokemonType.Rock, NotVeryEffective);
            SetEffectiveness(chart, PokemonType.Fire, PokemonType.Dragon, NotVeryEffective);

            // Water
            SetEffectiveness(chart, PokemonType.Water, PokemonType.Fire, SuperEffective);
            SetEffectiveness(chart, PokemonType.Water, PokemonType.Ground, SuperEffective);
            SetEffectiveness(chart, PokemonType.Water, PokemonType.Rock, SuperEffective);
            SetEffectiveness(chart, PokemonType.Water, PokemonType.Water, NotVeryEffective);
            SetEffectiveness(chart, PokemonType.Water, PokemonType.Grass, NotVeryEffective);
            SetEffectiveness(chart, PokemonType.Water, PokemonType.Dragon, NotVeryEffective);

            // Grass
            SetEffectiveness(chart, PokemonType.Grass, PokemonType.Water, SuperEffective);
            SetEffectiveness(chart, PokemonType.Grass, PokemonType.Ground, SuperEffective);
            SetEffectiveness(chart, PokemonType.Grass, PokemonType.Rock, SuperEffective);
            SetEffectiveness(chart, PokemonType.Grass, PokemonType.Fire, NotVeryEffective);
            SetEffectiveness(chart, PokemonType.Grass, PokemonType.Grass, NotVeryEffective);
            SetEffectiveness(chart, PokemonType.Grass, PokemonType.Poison, NotVeryEffective);
            SetEffectiveness(chart, PokemonType.Grass, PokemonType.Flying, NotVeryEffective);
            SetEffectiveness(chart, PokemonType.Grass, PokemonType.Bug, NotVeryEffective);
            SetEffectiveness(chart, PokemonType.Grass, PokemonType.Dragon, NotVeryEffective);
            SetEffectiveness(chart, PokemonType.Grass, PokemonType.Steel, NotVeryEffective);

            // Electric
            SetEffectiveness(chart, PokemonType.Electric, PokemonType.Water, SuperEffective);
            SetEffectiveness(chart, PokemonType.Electric, PokemonType.Flying, SuperEffective);
            SetEffectiveness(chart, PokemonType.Electric, PokemonType.Electric, NotVeryEffective);
            SetEffectiveness(chart, PokemonType.Electric, PokemonType.Grass, NotVeryEffective);
            SetEffectiveness(chart, PokemonType.Electric, PokemonType.Dragon, NotVeryEffective);
            SetEffectiveness(chart, PokemonType.Electric, PokemonType.Ground, Immune);

            // Ice
            SetEffectiveness(chart, PokemonType.Ice, PokemonType.Grass, SuperEffective);
            SetEffectiveness(chart, PokemonType.Ice, PokemonType.Ground, SuperEffective);
            SetEffectiveness(chart, PokemonType.Ice, PokemonType.Flying, SuperEffective);
            SetEffectiveness(chart, PokemonType.Ice, PokemonType.Dragon, SuperEffective);
            SetEffectiveness(chart, PokemonType.Ice, PokemonType.Fire, NotVeryEffective);
            SetEffectiveness(chart, PokemonType.Ice, PokemonType.Water, NotVeryEffective);
            SetEffectiveness(chart, PokemonType.Ice, PokemonType.Ice, NotVeryEffective);
            SetEffectiveness(chart, PokemonType.Ice, PokemonType.Steel, NotVeryEffective);

            // Fighting
            SetEffectiveness(chart, PokemonType.Fighting, PokemonType.Normal, SuperEffective);
            SetEffectiveness(chart, PokemonType.Fighting, PokemonType.Ice, SuperEffective);
            SetEffectiveness(chart, PokemonType.Fighting, PokemonType.Rock, SuperEffective);
            SetEffectiveness(chart, PokemonType.Fighting, PokemonType.Dark, SuperEffective);
            SetEffectiveness(chart, PokemonType.Fighting, PokemonType.Steel, SuperEffective);
            SetEffectiveness(chart, PokemonType.Fighting, PokemonType.Poison, NotVeryEffective);
            SetEffectiveness(chart, PokemonType.Fighting, PokemonType.Flying, NotVeryEffective);
            SetEffectiveness(chart, PokemonType.Fighting, PokemonType.Psychic, NotVeryEffective);
            SetEffectiveness(chart, PokemonType.Fighting, PokemonType.Bug, NotVeryEffective);
            SetEffectiveness(chart, PokemonType.Fighting, PokemonType.Fairy, NotVeryEffective);
            SetEffectiveness(chart, PokemonType.Fighting, PokemonType.Ghost, Immune);

            // Poison
            SetEffectiveness(chart, PokemonType.Poison, PokemonType.Grass, SuperEffective);
            SetEffectiveness(chart, PokemonType.Poison, PokemonType.Fairy, SuperEffective);
            SetEffectiveness(chart, PokemonType.Poison, PokemonType.Poison, NotVeryEffective);
            SetEffectiveness(chart, PokemonType.Poison, PokemonType.Ground, NotVeryEffective);
            SetEffectiveness(chart, PokemonType.Poison, PokemonType.Rock, NotVeryEffective);
            SetEffectiveness(chart, PokemonType.Poison, PokemonType.Ghost, NotVeryEffective);
            SetEffectiveness(chart, PokemonType.Poison, PokemonType.Steel, Immune);

            // Ground
            SetEffectiveness(chart, PokemonType.Ground, PokemonType.Fire, SuperEffective);
            SetEffectiveness(chart, PokemonType.Ground, PokemonType.Electric, SuperEffective);
            SetEffectiveness(chart, PokemonType.Ground, PokemonType.Poison, SuperEffective);
            SetEffectiveness(chart, PokemonType.Ground, PokemonType.Rock, SuperEffective);
            SetEffectiveness(chart, PokemonType.Ground, PokemonType.Steel, SuperEffective);
            SetEffectiveness(chart, PokemonType.Ground, PokemonType.Grass, NotVeryEffective);
            SetEffectiveness(chart, PokemonType.Ground, PokemonType.Bug, NotVeryEffective);
            SetEffectiveness(chart, PokemonType.Ground, PokemonType.Flying, Immune);

            // Flying
            SetEffectiveness(chart, PokemonType.Flying, PokemonType.Grass, SuperEffective);
            SetEffectiveness(chart, PokemonType.Flying, PokemonType.Fighting, SuperEffective);
            SetEffectiveness(chart, PokemonType.Flying, PokemonType.Bug, SuperEffective);
            SetEffectiveness(chart, PokemonType.Flying, PokemonType.Electric, NotVeryEffective);
            SetEffectiveness(chart, PokemonType.Flying, PokemonType.Rock, NotVeryEffective);
            SetEffectiveness(chart, PokemonType.Flying, PokemonType.Steel, NotVeryEffective);

            // Psychic
            SetEffectiveness(chart, PokemonType.Psychic, PokemonType.Fighting, SuperEffective);
            SetEffectiveness(chart, PokemonType.Psychic, PokemonType.Poison, SuperEffective);
            SetEffectiveness(chart, PokemonType.Psychic, PokemonType.Psychic, NotVeryEffective);
            SetEffectiveness(chart, PokemonType.Psychic, PokemonType.Steel, NotVeryEffective);
            SetEffectiveness(chart, PokemonType.Psychic, PokemonType.Dark, Immune);

            // Bug
            SetEffectiveness(chart, PokemonType.Bug, PokemonType.Grass, SuperEffective);
            SetEffectiveness(chart, PokemonType.Bug, PokemonType.Psychic, SuperEffective);
            SetEffectiveness(chart, PokemonType.Bug, PokemonType.Dark, SuperEffective);
            SetEffectiveness(chart, PokemonType.Bug, PokemonType.Fire, NotVeryEffective);
            SetEffectiveness(chart, PokemonType.Bug, PokemonType.Fighting, NotVeryEffective);
            SetEffectiveness(chart, PokemonType.Bug, PokemonType.Poison, NotVeryEffective);
            SetEffectiveness(chart, PokemonType.Bug, PokemonType.Flying, NotVeryEffective);
            SetEffectiveness(chart, PokemonType.Bug, PokemonType.Ghost, NotVeryEffective);
            SetEffectiveness(chart, PokemonType.Bug, PokemonType.Steel, NotVeryEffective);
            SetEffectiveness(chart, PokemonType.Bug, PokemonType.Fairy, NotVeryEffective);

            // Rock
            SetEffectiveness(chart, PokemonType.Rock, PokemonType.Fire, SuperEffective);
            SetEffectiveness(chart, PokemonType.Rock, PokemonType.Ice, SuperEffective);
            SetEffectiveness(chart, PokemonType.Rock, PokemonType.Flying, SuperEffective);
            SetEffectiveness(chart, PokemonType.Rock, PokemonType.Bug, SuperEffective);
            SetEffectiveness(chart, PokemonType.Rock, PokemonType.Fighting, NotVeryEffective);
            SetEffectiveness(chart, PokemonType.Rock, PokemonType.Ground, NotVeryEffective);
            SetEffectiveness(chart, PokemonType.Rock, PokemonType.Steel, NotVeryEffective);

            // Ghost
            SetEffectiveness(chart, PokemonType.Ghost, PokemonType.Psychic, SuperEffective);
            SetEffectiveness(chart, PokemonType.Ghost, PokemonType.Ghost, SuperEffective);
            SetEffectiveness(chart, PokemonType.Ghost, PokemonType.Dark, NotVeryEffective);
            SetEffectiveness(chart, PokemonType.Ghost, PokemonType.Normal, Immune);

            // Dragon
            SetEffectiveness(chart, PokemonType.Dragon, PokemonType.Dragon, SuperEffective);
            SetEffectiveness(chart, PokemonType.Dragon, PokemonType.Steel, NotVeryEffective);
            SetEffectiveness(chart, PokemonType.Dragon, PokemonType.Fairy, Immune);

            // Dark
            SetEffectiveness(chart, PokemonType.Dark, PokemonType.Psychic, SuperEffective);
            SetEffectiveness(chart, PokemonType.Dark, PokemonType.Ghost, SuperEffective);
            SetEffectiveness(chart, PokemonType.Dark, PokemonType.Fighting, NotVeryEffective);
            SetEffectiveness(chart, PokemonType.Dark, PokemonType.Dark, NotVeryEffective);
            SetEffectiveness(chart, PokemonType.Dark, PokemonType.Fairy, NotVeryEffective);

            // Steel
            SetEffectiveness(chart, PokemonType.Steel, PokemonType.Ice, SuperEffective);
            SetEffectiveness(chart, PokemonType.Steel, PokemonType.Rock, SuperEffective);
            SetEffectiveness(chart, PokemonType.Steel, PokemonType.Fairy, SuperEffective);
            SetEffectiveness(chart, PokemonType.Steel, PokemonType.Fire, NotVeryEffective);
            SetEffectiveness(chart, PokemonType.Steel, PokemonType.Water, NotVeryEffective);
            SetEffectiveness(chart, PokemonType.Steel, PokemonType.Electric, NotVeryEffective);
            SetEffectiveness(chart, PokemonType.Steel, PokemonType.Steel, NotVeryEffective);

            // Fairy
            SetEffectiveness(chart, PokemonType.Fairy, PokemonType.Fighting, SuperEffective);
            SetEffectiveness(chart, PokemonType.Fairy, PokemonType.Dragon, SuperEffective);
            SetEffectiveness(chart, PokemonType.Fairy, PokemonType.Dark, SuperEffective);
            SetEffectiveness(chart, PokemonType.Fairy, PokemonType.Fire, NotVeryEffective);
            SetEffectiveness(chart, PokemonType.Fairy, PokemonType.Poison, NotVeryEffective);
            SetEffectiveness(chart, PokemonType.Fairy, PokemonType.Steel, NotVeryEffective);

            return chart;
        }

        private static void SetEffectiveness(
            Dictionary<PokemonType, Dictionary<PokemonType, float>> chart,
            PokemonType attacker, PokemonType defender, float effectiveness)
        {
            chart[attacker][defender] = effectiveness;
        }

        #endregion

        #region Public API

        /// <summary>
        /// Gets the type effectiveness multiplier for a single defender type.
        /// </summary>
        float ITypeEffectiveness.GetEffectiveness(PokemonType attackType, PokemonType defenderType)
        {
            if (_typeChart.TryGetValue(attackType, out var defenderChart))
            {
                if (defenderChart.TryGetValue(defenderType, out var effectiveness))
                {
                    return effectiveness;
                }
            }
            return Normal;
        }

        /// <summary>
        /// Gets the combined type effectiveness for a dual-type defender.
        /// </summary>
        float ITypeEffectiveness.GetEffectiveness(PokemonType attackType, PokemonType primaryType, PokemonType? secondaryType)
        {
            float primaryMult = ((ITypeEffectiveness)this).GetEffectiveness(attackType, primaryType);

            if (secondaryType.HasValue)
            {
                float secondaryMult = ((ITypeEffectiveness)this).GetEffectiveness(attackType, secondaryType.Value);
                return primaryMult * secondaryMult;
            }

            return primaryMult;
        }

        /// <summary>
        /// Checks if the attack is super effective (>1x).
        /// </summary>
        bool ITypeEffectiveness.IsSuperEffective(float effectiveness)
        {
            return effectiveness > Normal;
        }

        /// <summary>
        /// Checks if the attack is not very effective (<1x but >0x).
        /// </summary>
        bool ITypeEffectiveness.IsNotVeryEffective(float effectiveness)
        {
            return effectiveness > Immune && effectiveness < Normal;
        }

        /// <summary>
        /// Checks if the defender is immune (0x).
        /// </summary>
        bool ITypeEffectiveness.IsImmune(float effectiveness)
        {
            return effectiveness == Immune;
        }

        /// <summary>
        /// Gets a human-readable description of the effectiveness.
        /// </summary>
        string ITypeEffectiveness.GetEffectivenessDescription(float effectiveness)
        {
            if (effectiveness == Immune)
                return GameMessages.NoEffect;
            if (effectiveness >= SuperEffective * SuperEffective)
                return GameMessages.SuperEffective4x;
            if (effectiveness >= SuperEffective)
                return GameMessages.SuperEffective;
            if (effectiveness <= NotVeryEffective * NotVeryEffective)
                return GameMessages.NotVeryEffective025x;
            if (effectiveness <= NotVeryEffective)
                return GameMessages.NotVeryEffective;
            return GameMessages.NormalEffectiveness;
        }

        /// <summary>
        /// Calculates STAB bonus if the move type matches the attacker's type.
        /// </summary>
        float ITypeEffectiveness.GetSTABMultiplier(PokemonType moveType, PokemonType primaryType, PokemonType? secondaryType)
        {
            if (moveType == primaryType || (secondaryType.HasValue && moveType == secondaryType.Value))
            {
                return STAB;
            }
            return Normal;
        }

        /// <summary>
        /// Gets all types that the given type is super effective against.
        /// </summary>
        List<PokemonType> ITypeEffectiveness.GetSuperEffectiveAgainst(PokemonType attackType)
        {
            var result = new List<PokemonType>();
            if (_typeChart.TryGetValue(attackType, out var defenderChart))
            {
                foreach (var kvp in defenderChart)
                {
                    if (kvp.Value >= SuperEffective)
                        result.Add(kvp.Key);
                }
            }
            return result;
        }

        /// <summary>
        /// Gets all types that the given type is resisted by.
        /// </summary>
        List<PokemonType> ITypeEffectiveness.GetResistedBy(PokemonType attackType)
        {
            var result = new List<PokemonType>();
            if (_typeChart.TryGetValue(attackType, out var defenderChart))
            {
                foreach (var kvp in defenderChart)
                {
                    if (kvp.Value > Immune && kvp.Value < Normal)
                        result.Add(kvp.Key);
                }
            }
            return result;
        }

        /// <summary>
        /// Gets all types that are immune to the given attack type.
        /// </summary>
        List<PokemonType> ITypeEffectiveness.GetImmuneTypes(PokemonType attackType)
        {
            var result = new List<PokemonType>();
            if (_typeChart.TryGetValue(attackType, out var defenderChart))
            {
                foreach (var kvp in defenderChart)
                {
                    if (kvp.Value == Immune)
                        result.Add(kvp.Key);
                }
            }
            return result;
        }

        #endregion

        #region Static Compatibility Methods

        /// <summary>
        /// Gets the type effectiveness multiplier for a single defender type (static wrapper for compatibility).
        /// </summary>
        public static float GetEffectiveness(PokemonType attackType, PokemonType defenderType)
        {
            return ((ITypeEffectiveness)_defaultInstance).GetEffectiveness(attackType, defenderType);
        }

        /// <summary>
        /// Gets the combined type effectiveness for a dual-type defender (static wrapper for compatibility).
        /// </summary>
        public static float GetEffectiveness(PokemonType attackType, PokemonType primaryType, PokemonType? secondaryType)
        {
            return ((ITypeEffectiveness)_defaultInstance).GetEffectiveness(attackType, primaryType, secondaryType);
        }

        /// <summary>
        /// Checks if the attack is super effective (static wrapper for compatibility).
        /// </summary>
        public static bool IsSuperEffective(float effectiveness)
        {
            return ((ITypeEffectiveness)_defaultInstance).IsSuperEffective(effectiveness);
        }

        /// <summary>
        /// Checks if the attack is not very effective (static wrapper for compatibility).
        /// </summary>
        public static bool IsNotVeryEffective(float effectiveness)
        {
            return ((ITypeEffectiveness)_defaultInstance).IsNotVeryEffective(effectiveness);
        }

        /// <summary>
        /// Checks if the defender is immune (static wrapper for compatibility).
        /// </summary>
        public static bool IsImmune(float effectiveness)
        {
            return ((ITypeEffectiveness)_defaultInstance).IsImmune(effectiveness);
        }

        /// <summary>
        /// Gets a human-readable description of the effectiveness (static wrapper for compatibility).
        /// </summary>
        public static string GetEffectivenessDescription(float effectiveness)
        {
            return ((ITypeEffectiveness)_defaultInstance).GetEffectivenessDescription(effectiveness);
        }

        /// <summary>
        /// Calculates STAB bonus if the move type matches the attacker's type (static wrapper for compatibility).
        /// </summary>
        public static float GetSTABMultiplier(PokemonType moveType, PokemonType primaryType, PokemonType? secondaryType)
        {
            return ((ITypeEffectiveness)_defaultInstance).GetSTABMultiplier(moveType, primaryType, secondaryType);
        }

        /// <summary>
        /// Gets all types that the given type is super effective against (static wrapper for compatibility).
        /// </summary>
        public static List<PokemonType> GetSuperEffectiveAgainst(PokemonType attackType)
        {
            return ((ITypeEffectiveness)_defaultInstance).GetSuperEffectiveAgainst(attackType);
        }

        /// <summary>
        /// Gets all types that the given type is resisted by (static wrapper for compatibility).
        /// </summary>
        public static List<PokemonType> GetResistedBy(PokemonType attackType)
        {
            return ((ITypeEffectiveness)_defaultInstance).GetResistedBy(attackType);
        }

        /// <summary>
        /// Gets all types that are immune to the given attack type (static wrapper for compatibility).
        /// </summary>
        public static List<PokemonType> GetImmuneTypes(PokemonType attackType)
        {
            return ((ITypeEffectiveness)_defaultInstance).GetImmuneTypes(attackType);
        }

        #endregion
    }
}

