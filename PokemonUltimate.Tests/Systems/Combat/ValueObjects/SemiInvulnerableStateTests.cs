using System;
using NUnit.Framework;
using PokemonUltimate.Combat.ValueObjects;

namespace PokemonUltimate.Tests.Systems.Combat.ValueObjects
{
    /// <summary>
    /// Tests for SemiInvulnerableState - Value Object representing semi-invulnerable move state.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.1: Battle Foundation
    /// **Documentation**: See `docs/features/2-combat-system/2.1-battle-foundation/architecture.md`
    /// </remarks>
    [TestFixture]
    public class SemiInvulnerableStateTests
    {
        #region Constructor Tests

        [Test]
        public void Constructor_Default_NoActiveMove()
        {
            // Act
            var state = new SemiInvulnerableState();

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(state.MoveName, Is.Null);
                Assert.That(state.IsCharging, Is.False);
                Assert.That(state.IsActive, Is.False);
            });
        }

        #endregion

        #region SetMove Tests

        [Test]
        public void SetMove_WithMoveName_SetsMove()
        {
            // Arrange
            var state = new SemiInvulnerableState();
            const string moveName = "Dig";

            // Act
            var newState = state.SetMove(moveName);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(newState.MoveName, Is.EqualTo(moveName));
                Assert.That(newState.IsCharging, Is.True); // Default is true
                Assert.That(newState.IsActive, Is.True);
            });
        }

        [Test]
        public void SetMove_WithIsChargingFalse_SetsNotCharging()
        {
            // Arrange
            var state = new SemiInvulnerableState();
            const string moveName = "Dig";

            // Act
            var newState = state.SetMove(moveName, isCharging: false);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(newState.MoveName, Is.EqualTo(moveName));
                Assert.That(newState.IsCharging, Is.False);
                Assert.That(newState.IsActive, Is.True);
            });
        }

        [Test]
        public void SetMove_NullMoveName_ThrowsArgumentException()
        {
            // Arrange
            var state = new SemiInvulnerableState();

            // Act & Assert
            Assert.Throws<ArgumentException>(() => state.SetMove(null));
        }

        [Test]
        public void SetMove_EmptyMoveName_ThrowsArgumentException()
        {
            // Arrange
            var state = new SemiInvulnerableState();

            // Act & Assert
            Assert.Throws<ArgumentException>(() => state.SetMove(string.Empty));
        }

        [Test]
        public void SetMove_WhitespaceMoveName_ThrowsArgumentException()
        {
            // Arrange
            var state = new SemiInvulnerableState();

            // Act & Assert
            Assert.Throws<ArgumentException>(() => state.SetMove("   "));
        }

        [Test]
        public void SetMove_Immutable_OriginalUnchanged()
        {
            // Arrange
            var original = new SemiInvulnerableState();

            // Act
            var modified = original.SetMove("Dig");

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(original.MoveName, Is.Null);
                Assert.That(modified.MoveName, Is.EqualTo("Dig"));
            });
        }

        #endregion

        #region Clear Tests

        [Test]
        public void Clear_WithActiveMove_ClearsMove()
        {
            // Arrange
            var state = new SemiInvulnerableState();
            state = state.SetMove("Dig");

            // Act
            var cleared = state.Clear();

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(cleared.MoveName, Is.Null);
                Assert.That(cleared.IsCharging, Is.False);
                Assert.That(cleared.IsActive, Is.False);
            });
        }

        [Test]
        public void Clear_AlreadyCleared_RemainsCleared()
        {
            // Arrange
            var state = new SemiInvulnerableState();

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
            var original = new SemiInvulnerableState();
            original = original.SetMove("Dig");

            // Act
            var cleared = original.Clear();

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(original.MoveName, Is.EqualTo("Dig"));
                Assert.That(cleared.MoveName, Is.Null);
            });
        }

        #endregion

        #region SetReady Tests

        [Test]
        public void SetReady_WithChargingMove_SetsNotCharging()
        {
            // Arrange
            var state = new SemiInvulnerableState();
            state = state.SetMove("Dig", isCharging: true);

            // Act
            var ready = state.SetReady();

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(ready.MoveName, Is.EqualTo("Dig"));
                Assert.That(ready.IsCharging, Is.False);
                Assert.That(ready.IsActive, Is.True);
            });
        }

        [Test]
        public void SetReady_NoActiveMove_NoChange()
        {
            // Arrange
            var state = new SemiInvulnerableState();

            // Act
            var ready = state.SetReady();

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(ready.MoveName, Is.Null);
                Assert.That(ready.IsActive, Is.False);
            });
        }

        [Test]
        public void SetReady_AlreadyReady_RemainsReady()
        {
            // Arrange
            var state = new SemiInvulnerableState();
            state = state.SetMove("Dig", isCharging: false);

            // Act
            var ready = state.SetReady();

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(ready.MoveName, Is.EqualTo("Dig"));
                Assert.That(ready.IsCharging, Is.False);
            });
        }

        [Test]
        public void SetReady_Immutable_OriginalUnchanged()
        {
            // Arrange
            var original = new SemiInvulnerableState();
            original = original.SetMove("Dig", isCharging: true);

            // Act
            var ready = original.SetReady();

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(original.IsCharging, Is.True);
                Assert.That(ready.IsCharging, Is.False);
            });
        }

        #endregion
    }
}
