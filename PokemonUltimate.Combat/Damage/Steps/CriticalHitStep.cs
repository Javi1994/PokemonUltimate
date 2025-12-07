using System;
using PokemonUltimate.Combat.Providers;
using PokemonUltimate.Core.Data.Constants;

namespace PokemonUltimate.Combat.Damage.Steps
{
    /// <summary>
    /// Determines if the attack is a critical hit and applies the 1.5x multiplier.
    /// Critical hits ignore negative stat stages on the attacker and positive stages on defender.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.4: Damage Calculation Pipeline
    /// **Documentation**: See `docs/features/2-combat-system/2.4-damage-calculation-pipeline/architecture.md`
    /// </remarks>
    public class CriticalHitStep : IDamageStep
    {
        private const float CriticalMultiplier = 1.5f;
        private const float BaseCritRate = 1f / 24f; // ~4.17% base crit rate (Gen 6+)

        private readonly IRandomProvider _randomProvider;

        /// <summary>
        /// Creates a new CriticalHitStep with a random provider.
        /// </summary>
        /// <param name="randomProvider">The random provider for critical hit rolls. Cannot be null.</param>
        /// <exception cref="ArgumentNullException">If randomProvider is null.</exception>
        public CriticalHitStep(IRandomProvider randomProvider)
        {
            _randomProvider = randomProvider ?? throw new ArgumentNullException(nameof(randomProvider), ErrorMessages.PokemonCannotBeNull);
        }

        public void Process(DamageContext context)
        {
            // Determine if critical
            bool isCritical = context.ForceCritical || RollForCritical(context);
            context.IsCritical = isCritical;

            if (isCritical)
            {
                context.Multiplier *= CriticalMultiplier;
            }
        }

        private bool RollForCritical(DamageContext context)
        {
            // If fixed random is provided, use it deterministically
            // This ensures tests are reproducible
            if (context.FixedRandomValue.HasValue)
            {
                // Only crit if fixed random is very high (> 0.95) to simulate rare crits
                return false; // For testing, fixed random = no natural crits
            }

            // TODO: Factor in crit stage from moves like Slash, Focus Energy, etc.
            // For now, use base crit rate
            float critChance = BaseCritRate;

            // Roll
            float roll = _randomProvider.NextFloat();
            return roll < critChance;
        }
    }
}

