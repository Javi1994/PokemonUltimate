using System;
using NUnit.Framework;
using PokemonUltimate.Combat.ValueObjects;

namespace PokemonUltimate.Tests.Systems.Combat.ValueObjects
{
    /// <summary>
    /// Tests for ProtectTracker - Value Object tracking consecutive Protect uses.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.1: Battle Foundation
    /// **Documentation**: See `docs/features/2-combat-system/2.1-battle-foundation/architecture.md`
    /// </remarks>
    [TestFixture]
    public class ProtectTrackerTests
    {
        #region Constructor Tests

        [Test]
        public void Constructor_Default_ZeroConsecutiveUses()
        {
            // Act
            var tracker = new ProtectTracker();

            // Assert
            Assert.That(tracker.ConsecutiveUses, Is.EqualTo(0));
        }

        #endregion

        #region Increment Tests

        [Test]
        public void Increment_FromZero_IncreasesToOne()
        {
            // Arrange
            var tracker = new ProtectTracker();

            // Act
            var newTracker = tracker.Increment();

            // Assert
            Assert.That(newTracker.ConsecutiveUses, Is.EqualTo(1));
        }

        [Test]
        public void Increment_MultipleTimes_Accumulates()
        {
            // Arrange
            var tracker = new ProtectTracker();

            // Act
            var newTracker = tracker.Increment();
            newTracker = newTracker.Increment();
            newTracker = newTracker.Increment();

            // Assert
            Assert.That(newTracker.ConsecutiveUses, Is.EqualTo(3));
        }

        [Test]
        public void Increment_Immutable_OriginalUnchanged()
        {
            // Arrange
            var original = new ProtectTracker();

            // Act
            var incremented = original.Increment();

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(original.ConsecutiveUses, Is.EqualTo(0));
                Assert.That(incremented.ConsecutiveUses, Is.EqualTo(1));
            });
        }

        #endregion

        #region Reset Tests

        [Test]
        public void Reset_FromZero_RemainsZero()
        {
            // Arrange
            var tracker = new ProtectTracker();

            // Act
            var reset = tracker.Reset();

            // Assert
            Assert.That(reset.ConsecutiveUses, Is.EqualTo(0));
        }

        [Test]
        public void Reset_WithUses_ResetsToZero()
        {
            // Arrange
            var tracker = new ProtectTracker();
            tracker = tracker.Increment().Increment().Increment();

            // Act
            var reset = tracker.Reset();

            // Assert
            Assert.That(reset.ConsecutiveUses, Is.EqualTo(0));
        }

        [Test]
        public void Reset_Immutable_OriginalUnchanged()
        {
            // Arrange
            var original = new ProtectTracker();
            original = original.Increment().Increment();

            // Act
            var reset = original.Reset();

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(original.ConsecutiveUses, Is.EqualTo(2));
                Assert.That(reset.ConsecutiveUses, Is.EqualTo(0));
            });
        }

        #endregion
    }
}
