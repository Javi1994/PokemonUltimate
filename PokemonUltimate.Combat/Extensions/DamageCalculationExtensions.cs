using PokemonUltimate.Combat.Engine;

namespace PokemonUltimate.Combat.Extensions
{
    /// <summary>
    /// Extension methods for damage calculations.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.4: Damage Calculation Pipeline
    /// **Documentation**: See `docs/features/2-combat-system/2.4-damage-calculation-pipeline/architecture.md`
    /// </remarks>
    public static class DamageCalculationExtensions
    {
        /// <summary>
        /// Ensures damage is at least the minimum required (1 HP).
        /// </summary>
        /// <param name="damage">The damage value to ensure.</param>
        /// <returns>The damage value, guaranteed to be at least the minimum.</returns>
        public static int EnsureMinimumDamage(this int damage)
        {
            return System.Math.Max(EndOfTurnConstants.MinimumDamage, damage);
        }
    }
}
