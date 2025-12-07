using System;
using PokemonUltimate.Core.Data.Constants;

namespace PokemonUltimate.Combat.Damage.Steps
{
    /// <summary>
    /// Applies damage multipliers from the attacker's held item.
    /// Handles items like Life Orb that boost damage.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.4: Damage Calculation Pipeline
    /// **Documentation**: See `docs/features/2-combat-system/2.4-damage-calculation-pipeline/architecture.md`
    /// </remarks>
    public class AttackerItemStep : IDamageStep
    {
        /// <summary>
        /// Processes the damage context and applies item-based damage multipliers.
        /// </summary>
        /// <param name="context">The damage context to modify.</param>
        public void Process(DamageContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context), ErrorMessages.ContextCannotBeNull);

            var attacker = context.Attacker.Pokemon;

            // Check if attacker has a held item
            if (attacker.HeldItem == null)
                return;

            // Create item modifier adapter
            var itemModifier = new ItemStatModifier(attacker.HeldItem);
            float multiplier = itemModifier.GetDamageMultiplier(context);

            // Apply multiplier if different from 1.0
            if (Math.Abs(multiplier - 1.0f) > 0.001f)
            {
                context.Multiplier *= multiplier;
            }
        }
    }
}

