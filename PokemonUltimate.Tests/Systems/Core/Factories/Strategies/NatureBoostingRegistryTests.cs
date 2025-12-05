using NUnit.Framework;
using PokemonUltimate.Core.Enums;
using PokemonUltimate.Core.Factories.Strategies.NatureBoosting;

namespace PokemonUltimate.Tests.Systems.Core.Factories.Strategies
{
    /// <summary>
    /// Tests for NatureBoostingRegistry - nature boosting strategy registry.
    /// </summary>
    /// <remarks>
    /// **Feature**: 1: Game Data
    /// **Sub-Feature**: 1.12: Factories & Calculators
    /// **Documentation**: See `docs/features/1-game-data/1.12-factories-calculators/README.md`
    /// </remarks>
    [TestFixture]
    public class NatureBoostingRegistryTests
    {
        #region GetBoostingNature Tests

        [Test]
        public void GetBoostingNature_AttackStat_ReturnsAdamant()
        {
            // Act
            var nature = NatureBoostingRegistry.GetBoostingNature(Stat.Attack);

            // Assert
            Assert.That(nature, Is.EqualTo(Nature.Adamant));
        }

        [Test]
        public void GetBoostingNature_DefenseStat_ReturnsBold()
        {
            // Act
            var nature = NatureBoostingRegistry.GetBoostingNature(Stat.Defense);

            // Assert
            Assert.That(nature, Is.EqualTo(Nature.Bold));
        }

        [Test]
        public void GetBoostingNature_SpAttackStat_ReturnsModest()
        {
            // Act
            var nature = NatureBoostingRegistry.GetBoostingNature(Stat.SpAttack);

            // Assert
            Assert.That(nature, Is.EqualTo(Nature.Modest));
        }

        [Test]
        public void GetBoostingNature_SpDefenseStat_ReturnsCalm()
        {
            // Act
            var nature = NatureBoostingRegistry.GetBoostingNature(Stat.SpDefense);

            // Assert
            Assert.That(nature, Is.EqualTo(Nature.Calm));
        }

        [Test]
        public void GetBoostingNature_SpeedStat_ReturnsJolly()
        {
            // Act
            var nature = NatureBoostingRegistry.GetBoostingNature(Stat.Speed);

            // Assert
            Assert.That(nature, Is.EqualTo(Nature.Jolly));
        }

        [Test]
        public void GetBoostingNature_HPStat_ReturnsHardy()
        {
            // Act
            var nature = NatureBoostingRegistry.GetBoostingNature(Stat.HP);

            // Assert
            Assert.That(nature, Is.EqualTo(Nature.Hardy));
        }

        [Test]
        public void GetBoostingNature_AccuracyStat_ReturnsHardy()
        {
            // Act
            var nature = NatureBoostingRegistry.GetBoostingNature(Stat.Accuracy);

            // Assert
            Assert.That(nature, Is.EqualTo(Nature.Hardy));
        }

        [Test]
        public void GetBoostingNature_EvasionStat_ReturnsHardy()
        {
            // Act
            var nature = NatureBoostingRegistry.GetBoostingNature(Stat.Evasion);

            // Assert
            Assert.That(nature, Is.EqualTo(Nature.Hardy));
        }

        #endregion
    }
}
