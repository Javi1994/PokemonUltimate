using System;
using PokemonUltimate.Core.Data.Blueprints;

namespace PokemonUltimate.Combat.Damage
{
    /// <summary>
    /// Contains all data needed for damage calculation.
    /// Inputs are immutable, calculation state is mutable as it passes through the pipeline.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.4: Damage Calculation Pipeline
    /// **Documentation**: See `docs/features/2-combat-system/2.4-damage-calculation-pipeline/architecture.md`
    /// </remarks>
    public class DamageContext
    {
        #region Inputs (Immutable)

        /// <summary>
        /// The slot containing the attacking Pokemon.
        /// </summary>
        public BattleSlot Attacker { get; }

        /// <summary>
        /// The slot containing the defending Pokemon.
        /// </summary>
        public BattleSlot Defender { get; }

        /// <summary>
        /// The move being used.
        /// </summary>
        public MoveData Move { get; }

        /// <summary>
        /// The battlefield state.
        /// </summary>
        public BattleField Field { get; }

        /// <summary>
        /// If true, force a critical hit.
        /// </summary>
        public bool ForceCritical { get; }

        /// <summary>
        /// If set, use this random value instead of generating one (0.0 to 1.0).
        /// </summary>
        public float? FixedRandomValue { get; }

        #endregion

        #region Calculation State (Mutable)

        /// <summary>
        /// The base damage before multipliers.
        /// Set by BaseDamageStep.
        /// </summary>
        public float BaseDamage { get; set; }

        /// <summary>
        /// The cumulative multiplier applied to base damage.
        /// </summary>
        public float Multiplier { get; set; } = 1.0f;

        /// <summary>
        /// Whether this attack is a critical hit.
        /// Set by CriticalHitStep.
        /// </summary>
        public bool IsCritical { get; set; }

        /// <summary>
        /// The type effectiveness multiplier (0, 0.25, 0.5, 1, 2, 4).
        /// Set by TypeEffectivenessStep.
        /// </summary>
        public float TypeEffectiveness { get; set; } = 1.0f;

        /// <summary>
        /// Whether this attack benefits from STAB (Same Type Attack Bonus).
        /// Set by StabStep.
        /// </summary>
        public bool IsStab { get; set; }

        /// <summary>
        /// The random factor applied (0.85 to 1.0).
        /// Set by RandomFactorStep.
        /// </summary>
        public float RandomFactor { get; set; } = 1.0f;

        #endregion

        #region Results

        /// <summary>
        /// The final calculated damage.
        /// Minimum 1 unless immune (TypeEffectiveness = 0) or status move (BaseDamage = 0).
        /// </summary>
        public int FinalDamage
        {
            get
            {
                // Immunity = 0 damage
                if (TypeEffectiveness == 0f)
                    return 0;

                // Status moves (power 0) = 0 damage
                if (BaseDamage == 0f)
                    return 0;

                // Minimum 1 damage for damaging moves
                return Math.Max(1, (int)(BaseDamage * Multiplier));
            }
        }

        #endregion

        /// <summary>
        /// Creates a new damage context.
        /// </summary>
        /// <param name="attacker">The attacking slot.</param>
        /// <param name="defender">The defending slot.</param>
        /// <param name="move">The move being used.</param>
        /// <param name="field">The battlefield.</param>
        /// <param name="forceCritical">Force a critical hit.</param>
        /// <param name="fixedRandomValue">Use a fixed random value (for testing).</param>
        public DamageContext(
            BattleSlot attacker,
            BattleSlot defender,
            MoveData move,
            BattleField field,
            bool forceCritical = false,
            float? fixedRandomValue = null)
        {
            Attacker = attacker ?? throw new ArgumentNullException(nameof(attacker));
            Defender = defender ?? throw new ArgumentNullException(nameof(defender));
            Move = move ?? throw new ArgumentNullException(nameof(move));
            Field = field ?? throw new ArgumentNullException(nameof(field));
            ForceCritical = forceCritical;
            FixedRandomValue = fixedRandomValue;
        }
    }
}

