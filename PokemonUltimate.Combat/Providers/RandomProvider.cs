using System;
using PokemonUltimate.Core.Data.Constants;

namespace PokemonUltimate.Combat.Providers
{
    /// <summary>
    /// Standard implementation of IRandomProvider using System.Random.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.3: Turn Order Resolution
    /// **Documentation**: See `docs/features/2-combat-system/2.3-turn-order-resolution/architecture.md`
    /// </remarks>
    public class RandomProvider : IRandomProvider
    {
        private readonly Random _random;

        /// <summary>
        /// Creates a new RandomProvider with a random seed.
        /// </summary>
        public RandomProvider()
        {
            _random = new Random();
        }

        /// <summary>
        /// Creates a new RandomProvider with a specific seed for reproducible results.
        /// </summary>
        /// <param name="seed">The seed value.</param>
        public RandomProvider(int seed)
        {
            _random = new Random(seed);
        }

        /// <summary>
        /// Returns a random integer between 0 and maxValue (exclusive).
        /// </summary>
        /// <param name="maxValue">The exclusive upper bound.</param>
        /// <returns>A random integer in the range [0, maxValue).</returns>
        /// <exception cref="ArgumentOutOfRangeException">If maxValue is less than 0.</exception>
        public int Next(int maxValue)
        {
            if (maxValue < 0)
                throw new ArgumentOutOfRangeException(nameof(maxValue), ErrorMessages.AmountCannotBeNegative);

            return _random.Next(maxValue);
        }

        /// <summary>
        /// Returns a random integer between minValue (inclusive) and maxValue (exclusive).
        /// </summary>
        /// <param name="minValue">The inclusive lower bound.</param>
        /// <param name="maxValue">The exclusive upper bound.</param>
        /// <returns>A random integer in the range [minValue, maxValue).</returns>
        /// <exception cref="ArgumentOutOfRangeException">If maxValue is less than minValue.</exception>
        public int Next(int minValue, int maxValue)
        {
            if (maxValue < minValue)
                throw new ArgumentOutOfRangeException(nameof(maxValue), "maxValue must be greater than or equal to minValue");

            return _random.Next(minValue, maxValue);
        }

        /// <summary>
        /// Returns a random float between 0.0 and 1.0.
        /// </summary>
        /// <returns>A random float in the range [0.0, 1.0).</returns>
        public float NextFloat()
        {
            return (float)_random.NextDouble();
        }

        /// <summary>
        /// Returns a random double between 0.0 and 1.0.
        /// </summary>
        /// <returns>A random double in the range [0.0, 1.0).</returns>
        public double NextDouble()
        {
            return _random.NextDouble();
        }
    }
}
