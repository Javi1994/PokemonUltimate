using PokemonUltimate.Combat.Foundation.Field;
using PokemonUltimate.Core.Data.Blueprints;

namespace PokemonUltimate.Combat.Systems.Damage.Definition
{
    /// <summary>
    /// Interface for damage calculation pipeline.
    /// Provides abstraction for damage calculation, enabling dependency injection and testability.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.4: Damage Calculation Pipeline
    /// **Documentation**: See `docs/features/2-combat-system/2.4-damage-calculation-pipeline/architecture.md`
    /// </remarks>
    public interface IDamagePipeline
    {
        /// <summary>
        /// Calculates damage for an attack.
        /// </summary>
        /// <param name="attacker">The attacking slot.</param>
        /// <param name="defender">The defending slot.</param>
        /// <param name="move">The move being used.</param>
        /// <param name="field">The battlefield.</param>
        /// <param name="forceCritical">Force a critical hit (for testing).</param>
        /// <param name="fixedRandomValue">Use a fixed random value 0-1 (for testing).</param>
        /// <returns>The damage context with calculated values.</returns>
        DamageContext Calculate(
            BattleSlot attacker,
            BattleSlot defender,
            MoveData move,
            BattleField field,
            bool forceCritical = false,
            float? fixedRandomValue = null);
    }
}
