using PokemonUltimate.Combat.Foundation.Field;
using PokemonUltimate.Core.Data.Enums;

namespace PokemonUltimate.Combat.Systems.Damage
{
    /// <summary>
    /// Interface for objects that provide passive stat and damage modifications.
    /// Used by abilities and items that modify calculations without generating Actions.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.4: Damage Calculation Pipeline
    /// **Documentation**: See `docs/features/2-combat-system/2.4-damage-calculation-pipeline/architecture.md`
    /// </remarks>
    public interface IStatModifier
    {
        /// <summary>
        /// Gets the stat multiplier for a specific stat.
        /// Returns 1.0f if this modifier doesn't affect the stat.
        /// </summary>
        /// <param name="holder">The slot holding this modifier (Pokemon with ability/item).</param>
        /// <param name="stat">The stat being queried.</param>
        /// <param name="field">The battlefield context.</param>
        /// <returns>Multiplier for the stat (1.0f = no change).</returns>
        float GetStatMultiplier(BattleSlot holder, Stat stat, BattleField field);

        /// <summary>
        /// Gets the damage multiplier for an attack.
        /// Returns 1.0f if this modifier doesn't affect damage.
        /// </summary>
        /// <param name="context">The damage calculation context.</param>
        /// <returns>Multiplier for the damage (1.0f = no change).</returns>
        float GetDamageMultiplier(DamageContext context);
    }
}

