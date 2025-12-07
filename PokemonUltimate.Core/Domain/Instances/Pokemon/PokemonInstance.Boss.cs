using System;
using System.Reflection;

namespace PokemonUltimate.Core.Domain.Instances.Pokemon
{
    /// <summary>
    /// Boss battle-related methods for PokemonInstance.
    /// Handles applying Boss multipliers to HP and stats.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.19: Battle Formats
    /// **Documentation**: See `docs/features/2-combat-system/PLAN_COMPLETAR_FEATURE_2.md`
    /// </remarks>
    public partial class PokemonInstance
    {
        /// <summary>
        /// Applies Boss battle multipliers to this Pokemon's HP and stats.
        /// This is used for Raid battles where Boss Pokemon have increased HP and stats.
        /// </summary>
        /// <param name="hpMultiplier">HP multiplier (e.g., 5.0 = 5x HP).</param>
        /// <param name="statMultiplier">Stat multiplier (e.g., 1.5 = 1.5x stats).</param>
        /// <exception cref="ArgumentException">If multipliers are less than 1.0.</exception>
        public void ApplyBossMultipliers(float hpMultiplier, float statMultiplier)
        {
            if (hpMultiplier < 1.0f)
                throw new ArgumentException("HP multiplier must be >= 1.0", nameof(hpMultiplier));
            if (statMultiplier < 1.0f)
                throw new ArgumentException("Stat multiplier must be >= 1.0", nameof(statMultiplier));

            // Store HP percentage to maintain proportion
            float hpPercent = MaxHP > 0 ? (float)CurrentHP / MaxHP : 1f;

            // Apply HP multiplier
            int newMaxHP = (int)(MaxHP * hpMultiplier);
            SetMaxHP(newMaxHP);

            // Restore HP proportionally
            int newCurrentHP = (int)(newMaxHP * hpPercent);
            CurrentHP = newCurrentHP;

            // Apply stat multipliers
            SetAttack((int)(Attack * statMultiplier));
            SetDefense((int)(Defense * statMultiplier));
            SetSpAttack((int)(SpAttack * statMultiplier));
            SetSpDefense((int)(SpDefense * statMultiplier));
            SetSpeed((int)(Speed * statMultiplier));
        }

        // Private setters using reflection to modify private set properties
        private void SetMaxHP(int value)
        {
            var property = typeof(Domain.Instances.Pokemon.PokemonInstance).GetProperty(nameof(MaxHP), BindingFlags.Public | BindingFlags.Instance);
            var setter = property?.GetSetMethod(nonPublic: true);
            setter?.Invoke(this, new object[] { value });
        }

        private void SetAttack(int value)
        {
            var property = typeof(Domain.Instances.Pokemon.PokemonInstance).GetProperty(nameof(Attack), BindingFlags.Public | BindingFlags.Instance);
            var setter = property?.GetSetMethod(nonPublic: true);
            setter?.Invoke(this, new object[] { value });
        }

        private void SetDefense(int value)
        {
            var property = typeof(Domain.Instances.Pokemon.PokemonInstance).GetProperty(nameof(Defense), BindingFlags.Public | BindingFlags.Instance);
            var setter = property?.GetSetMethod(nonPublic: true);
            setter?.Invoke(this, new object[] { value });
        }

        private void SetSpAttack(int value)
        {
            var property = typeof(Domain.Instances.Pokemon.PokemonInstance).GetProperty(nameof(SpAttack), BindingFlags.Public | BindingFlags.Instance);
            var setter = property?.GetSetMethod(nonPublic: true);
            setter?.Invoke(this, new object[] { value });
        }

        private void SetSpDefense(int value)
        {
            var property = typeof(Domain.Instances.Pokemon.PokemonInstance).GetProperty(nameof(SpDefense), BindingFlags.Public | BindingFlags.Instance);
            var setter = property?.GetSetMethod(nonPublic: true);
            setter?.Invoke(this, new object[] { value });
        }

        private void SetSpeed(int value)
        {
            var property = typeof(Domain.Instances.Pokemon.PokemonInstance).GetProperty(nameof(Speed), BindingFlags.Public | BindingFlags.Instance);
            var setter = property?.GetSetMethod(nonPublic: true);
            setter?.Invoke(this, new object[] { value });
        }
    }
}

