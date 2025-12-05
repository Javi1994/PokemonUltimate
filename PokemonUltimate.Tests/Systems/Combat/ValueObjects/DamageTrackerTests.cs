using NUnit.Framework;
using PokemonUltimate.Combat.ValueObjects;

namespace PokemonUltimate.Tests.Systems.Combat.ValueObjects
{
    /// <summary>
    /// Tests for DamageTracker - Value Object tracking damage taken during a turn.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.1: Battle Foundation
    /// **Documentation**: See `docs/features/2-combat-system/2.1-battle-foundation/architecture.md`
    /// </remarks>
    [TestFixture]
    public class DamageTrackerTests
    {
        #region Constructor Tests

        [Test]
        public void Constructor_Default_AllValuesZero()
        {
            // Act
            var tracker = new DamageTracker();

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(tracker.PhysicalDamage, Is.EqualTo(0));
                Assert.That(tracker.SpecialDamage, Is.EqualTo(0));
                Assert.That(tracker.WasHitWhileFocusing, Is.False);
            });
        }

        #endregion

        #region AddPhysicalDamage Tests

        [Test]
        public void AddPhysicalDamage_PositiveDamage_IncreasesPhysicalDamage()
        {
            // Arrange
            var tracker = new DamageTracker();

            // Act
            var newTracker = tracker.AddPhysicalDamage(10);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(newTracker.PhysicalDamage, Is.EqualTo(10));
                Assert.That(newTracker.SpecialDamage, Is.EqualTo(0));
                Assert.That(newTracker.WasHitWhileFocusing, Is.False);
            });
        }

        [Test]
        public void AddPhysicalDamage_MultipleTimes_Accumulates()
        {
            // Arrange
            var tracker = new DamageTracker();

            // Act
            var newTracker = tracker.AddPhysicalDamage(10);
            newTracker = newTracker.AddPhysicalDamage(5);

            // Assert
            Assert.That(newTracker.PhysicalDamage, Is.EqualTo(15));
        }

        [Test]
        public void AddPhysicalDamage_Immutable_OriginalUnchanged()
        {
            // Arrange
            var original = new DamageTracker();

            // Act
            var modified = original.AddPhysicalDamage(10);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(original.PhysicalDamage, Is.EqualTo(0));
                Assert.That(modified.PhysicalDamage, Is.EqualTo(10));
            });
        }

        #endregion

        #region AddSpecialDamage Tests

        [Test]
        public void AddSpecialDamage_PositiveDamage_IncreasesSpecialDamage()
        {
            // Arrange
            var tracker = new DamageTracker();

            // Act
            var newTracker = tracker.AddSpecialDamage(15);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(newTracker.PhysicalDamage, Is.EqualTo(0));
                Assert.That(newTracker.SpecialDamage, Is.EqualTo(15));
                Assert.That(newTracker.WasHitWhileFocusing, Is.False);
            });
        }

        [Test]
        public void AddSpecialDamage_MultipleTimes_Accumulates()
        {
            // Arrange
            var tracker = new DamageTracker();

            // Act
            var newTracker = tracker.AddSpecialDamage(10);
            newTracker = newTracker.AddSpecialDamage(8);

            // Assert
            Assert.That(newTracker.SpecialDamage, Is.EqualTo(18));
        }

        [Test]
        public void AddSpecialDamage_Immutable_OriginalUnchanged()
        {
            // Arrange
            var original = new DamageTracker();

            // Act
            var modified = original.AddSpecialDamage(15);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(original.SpecialDamage, Is.EqualTo(0));
                Assert.That(modified.SpecialDamage, Is.EqualTo(15));
            });
        }

        #endregion

        #region SetHitWhileFocusing Tests

        [Test]
        public void SetHitWhileFocusing_True_SetsFlag()
        {
            // Arrange
            var tracker = new DamageTracker();

            // Act
            var newTracker = tracker.SetHitWhileFocusing(true);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(newTracker.WasHitWhileFocusing, Is.True);
                Assert.That(newTracker.PhysicalDamage, Is.EqualTo(0));
                Assert.That(newTracker.SpecialDamage, Is.EqualTo(0));
            });
        }

        [Test]
        public void SetHitWhileFocusing_False_ClearsFlag()
        {
            // Arrange
            var tracker = new DamageTracker();
            tracker = tracker.SetHitWhileFocusing(true);

            // Act
            var newTracker = tracker.SetHitWhileFocusing(false);

            // Assert
            Assert.That(newTracker.WasHitWhileFocusing, Is.False);
        }

        [Test]
        public void SetHitWhileFocusing_Immutable_OriginalUnchanged()
        {
            // Arrange
            var original = new DamageTracker();

            // Act
            var modified = original.SetHitWhileFocusing(true);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(original.WasHitWhileFocusing, Is.False);
                Assert.That(modified.WasHitWhileFocusing, Is.True);
            });
        }

        #endregion

        #region Combined Operations Tests

        [Test]
        public void MultipleOperations_AllValuesSet_Correctly()
        {
            // Arrange
            var tracker = new DamageTracker();

            // Act
            var newTracker = tracker
                .AddPhysicalDamage(10)
                .AddSpecialDamage(15)
                .SetHitWhileFocusing(true);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(newTracker.PhysicalDamage, Is.EqualTo(10));
                Assert.That(newTracker.SpecialDamage, Is.EqualTo(15));
                Assert.That(newTracker.WasHitWhileFocusing, Is.True);
            });
        }

        #endregion

        #region Reset Tests

        [Test]
        public void Reset_WithValues_ResetsAllToZero()
        {
            // Arrange
            var tracker = new DamageTracker();
            tracker = tracker.AddPhysicalDamage(10).AddSpecialDamage(15).SetHitWhileFocusing(true);

            // Act
            var reset = tracker.Reset();

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(reset.PhysicalDamage, Is.EqualTo(0));
                Assert.That(reset.SpecialDamage, Is.EqualTo(0));
                Assert.That(reset.WasHitWhileFocusing, Is.False);
            });
        }

        [Test]
        public void Reset_Immutable_OriginalUnchanged()
        {
            // Arrange
            var original = new DamageTracker();
            original = original.AddPhysicalDamage(10);

            // Act
            var reset = original.Reset();

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(original.PhysicalDamage, Is.EqualTo(10));
                Assert.That(reset.PhysicalDamage, Is.EqualTo(0));
            });
        }

        #endregion
    }
}
