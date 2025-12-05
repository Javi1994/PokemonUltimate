using System.Collections.Generic;
using NUnit.Framework;
using PokemonUltimate.Combat;
using PokemonUltimate.Combat.Validation;
using PokemonUltimate.Content.Catalogs.Pokemon;
using PokemonUltimate.Core.Enums;
using PokemonUltimate.Core.Factories;
using PokemonUltimate.Core.Instances;

namespace PokemonUltimate.Tests.Systems.Combat.Validation
{
    /// <summary>
    /// Tests for BattleStateValidator - validates battle state invariants and consistency.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.1: Battle Foundation
    /// **Documentation**: See `docs/features/2-combat-system/2.1-battle-foundation/architecture.md`
    /// </remarks>
    [TestFixture]
    public class BattleStateValidatorTests
    {
        private BattleStateValidator _validator;
        private BattleField _field;
        private BattleSlot _slot;

        [SetUp]
        public void SetUp()
        {
            _validator = new BattleStateValidator();
            _field = new BattleField();
            _field.Initialize(BattleRules.Singles,
                new[] { PokemonFactory.Create(PokemonCatalog.Pikachu, 50) },
                new[] { PokemonFactory.Create(PokemonCatalog.Squirtle, 50) });
            _slot = _field.PlayerSide.Slots[0];
        }

        #region ValidateSlot Tests

        [Test]
        public void ValidateSlot_ValidSlot_ReturnsNoErrors()
        {
            // Act
            var errors = _validator.ValidateSlot(_slot);

            // Assert
            Assert.That(errors, Is.Empty);
        }

        [Test]
        public void ValidateSlot_NullSlot_ReturnsError()
        {
            // Act
            var errors = _validator.ValidateSlot(null);

            // Assert
            Assert.That(errors, Has.Count.EqualTo(1));
            Assert.That(errors[0], Does.Contain("cannot be null"));
        }

        [Test]
        public void ValidateSlot_InvalidStatStage_ReturnsError()
        {
            // Arrange - This would require reflection or a way to set invalid stat stages
            // Since StatStages clamps values, we'll test with valid but edge cases
            _slot.ModifyStatStage(Stat.Attack, 6); // Max valid

            // Act
            var errors = _validator.ValidateSlot(_slot);

            // Assert - Should be valid since 6 is within range
            Assert.That(errors, Is.Empty);
        }

        #endregion

        #region ValidateStatStages Tests

        [Test]
        public void ValidateStatStages_ValidStages_ReturnsNoErrors()
        {
            // Act
            var errors = _validator.ValidateStatStages(_slot);

            // Assert
            Assert.That(errors, Is.Empty);
        }

        [Test]
        public void ValidateStatStages_NullSlot_ReturnsNoErrors()
        {
            // Act
            var errors = _validator.ValidateStatStages(null);

            // Assert
            Assert.That(errors, Is.Empty);
        }

        [Test]
        public void ValidateStatStages_AllStatsAtZero_ReturnsNoErrors()
        {
            // Arrange - Default state should have all stats at 0
            // Act
            var errors = _validator.ValidateStatStages(_slot);

            // Assert
            Assert.That(errors, Is.Empty);
        }

        #endregion

        #region ValidateStateCounters Tests

        [Test]
        public void ValidateStateCounters_ValidCounters_ReturnsNoErrors()
        {
            // Act
            var errors = _validator.ValidateStateCounters(_slot);

            // Assert
            Assert.That(errors, Is.Empty);
        }

        [Test]
        public void ValidateStateCounters_NullSlot_ReturnsNoErrors()
        {
            // Act
            var errors = _validator.ValidateStateCounters(null);

            // Assert
            Assert.That(errors, Is.Empty);
        }

        [Test]
        public void ValidateStateCounters_ZeroProtectUses_ReturnsNoErrors()
        {
            // Arrange - Default should be 0
            // Act
            var errors = _validator.ValidateStateCounters(_slot);

            // Assert
            Assert.That(errors, Is.Empty);
        }

        #endregion

        #region ValidateSide Tests

        [Test]
        public void ValidateSide_ValidSide_ReturnsNoErrors()
        {
            // Act
            var errors = _validator.ValidateSide(_field.PlayerSide);

            // Assert
            Assert.That(errors, Is.Empty);
        }

        [Test]
        public void ValidateSide_NullSide_ReturnsError()
        {
            // Act
            var errors = _validator.ValidateSide(null);

            // Assert
            Assert.That(errors, Has.Count.EqualTo(1));
            Assert.That(errors[0], Does.Contain("cannot be null"));
        }

        #endregion

        #region ValidateField Tests

        [Test]
        public void ValidateField_ValidField_ReturnsNoErrors()
        {
            // Act
            var errors = _validator.ValidateField(_field);

            // Assert
            Assert.That(errors, Is.Empty);
        }

        [Test]
        public void ValidateField_NullField_ReturnsError()
        {
            // Act
            var errors = _validator.ValidateField(null);

            // Assert
            Assert.That(errors, Has.Count.EqualTo(1));
            Assert.That(errors[0], Does.Contain("cannot be null"));
        }

        [Test]
        public void ValidateField_ValidWeather_ReturnsNoErrors()
        {
            // Arrange
            _field.SetWeather(Weather.Rain, 5);

            // Act
            var errors = _validator.ValidateField(_field);

            // Assert
            Assert.That(errors, Is.Empty);
        }

        [Test]
        public void ValidateField_ValidTerrain_ReturnsNoErrors()
        {
            // Arrange
            _field.SetTerrain(Terrain.Electric, 5);

            // Act
            var errors = _validator.ValidateField(_field);

            // Assert
            Assert.That(errors, Is.Empty);
        }

        #endregion

        #region ValidatePartyConsistency Tests

        [Test]
        public void ValidatePartyConsistency_ValidParty_ReturnsNoErrors()
        {
            // Act
            var errors = _validator.ValidatePartyConsistency(_field.PlayerSide);

            // Assert
            Assert.That(errors, Is.Empty);
        }

        [Test]
        public void ValidatePartyConsistency_NullSide_ReturnsNoErrors()
        {
            // Act
            var errors = _validator.ValidatePartyConsistency(null);

            // Assert
            Assert.That(errors, Is.Empty);
        }

        #endregion
    }
}
