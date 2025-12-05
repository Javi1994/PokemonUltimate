using System;
using NUnit.Framework;
using PokemonUltimate.Combat.ValueObjects;

namespace PokemonUltimate.Tests.Systems.Combat.ValueObjects
{
    /// <summary>
    /// Tests for ChargingMoveState - Value Object representing charging move state.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.1: Battle Foundation
    /// **Documentation**: See `docs/features/2-combat-system/2.1-battle-foundation/architecture.md`
    /// </remarks>
    [TestFixture]
    public class ChargingMoveStateTests
    {
        #region Constructor Tests

        [Test]
        public void Constructor_Default_NoActiveMove()
        {
            // Act
            var state = new ChargingMoveState();

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(state.MoveName, Is.Null);
                Assert.That(state.IsActive, Is.False);
            });
        }

        #endregion

        #region SetMove Tests

        [Test]
        public void SetMove_WithMoveName_SetsMove()
        {
            // Arrange
            var state = new ChargingMoveState();
            const string moveName = "Solar Beam";

            // Act
            var newState = state.SetMove(moveName);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(newState.MoveName, Is.EqualTo(moveName));
                Assert.That(newState.IsActive, Is.True);
            });
        }

        [Test]
        public void SetMove_NullMoveName_ThrowsArgumentException()
        {
            // Arrange
            var state = new ChargingMoveState();

            // Act & Assert
            Assert.Throws<ArgumentException>(() => state.SetMove(null));
        }

        [Test]
        public void SetMove_EmptyMoveName_ThrowsArgumentException()
        {
            // Arrange
            var state = new ChargingMoveState();

            // Act & Assert
            Assert.Throws<ArgumentException>(() => state.SetMove(string.Empty));
        }

        [Test]
        public void SetMove_WhitespaceMoveName_ThrowsArgumentException()
        {
            // Arrange
            var state = new ChargingMoveState();

            // Act & Assert
            Assert.Throws<ArgumentException>(() => state.SetMove("   "));
        }

        [Test]
        public void SetMove_Immutable_OriginalUnchanged()
        {
            // Arrange
            var original = new ChargingMoveState();

            // Act
            var modified = original.SetMove("Solar Beam");

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(original.MoveName, Is.Null);
                Assert.That(modified.MoveName, Is.EqualTo("Solar Beam"));
            });
        }

        #endregion

        #region Clear Tests

        [Test]
        public void Clear_WithActiveMove_ClearsMove()
        {
            // Arrange
            var state = new ChargingMoveState();
            state = state.SetMove("Solar Beam");

            // Act
            var cleared = state.Clear();

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(cleared.MoveName, Is.Null);
                Assert.That(cleared.IsActive, Is.False);
            });
        }

        [Test]
        public void Clear_AlreadyCleared_RemainsCleared()
        {
            // Arrange
            var state = new ChargingMoveState();

            // Act
            var cleared = state.Clear();

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(cleared.MoveName, Is.Null);
                Assert.That(cleared.IsActive, Is.False);
            });
        }

        [Test]
        public void Clear_Immutable_OriginalUnchanged()
        {
            // Arrange
            var original = new ChargingMoveState();
            original = original.SetMove("Solar Beam");

            // Act
            var cleared = original.Clear();

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(original.MoveName, Is.EqualTo("Solar Beam"));
                Assert.That(cleared.MoveName, Is.Null);
            });
        }

        #endregion
    }
}
