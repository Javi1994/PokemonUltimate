using PokemonUltimate.Combat.Helpers;
using PokemonUltimate.Combat.Providers;

namespace PokemonUltimate.Tests.Systems.Combat.Helpers
{
    /// <summary>
    /// Helper methods for creating test objects with deterministic behavior.
    /// </summary>
    public static class TestHelpers
    {
        /// <summary>
        /// Creates an AccuracyChecker that always hits (for deterministic testing).
        /// Uses a RandomProvider that returns low values for accuracy checks.
        /// </summary>
        public static AccuracyChecker CreateAlwaysHitAccuracyChecker()
        {
            // Use a RandomProvider with seed that returns low values for accuracy checks
            // AccuracyChecker uses NextFloat() * 100f, so we need values < accuracy%
            // Seed 1 gives predictable low values that will hit for moves with 95%+ accuracy
            var alwaysHitRandom = new AlwaysHitRandomProvider();
            return new AccuracyChecker(alwaysHitRandom);
        }

        /// <summary>
        /// Creates a RandomProvider with a fixed seed for deterministic testing.
        /// </summary>
        public static RandomProvider CreateFixedRandomProvider(int seed = 42)
        {
            return new RandomProvider(seed);
        }

        /// <summary>
        /// Creates a RandomProvider that always returns fixed values for deterministic damage calculation.
        /// Returns 1.0 for NextFloat/NextDouble to get maximum damage multiplier (1.0x).
        /// </summary>
        public static IRandomProvider CreateFixedValueRandomProvider(float fixedFloatValue = 1.0f, double fixedDoubleValue = 1.0)
        {
            return new FixedValueRandomProvider(fixedFloatValue, fixedDoubleValue);
        }
    }

    /// <summary>
    /// RandomProvider that always returns low values to guarantee hits in tests.
    /// </summary>
    internal class AlwaysHitRandomProvider : IRandomProvider
    {
        public int Next(int maxValue)
        {
            return 0; // Always return 0 for deterministic testing
        }

        public int Next(int minValue, int maxValue)
        {
            return minValue; // Always return minimum for deterministic testing
        }

        public float NextFloat()
        {
            return 0.0f; // Always return 0.0 for accuracy checks (guarantees hit for any accuracy > 0%)
        }

        public double NextDouble()
        {
            return 0.0; // Always return 0.0 for deterministic testing
        }
    }

    /// <summary>
    /// RandomProvider that always returns a fixed value for deterministic damage calculation.
    /// Returns 1.0 for NextFloat/NextDouble to get maximum damage multiplier (1.0x).
    /// </summary>
    internal class FixedValueRandomProvider : IRandomProvider
    {
        private readonly float _fixedFloatValue;
        private readonly double _fixedDoubleValue;

        public FixedValueRandomProvider(float fixedFloatValue = 1.0f, double fixedDoubleValue = 1.0)
        {
            _fixedFloatValue = fixedFloatValue;
            _fixedDoubleValue = fixedDoubleValue;
        }

        public int Next(int maxValue)
        {
            return maxValue > 0 ? maxValue - 1 : 0; // Return max-1 for deterministic testing
        }

        public int Next(int minValue, int maxValue)
        {
            return maxValue > minValue ? maxValue - 1 : minValue; // Return max-1 for deterministic testing
        }

        public float NextFloat()
        {
            return _fixedFloatValue; // Always return fixed value for deterministic damage
        }

        public double NextDouble()
        {
            return _fixedDoubleValue; // Always return fixed value for deterministic damage
        }
    }
}

