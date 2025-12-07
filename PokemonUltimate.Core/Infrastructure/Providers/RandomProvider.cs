using System;
using PokemonUltimate.Core.Infrastructure.Providers.Definition;

namespace PokemonUltimate.Core.Infrastructure.Providers
{
    /// <summary>
    /// Default implementation of IRandomProvider using System.Random.
    /// </summary>
    /// <remarks>
    /// **Feature**: 1: Game Data
    /// **Sub-Feature**: 1.12: Factories & Calculators
    /// **Documentation**: See `docs/features/1-game-data/1.12-factories-calculators/README.md`
    /// </remarks>
    public class RandomProvider : IRandomProvider
    {
        private readonly Random _random;

        /// <summary>
        /// Creates a new RandomProvider with optional seed for deterministic generation.
        /// </summary>
        /// <param name="seed">Optional seed for random number generation. If null, uses system time.</param>
        public RandomProvider(int? seed = null)
        {
            _random = seed.HasValue ? new Random(seed.Value) : new Random();
        }

        /// <summary>
        /// Returns a random integer between 0 and maxValue (exclusive).
        /// </summary>
        public int Next(int maxValue)
        {
            return _random.Next(maxValue);
        }

        /// <summary>
        /// Returns a random integer between minValue (inclusive) and maxValue (exclusive).
        /// </summary>
        public int Next(int minValue, int maxValue)
        {
            return _random.Next(minValue, maxValue);
        }

        /// <summary>
        /// Returns a random float between 0.0 and 1.0.
        /// </summary>
        public float NextFloat()
        {
            return (float)_random.NextDouble();
        }

        /// <summary>
        /// Returns a random double between 0.0 and 1.0.
        /// </summary>
        public double NextDouble()
        {
            return _random.NextDouble();
        }
    }
}
