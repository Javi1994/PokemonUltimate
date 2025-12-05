using NUnit.Framework;
using PokemonUltimate.Core.Blueprints;
using PokemonUltimate.Core.Blueprints.Strategies;
using PokemonUltimate.Core.Enums;

namespace PokemonUltimate.Tests.Systems.Core.Blueprints.Strategies
{
    /// <summary>
    /// Tests for StatGetterRegistry - stat getter strategy registry.
    /// </summary>
    /// <remarks>
    /// **Feature**: 1: Game Data
    /// **Sub-Feature**: 1.1: Pokemon Data
    /// **Documentation**: See `docs/features/1-game-data/1.1-pokemon-data/architecture.md`
    /// </remarks>
    [TestFixture]
    public class StatGetterRegistryTests
    {
        private BaseStats _baseStats;

        [SetUp]
        public void SetUp()
        {
            _baseStats = new BaseStats
            {
                HP = 100,
                Attack = 80,
                Defense = 75,
                SpAttack = 90,
                SpDefense = 85,
                Speed = 95
            };
        }

        #region GetStat Tests

        [Test]
        public void GetStat_HPStat_ReturnsHPValue()
        {
            // Act
            var result = StatGetterRegistry.GetStat(_baseStats, Stat.HP);

            // Assert
            Assert.That(result, Is.EqualTo(100));
        }

        [Test]
        public void GetStat_AttackStat_ReturnsAttackValue()
        {
            // Act
            var result = StatGetterRegistry.GetStat(_baseStats, Stat.Attack);

            // Assert
            Assert.That(result, Is.EqualTo(80));
        }

        [Test]
        public void GetStat_DefenseStat_ReturnsDefenseValue()
        {
            // Act
            var result = StatGetterRegistry.GetStat(_baseStats, Stat.Defense);

            // Assert
            Assert.That(result, Is.EqualTo(75));
        }

        [Test]
        public void GetStat_SpAttackStat_ReturnsSpAttackValue()
        {
            // Act
            var result = StatGetterRegistry.GetStat(_baseStats, Stat.SpAttack);

            // Assert
            Assert.That(result, Is.EqualTo(90));
        }

        [Test]
        public void GetStat_SpDefenseStat_ReturnsSpDefenseValue()
        {
            // Act
            var result = StatGetterRegistry.GetStat(_baseStats, Stat.SpDefense);

            // Assert
            Assert.That(result, Is.EqualTo(85));
        }

        [Test]
        public void GetStat_SpeedStat_ReturnsSpeedValue()
        {
            // Act
            var result = StatGetterRegistry.GetStat(_baseStats, Stat.Speed);

            // Assert
            Assert.That(result, Is.EqualTo(95));
        }

        [Test]
        public void GetStat_AccuracyStat_ReturnsZero()
        {
            // Act
            var result = StatGetterRegistry.GetStat(_baseStats, Stat.Accuracy);

            // Assert
            Assert.That(result, Is.EqualTo(0));
        }

        [Test]
        public void GetStat_EvasionStat_ReturnsZero()
        {
            // Act
            var result = StatGetterRegistry.GetStat(_baseStats, Stat.Evasion);

            // Assert
            Assert.That(result, Is.EqualTo(0));
        }

        #endregion
    }
}
