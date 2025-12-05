using NUnit.Framework;
using PokemonUltimate.Content.Catalogs.Pokemon;
using PokemonUltimate.Core.Enums;
using PokemonUltimate.Core.Factories;
using PokemonUltimate.Core.Instances;
using PokemonUltimate.Core.Instances.Strategies;

namespace PokemonUltimate.Tests.Systems.Core.Instances.Strategies
{
    /// <summary>
    /// Tests for PokemonStatGetterRegistry - PokemonInstance stat getter registry.
    /// </summary>
    /// <remarks>
    /// **Feature**: 1: Game Data
    /// **Sub-Feature**: 1.1: Pokemon Data
    /// **Documentation**: See `docs/features/1-game-data/1.1-pokemon-data/architecture.md`
    /// </remarks>
    [TestFixture]
    public class PokemonStatGetterRegistryTests
    {
        private PokemonInstance _pokemon;

        [SetUp]
        public void SetUp()
        {
            _pokemon = PokemonInstanceBuilder.Create(PokemonCatalog.Pikachu, 25).Build();
        }

        #region GetStat Tests

        [Test]
        public void GetStat_HPStat_ReturnsMaxHP()
        {
            // Act
            var result = PokemonStatGetterRegistry.GetStat(_pokemon, Stat.HP);

            // Assert
            Assert.That(result, Is.EqualTo(_pokemon.MaxHP));
        }

        [Test]
        public void GetStat_AttackStat_ReturnsAttack()
        {
            // Act
            var result = PokemonStatGetterRegistry.GetStat(_pokemon, Stat.Attack);

            // Assert
            Assert.That(result, Is.EqualTo(_pokemon.Attack));
        }

        [Test]
        public void GetStat_DefenseStat_ReturnsDefense()
        {
            // Act
            var result = PokemonStatGetterRegistry.GetStat(_pokemon, Stat.Defense);

            // Assert
            Assert.That(result, Is.EqualTo(_pokemon.Defense));
        }

        [Test]
        public void GetStat_SpAttackStat_ReturnsSpAttack()
        {
            // Act
            var result = PokemonStatGetterRegistry.GetStat(_pokemon, Stat.SpAttack);

            // Assert
            Assert.That(result, Is.EqualTo(_pokemon.SpAttack));
        }

        [Test]
        public void GetStat_SpDefenseStat_ReturnsSpDefense()
        {
            // Act
            var result = PokemonStatGetterRegistry.GetStat(_pokemon, Stat.SpDefense);

            // Assert
            Assert.That(result, Is.EqualTo(_pokemon.SpDefense));
        }

        [Test]
        public void GetStat_SpeedStat_ReturnsSpeed()
        {
            // Act
            var result = PokemonStatGetterRegistry.GetStat(_pokemon, Stat.Speed);

            // Assert
            Assert.That(result, Is.EqualTo(_pokemon.Speed));
        }

        [Test]
        public void GetStat_AccuracyStat_Returns100()
        {
            // Act
            var result = PokemonStatGetterRegistry.GetStat(_pokemon, Stat.Accuracy);

            // Assert
            Assert.That(result, Is.EqualTo(100));
        }

        [Test]
        public void GetStat_EvasionStat_Returns100()
        {
            // Act
            var result = PokemonStatGetterRegistry.GetStat(_pokemon, Stat.Evasion);

            // Assert
            Assert.That(result, Is.EqualTo(100));
        }

        #endregion
    }
}
