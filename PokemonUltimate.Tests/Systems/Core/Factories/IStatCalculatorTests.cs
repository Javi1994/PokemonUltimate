using NUnit.Framework;
using PokemonUltimate.Core.Enums;
using PokemonUltimate.Core.Factories;

namespace PokemonUltimate.Tests.Systems.Core.Factories
{
    /// <summary>
    /// Tests for IStatCalculator interface implementation.
    /// </summary>
    /// <remarks>
    /// **Feature**: 1: Game Data
    /// **Sub-Feature**: 1.12: Factories & Calculators
    /// **Documentation**: See `docs/features/1-game-data/1.12-factories-calculators/README.md`
    /// </remarks>
    [TestFixture]
    public class IStatCalculatorTests
    {
        private IStatCalculator _calculator;

        [SetUp]
        public void SetUp()
        {
            _calculator = StatCalculator.Default;
        }

        #region Interface Implementation Tests

        [Test]
        public void StatCalculator_ImplementsIStatCalculator()
        {
            // Assert
            Assert.That(_calculator, Is.InstanceOf<IStatCalculator>());
        }

        [Test]
        public void CalculateHP_ThroughInterface_ReturnsCorrectValue()
        {
            // Arrange
            int baseHP = 100;
            int level = 50;

            // Act
            var result = _calculator.CalculateHP(baseHP, level);

            // Assert
            Assert.That(result, Is.GreaterThan(0));
        }

        [Test]
        public void CalculateStat_ThroughInterface_ReturnsCorrectValue()
        {
            // Arrange
            int baseStat = 100;
            int level = 50;
            Nature nature = Nature.Hardy;
            Stat stat = Stat.Attack;

            // Act
            var result = _calculator.CalculateStat(baseStat, level, nature, stat);

            // Assert
            Assert.That(result, Is.GreaterThan(0));
        }

        [Test]
        public void GetStageMultiplier_ThroughInterface_ReturnsCorrectMultiplier()
        {
            // Act & Assert
            Assert.That(_calculator.GetStageMultiplier(0), Is.EqualTo(1.0f));
            Assert.That(_calculator.GetStageMultiplier(1), Is.GreaterThan(1.0f));
            Assert.That(_calculator.GetStageMultiplier(-1), Is.LessThan(1.0f));
        }

        #endregion
    }
}
