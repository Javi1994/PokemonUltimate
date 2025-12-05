using System;
using NUnit.Framework;
using PokemonUltimate.Combat.Providers;

namespace PokemonUltimate.Tests.Systems.Combat.Providers
{
    /// <summary>
    /// Tests for RandomProvider - random number generation for battle calculations.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.3: Turn Order Resolution
    /// **Documentation**: See `docs/features/2-combat-system/2.3-turn-order-resolution/architecture.md`
    /// </remarks>
    [TestFixture]
    public class RandomProviderTests
    {
        #region Constructor Tests

        [Test]
        public void Constructor_DefaultSeed_CreatesInstance()
        {
            // Act
            var provider = new RandomProvider();

            // Assert
            Assert.That(provider, Is.Not.Null);
        }

        [Test]
        public void Constructor_WithSeed_CreatesInstance()
        {
            // Act
            var provider = new RandomProvider(42);

            // Assert
            Assert.That(provider, Is.Not.Null);
        }

        #endregion

        #region Next(int maxValue) Tests

        [Test]
        public void Next_MaxValue_ReturnsValueInRange()
        {
            // Arrange
            var provider = new RandomProvider(42);
            const int maxValue = 100;

            // Act
            var result = provider.Next(maxValue);

            // Assert
            Assert.That(result, Is.GreaterThanOrEqualTo(0));
            Assert.That(result, Is.LessThan(maxValue));
        }

        [Test]
        public void Next_MaxValueZero_ReturnsZero()
        {
            // Arrange
            var provider = new RandomProvider(42);

            // Act
            var result = provider.Next(0);

            // Assert
            Assert.That(result, Is.EqualTo(0));
        }

        [Test]
        public void Next_MaxValueOne_ReturnsZero()
        {
            // Arrange
            var provider = new RandomProvider(42);

            // Act
            var result = provider.Next(1);

            // Assert
            Assert.That(result, Is.EqualTo(0));
        }

        [Test]
        public void Next_NegativeMaxValue_ThrowsArgumentOutOfRangeException()
        {
            // Arrange
            var provider = new RandomProvider(42);

            // Act & Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => provider.Next(-1));
        }

        [Test]
        public void Next_SameSeed_ProducesSameSequence()
        {
            // Arrange
            var provider1 = new RandomProvider(42);
            var provider2 = new RandomProvider(42);

            // Act
            var result1 = provider1.Next(100);
            var result2 = provider2.Next(100);

            // Assert
            Assert.That(result1, Is.EqualTo(result2));
        }

        #endregion

        #region Next(int minValue, int maxValue) Tests

        [Test]
        public void Next_MinMaxValue_ReturnsValueInRange()
        {
            // Arrange
            var provider = new RandomProvider(42);
            const int minValue = 10;
            const int maxValue = 20;

            // Act
            var result = provider.Next(minValue, maxValue);

            // Assert
            Assert.That(result, Is.GreaterThanOrEqualTo(minValue));
            Assert.That(result, Is.LessThan(maxValue));
        }

        [Test]
        public void Next_SameMinMax_ReturnsMinValue()
        {
            // Arrange
            var provider = new RandomProvider(42);
            const int value = 10;

            // Act
            var result = provider.Next(value, value);

            // Assert
            Assert.That(result, Is.EqualTo(value));
        }

        [Test]
        public void Next_MaxLessThanMin_ThrowsArgumentOutOfRangeException()
        {
            // Arrange
            var provider = new RandomProvider(42);

            // Act & Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => provider.Next(10, 5));
        }

        [Test]
        public void Next_SameSeedMinMax_ProducesSameSequence()
        {
            // Arrange
            var provider1 = new RandomProvider(42);
            var provider2 = new RandomProvider(42);

            // Act
            var result1 = provider1.Next(10, 20);
            var result2 = provider2.Next(10, 20);

            // Assert
            Assert.That(result1, Is.EqualTo(result2));
        }

        #endregion

        #region NextFloat() Tests

        [Test]
        public void NextFloat_ReturnsValueInRange()
        {
            // Arrange
            var provider = new RandomProvider(42);

            // Act
            var result = provider.NextFloat();

            // Assert
            Assert.That(result, Is.GreaterThanOrEqualTo(0.0f));
            Assert.That(result, Is.LessThan(1.0f));
        }

        [Test]
        public void NextFloat_SameSeed_ProducesSameValue()
        {
            // Arrange
            var provider1 = new RandomProvider(42);
            var provider2 = new RandomProvider(42);

            // Act
            var result1 = provider1.NextFloat();
            var result2 = provider2.NextFloat();

            // Assert
            Assert.That(result1, Is.EqualTo(result2));
        }

        [Test]
        public void NextFloat_MultipleCalls_ProducesDifferentValues()
        {
            // Arrange
            var provider = new RandomProvider(42);
            var results = new float[10];

            // Act
            for (int i = 0; i < 10; i++)
            {
                results[i] = provider.NextFloat();
            }

            // Assert - At least one value should be different (very unlikely all are same)
            var firstValue = results[0];
            bool hasDifferentValue = false;
            for (int i = 1; i < 10; i++)
            {
                if (results[i] != firstValue)
                {
                    hasDifferentValue = true;
                    break;
                }
            }
            Assert.That(hasDifferentValue, Is.True, "Multiple calls should produce different values");
        }

        #endregion

        #region NextDouble() Tests

        [Test]
        public void NextDouble_ReturnsValueInRange()
        {
            // Arrange
            var provider = new RandomProvider(42);

            // Act
            var result = provider.NextDouble();

            // Assert
            Assert.That(result, Is.GreaterThanOrEqualTo(0.0));
            Assert.That(result, Is.LessThan(1.0));
        }

        [Test]
        public void NextDouble_SameSeed_ProducesSameValue()
        {
            // Arrange
            var provider1 = new RandomProvider(42);
            var provider2 = new RandomProvider(42);

            // Act
            var result1 = provider1.NextDouble();
            var result2 = provider2.NextDouble();

            // Assert
            Assert.That(result1, Is.EqualTo(result2));
        }

        #endregion
    }
}
