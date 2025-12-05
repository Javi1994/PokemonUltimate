using System;
using NUnit.Framework;
using PokemonUltimate.Combat;
using PokemonUltimate.Combat.Damage;
using PokemonUltimate.Combat.Factories;
using PokemonUltimate.Content.Catalogs.Moves;
using PokemonUltimate.Content.Catalogs.Pokemon;
using PokemonUltimate.Core.Blueprints;
using PokemonUltimate.Core.Enums;
using PokemonUltimate.Core.Factories;
using PokemonUltimate.Core.Instances;

namespace PokemonUltimate.Tests.Systems.Combat.Factories
{
    /// <summary>
    /// Tests for DamageContextFactory - factory for creating DamageContext instances.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.4: Damage Calculation Pipeline
    /// **Documentation**: See `docs/features/2-combat-system/2.4-damage-calculation-pipeline/architecture.md`
    /// </remarks>
    [TestFixture]
    public class DamageContextFactoryTests
    {
        private DamageContextFactory _factory;
        private BattleField _field;
        private BattleSlot _attackerSlot;
        private BattleSlot _defenderSlot;
        private MoveData _move;

        [SetUp]
        public void SetUp()
        {
            _factory = new DamageContextFactory();
            _field = new BattleField();

            var playerParty = new[] { PokemonFactory.Create(PokemonCatalog.Pikachu, 50) };
            var enemyParty = new[] { PokemonFactory.Create(PokemonCatalog.Squirtle, 50) };

            _field.Initialize(BattleRules.Singles, playerParty, enemyParty);
            _attackerSlot = _field.PlayerSide.Slots[0];
            _defenderSlot = _field.EnemySide.Slots[0];
            _move = MoveCatalog.Tackle;
        }

        #region CreateForMove Tests

        [Test]
        public void CreateForMove_ValidParameters_CreatesContext()
        {
            // Act
            var context = _factory.CreateForMove(_attackerSlot, _defenderSlot, _move, _field);

            // Assert
            Assert.That(context, Is.Not.Null);
            Assert.That(context.Attacker, Is.EqualTo(_attackerSlot));
            Assert.That(context.Defender, Is.EqualTo(_defenderSlot));
            Assert.That(context.Move, Is.EqualTo(_move));
            Assert.That(context.Field, Is.EqualTo(_field));
        }

        [Test]
        public void CreateForMove_WithForceCritical_SetsForceCritical()
        {
            // Act
            var context = _factory.CreateForMove(_attackerSlot, _defenderSlot, _move, _field, forceCritical: true);

            // Assert
            Assert.That(context.ForceCritical, Is.True);
        }

        [Test]
        public void CreateForMove_WithFixedRandomValue_SetsFixedRandomValue()
        {
            // Arrange
            const float fixedValue = 0.85f;

            // Act
            var context = _factory.CreateForMove(_attackerSlot, _defenderSlot, _move, _field, fixedRandomValue: fixedValue);

            // Assert
            Assert.That(context.FixedRandomValue, Is.EqualTo(fixedValue));
        }

        [Test]
        public void CreateForMove_NullAttacker_ThrowsArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                _factory.CreateForMove(null, _defenderSlot, _move, _field));
        }

        [Test]
        public void CreateForMove_NullDefender_ThrowsArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                _factory.CreateForMove(_attackerSlot, null, _move, _field));
        }

        [Test]
        public void CreateForMove_NullMove_ThrowsArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                _factory.CreateForMove(_attackerSlot, _defenderSlot, null, _field));
        }

        [Test]
        public void CreateForMove_NullField_ThrowsArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                _factory.CreateForMove(_attackerSlot, _defenderSlot, _move, null));
        }

        #endregion

        #region CreateForStatusDamage Tests

        [Test]
        public void CreateForStatusDamage_ValidParameters_CreatesContext()
        {
            // Arrange
            const int damage = 10;

            // Act
            var context = _factory.CreateForStatusDamage(_attackerSlot, damage, _field);

            // Assert
            Assert.That(context, Is.Not.Null);
            Assert.That(context.Attacker, Is.EqualTo(_attackerSlot));
            Assert.That(context.Defender, Is.EqualTo(_attackerSlot));
            Assert.That(context.BaseDamage, Is.EqualTo(damage));
            Assert.That(context.Multiplier, Is.EqualTo(1.0f));
            Assert.That(context.TypeEffectiveness, Is.EqualTo(1.0f));
            Assert.That(context.Move.Name, Is.EqualTo("Status Damage"));
            Assert.That(context.Move.Category, Is.EqualTo(MoveCategory.Status));
        }

        [Test]
        public void CreateForStatusDamage_NullSlot_ThrowsArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                _factory.CreateForStatusDamage(null, 10, _field));
        }

        [Test]
        public void CreateForStatusDamage_NullField_ThrowsArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                _factory.CreateForStatusDamage(_attackerSlot, 10, null));
        }

        #endregion

        #region CreateForHazardDamage Tests

        [Test]
        public void CreateForHazardDamage_ValidParameters_CreatesContext()
        {
            // Arrange
            const int damage = 12;

            // Act
            var context = _factory.CreateForHazardDamage(_attackerSlot, damage, _field);

            // Assert
            Assert.That(context, Is.Not.Null);
            Assert.That(context.Attacker, Is.EqualTo(_attackerSlot));
            Assert.That(context.Defender, Is.EqualTo(_attackerSlot));
            Assert.That(context.BaseDamage, Is.EqualTo(damage));
            Assert.That(context.Multiplier, Is.EqualTo(1.0f));
            Assert.That(context.TypeEffectiveness, Is.EqualTo(1.0f));
            Assert.That(context.Move.Name, Is.EqualTo("Entry Hazard"));
            Assert.That(context.Move.Category, Is.EqualTo(MoveCategory.Physical));
        }

        [Test]
        public void CreateForHazardDamage_NullSlot_ThrowsArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                _factory.CreateForHazardDamage(null, 10, _field));
        }

        [Test]
        public void CreateForHazardDamage_NullField_ThrowsArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                _factory.CreateForHazardDamage(_attackerSlot, 10, null));
        }

        #endregion

        #region CreateForRecoil Tests

        [Test]
        public void CreateForRecoil_ValidParameters_CreatesContext()
        {
            // Arrange
            const int damage = 15;

            // Act
            var context = _factory.CreateForRecoil(_attackerSlot, damage, _move, _field);

            // Assert
            Assert.That(context, Is.Not.Null);
            Assert.That(context.Attacker, Is.EqualTo(_attackerSlot));
            Assert.That(context.Defender, Is.EqualTo(_attackerSlot));
            Assert.That(context.BaseDamage, Is.EqualTo(damage));
            Assert.That(context.Multiplier, Is.EqualTo(1.0f));
            Assert.That(context.TypeEffectiveness, Is.EqualTo(1.0f));
            Assert.That(context.Move, Is.EqualTo(_move));
        }

        [Test]
        public void CreateForRecoil_NullSlot_ThrowsArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                _factory.CreateForRecoil(null, 10, _move, _field));
        }

        [Test]
        public void CreateForRecoil_NullMove_ThrowsArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                _factory.CreateForRecoil(_attackerSlot, 10, null, _field));
        }

        [Test]
        public void CreateForRecoil_NullField_ThrowsArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                _factory.CreateForRecoil(_attackerSlot, 10, _move, null));
        }

        #endregion

        #region CreateForCounter Tests

        [Test]
        public void CreateForCounter_ValidParameters_CreatesContext()
        {
            // Arrange
            const int damage = 20;

            // Act
            var context = _factory.CreateForCounter(_attackerSlot, _defenderSlot, damage, _move, _field);

            // Assert
            Assert.That(context, Is.Not.Null);
            Assert.That(context.Attacker, Is.EqualTo(_defenderSlot));
            Assert.That(context.Defender, Is.EqualTo(_attackerSlot));
            Assert.That(context.BaseDamage, Is.EqualTo(damage));
            Assert.That(context.Multiplier, Is.EqualTo(1.0f));
            Assert.That(context.TypeEffectiveness, Is.EqualTo(1.0f));
            Assert.That(context.Move, Is.EqualTo(_move));
        }

        [Test]
        public void CreateForCounter_NullAttacker_ThrowsArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                _factory.CreateForCounter(null, _defenderSlot, 10, _move, _field));
        }

        [Test]
        public void CreateForCounter_NullDefender_ThrowsArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                _factory.CreateForCounter(_attackerSlot, null, 10, _move, _field));
        }

        [Test]
        public void CreateForCounter_NullMove_ThrowsArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                _factory.CreateForCounter(_attackerSlot, _defenderSlot, 10, null, _field));
        }

        [Test]
        public void CreateForCounter_NullField_ThrowsArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                _factory.CreateForCounter(_attackerSlot, _defenderSlot, 10, _move, null));
        }

        #endregion
    }
}
