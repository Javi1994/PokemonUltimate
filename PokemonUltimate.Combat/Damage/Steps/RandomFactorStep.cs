using System;
using PokemonUltimate.Combat.Providers;
using PokemonUltimate.Core.Data.Constants;

namespace PokemonUltimate.Combat.Damage.Steps
{
    /// <summary>
    /// Applies the random damage factor (0.85 to 1.0).
    /// This creates natural variance in damage.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.4: Damage Calculation Pipeline
    /// **Documentation**: See `docs/features/2-combat-system/2.4-damage-calculation-pipeline/architecture.md`
    /// </remarks>
    public class RandomFactorStep : IDamageStep
    {
        private const float MinFactor = 0.85f;
        private const float MaxFactor = 1.0f;

        private readonly IRandomProvider _randomProvider;

        /// <summary>
        /// Creates a new RandomFactorStep with a random provider.
        /// </summary>
        /// <param name="randomProvider">The random provider for generating random factors. Cannot be null.</param>
        /// <exception cref="ArgumentNullException">If randomProvider is null.</exception>
        public RandomFactorStep(IRandomProvider randomProvider)
        {
            _randomProvider = randomProvider ?? throw new ArgumentNullException(nameof(randomProvider), ErrorMessages.PokemonCannotBeNull);
        }

        public void Process(DamageContext context)
        {
            float randomFactor;

            if (context.FixedRandomValue.HasValue)
            {
                // Use fixed value for testing (scale from 0-1 to 0.85-1.0)
                randomFactor = MinFactor + (context.FixedRandomValue.Value * (MaxFactor - MinFactor));
            }
            else
            {
                // Generate random factor between 0.85 and 1.0
                randomFactor = MinFactor + (_randomProvider.NextFloat() * (MaxFactor - MinFactor));
            }

            context.RandomFactor = randomFactor;
            context.Multiplier *= randomFactor;
        }
    }
}

