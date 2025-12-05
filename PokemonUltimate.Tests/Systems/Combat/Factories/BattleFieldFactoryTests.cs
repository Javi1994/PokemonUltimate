using System;
using System.Collections.Generic;
using NUnit.Framework;
using PokemonUltimate.Combat;
using PokemonUltimate.Combat.Factories;
using PokemonUltimate.Content.Catalogs.Pokemon;
using PokemonUltimate.Core.Factories;
using PokemonUltimate.Core.Instances;

namespace PokemonUltimate.Tests.Systems.Combat.Factories
{
    /// <summary>
    /// Tests for BattleFieldFactory - factory for creating BattleField instances.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.6: Combat Engine
    /// **Documentation**: See `docs/features/2-combat-system/2.6-combat-engine/architecture.md`
    /// </remarks>
    [TestFixture]
    public class BattleFieldFactoryTests
    {
        private BattleFieldFactory _factory;
        private BattleRules _rules;
        private IReadOnlyList<PokemonInstance> _playerParty;
        private IReadOnlyList<PokemonInstance> _enemyParty;

        [SetUp]
        public void SetUp()
        {
            _factory = new BattleFieldFactory();
            _rules = BattleRules.Singles;
            _playerParty = new[] { PokemonFactory.Create(PokemonCatalog.Pikachu, 50) };
            _enemyParty = new[] { PokemonFactory.Create(PokemonCatalog.Squirtle, 50) };
        }

        #region Create Tests

        [Test]
        public void Create_ValidParameters_CreatesInitializedField()
        {
            // Act
            var field = _factory.Create(_rules, _playerParty, _enemyParty);

            // Assert
            Assert.That(field, Is.Not.Null);
            Assert.That(field.PlayerSide, Is.Not.Null);
            Assert.That(field.EnemySide, Is.Not.Null);
            Assert.That(field.PlayerSide.Slots.Count, Is.EqualTo(1));
            Assert.That(field.EnemySide.Slots.Count, Is.EqualTo(1));
        }

        [Test]
        public void Create_SinglesBattle_CreatesSingleSlots()
        {
            // Arrange
            var rules = BattleRules.Singles;

            // Act
            var field = _factory.Create(rules, _playerParty, _enemyParty);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(field.PlayerSide.Slots.Count, Is.EqualTo(1));
                Assert.That(field.EnemySide.Slots.Count, Is.EqualTo(1));
            });
        }

        [Test]
        public void Create_DoublesBattle_CreatesDoubleSlots()
        {
            // Arrange
            var rules = new BattleRules { PlayerSlots = 2, EnemySlots = 2 };
            var playerParty = new[]
            {
                PokemonFactory.Create(PokemonCatalog.Pikachu, 50),
                PokemonFactory.Create(PokemonCatalog.Charmander, 50)
            };
            var enemyParty = new[]
            {
                PokemonFactory.Create(PokemonCatalog.Squirtle, 50),
                PokemonFactory.Create(PokemonCatalog.Bulbasaur, 50)
            };

            // Act
            var field = _factory.Create(rules, playerParty, enemyParty);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(field.PlayerSide.Slots.Count, Is.EqualTo(2));
                Assert.That(field.EnemySide.Slots.Count, Is.EqualTo(2));
            });
        }

        [Test]
        public void Create_NullRules_ThrowsArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                _factory.Create(null, _playerParty, _enemyParty));
        }

        [Test]
        public void Create_NullPlayerParty_ThrowsArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                _factory.Create(_rules, null, _enemyParty));
        }

        [Test]
        public void Create_NullEnemyParty_ThrowsArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                _factory.Create(_rules, _playerParty, null));
        }

        [Test]
        public void Create_InitializesFieldCorrectly()
        {
            // Act
            var field = _factory.Create(_rules, _playerParty, _enemyParty);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(field.PlayerSide.Slots[0].Pokemon, Is.Not.Null);
                Assert.That(field.EnemySide.Slots[0].Pokemon, Is.Not.Null);
                Assert.That(field.PlayerSide.Slots[0].Pokemon.Species.Name, Is.EqualTo("Pikachu"));
                Assert.That(field.EnemySide.Slots[0].Pokemon.Species.Name, Is.EqualTo("Squirtle"));
            });
        }

        #endregion
    }
}
