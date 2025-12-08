using System;
using PokemonUltimate.Combat.Damage.Definition;
using PokemonUltimate.Core.Data.Constants;

namespace PokemonUltimate.Combat.Damage.Steps
{
    /// <summary>
    /// Applies damage multipliers from the attacker's ability.
    /// Handles abilities like Blaze, Torrent, Overgrow that boost damage when HP is low.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.4: Damage Calculation Pipeline
    /// **Documentation**: See `docs/features/2-combat-system/2.4-damage-calculation-pipeline/architecture.md`
    /// </remarks>
    public class AttackerAbilityStep : IDamageStep
    {
        /// <summary>
        /// Processes the damage context and applies ability-based damage multipliers.
        /// </summary>
        /// <param name="context">The damage context to modify.</param>
        public void Process(DamageContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context), ErrorMessages.ContextCannotBeNull);

            var attacker = context.Attacker.Pokemon;

            // Check if attacker has an ability
            if (attacker.Ability == null)
                return;

            // Create ability modifier adapter
            var abilityModifier = new AbilityStatModifier(attacker.Ability);
            float multiplier = abilityModifier.GetDamageMultiplier(context);

            // Apply multiplier if different from 1.0
            if (Math.Abs(multiplier - 1.0f) > 0.001f)
            {
                context.Multiplier *= multiplier;
            }
        }
    }
}

