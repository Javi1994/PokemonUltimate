namespace PokemonUltimate.Core.Infrastructure.Providers.Definition
{
    /// <summary>
    /// Provides random number generation for game calculations.
    /// Allows for dependency injection and testability by replacing static Random instances.
    /// </summary>
    /// <remarks>
    /// **Feature**: 1: Game Data
    /// **Sub-Feature**: 1.12: Factories & Calculators
    /// **Documentation**: See `docs/features/1-game-data/1.12-factories-calculators/README.md`
    /// </remarks>
    public interface IRandomProvider
    {
        /// <summary>
        /// Returns a random integer between 0 and maxValue (exclusive).
        /// </summary>
        /// <param name="maxValue">The exclusive upper bound.</param>
        /// <returns>A random integer in the range [0, maxValue).</returns>
        int Next(int maxValue);

        /// <summary>
        /// Returns a random integer between minValue (inclusive) and maxValue (exclusive).
        /// </summary>
        /// <param name="minValue">The inclusive lower bound.</param>
        /// <param name="maxValue">The exclusive upper bound.</param>
        /// <returns>A random integer in the range [minValue, maxValue).</returns>
        int Next(int minValue, int maxValue);

        /// <summary>
        /// Returns a random float between 0.0 and 1.0.
        /// </summary>
        /// <returns>A random float in the range [0.0, 1.0).</returns>
        float NextFloat();

        /// <summary>
        /// Returns a random double between 0.0 and 1.0.
        /// </summary>
        /// <returns>A random double in the range [0.0, 1.0).</returns>
        double NextDouble();
    }
}
