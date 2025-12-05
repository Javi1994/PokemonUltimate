using NUnit.Framework;
using PokemonUltimate.Combat.ValueObjects;

namespace PokemonUltimate.Tests.Systems.Combat.ValueObjects
{
    /// <summary>
    /// Tests for MoveStateTracker - Value Object tracking all move-related states.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.1: Battle Foundation
    /// **Documentation**: See `docs/features/2-combat-system/2.1-battle-foundation/architecture.md`
    /// </remarks>
    [TestFixture]
    public class MoveStateTrackerTests
    {
        #region Constructor Tests

        [Test]
        public void Constructor_Default_AllStatesCleared()
        {
            // Act
            var tracker = new MoveStateTracker();

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(tracker.SemiInvulnerableState, Is.Not.Null);
                Assert.That(tracker.SemiInvulnerableState.IsActive, Is.False);
                Assert.That(tracker.ChargingMoveState, Is.Not.Null);
                Assert.That(tracker.ChargingMoveState.IsActive, Is.False);
            });
        }

        #endregion

        #region WithSemiInvulnerableState Tests

        [Test]
        public void WithSemiInvulnerableState_UpdatesSemiInvulnerableState()
        {
            // Arrange
            var tracker = new MoveStateTracker();
            var newSemiState = new SemiInvulnerableState().SetMove("Dig");

            // Act
            var updated = tracker.WithSemiInvulnerableState(newSemiState);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(updated.SemiInvulnerableState.MoveName, Is.EqualTo("Dig"));
                Assert.That(updated.SemiInvulnerableState.IsActive, Is.True);
                Assert.That(updated.ChargingMoveState.IsActive, Is.False);
            });
        }

        [Test]
        public void WithSemiInvulnerableState_Null_UsesDefault()
        {
            // Arrange
            var tracker = new MoveStateTracker();
            var originalSemiState = tracker.SemiInvulnerableState;

            // Act
            var updated = tracker.WithSemiInvulnerableState(null);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(updated.SemiInvulnerableState, Is.Not.Null);
                Assert.That(updated.SemiInvulnerableState.IsActive, Is.False);
            });
        }

        [Test]
        public void WithSemiInvulnerableState_Immutable_OriginalUnchanged()
        {
            // Arrange
            var original = new MoveStateTracker();
            var newSemiState = new SemiInvulnerableState().SetMove("Dig");

            // Act
            var updated = original.WithSemiInvulnerableState(newSemiState);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(original.SemiInvulnerableState.IsActive, Is.False);
                Assert.That(updated.SemiInvulnerableState.IsActive, Is.True);
            });
        }

        #endregion

        #region WithChargingMoveState Tests

        [Test]
        public void WithChargingMoveState_UpdatesChargingMoveState()
        {
            // Arrange
            var tracker = new MoveStateTracker();
            var newChargingState = new ChargingMoveState().SetMove("Solar Beam");

            // Act
            var updated = tracker.WithChargingMoveState(newChargingState);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(updated.ChargingMoveState.MoveName, Is.EqualTo("Solar Beam"));
                Assert.That(updated.ChargingMoveState.IsActive, Is.True);
                Assert.That(updated.SemiInvulnerableState.IsActive, Is.False);
            });
        }

        [Test]
        public void WithChargingMoveState_Null_UsesDefault()
        {
            // Arrange
            var tracker = new MoveStateTracker();

            // Act
            var updated = tracker.WithChargingMoveState(null);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(updated.ChargingMoveState, Is.Not.Null);
                Assert.That(updated.ChargingMoveState.IsActive, Is.False);
            });
        }

        [Test]
        public void WithChargingMoveState_Immutable_OriginalUnchanged()
        {
            // Arrange
            var original = new MoveStateTracker();
            var newChargingState = new ChargingMoveState().SetMove("Solar Beam");

            // Act
            var updated = original.WithChargingMoveState(newChargingState);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(original.ChargingMoveState.IsActive, Is.False);
                Assert.That(updated.ChargingMoveState.IsActive, Is.True);
            });
        }

        #endregion

        #region Combined Operations Tests

        [Test]
        public void MultipleOperations_BothStatesSet_Correctly()
        {
            // Arrange
            var tracker = new MoveStateTracker();
            var semiState = new SemiInvulnerableState().SetMove("Dig");
            var chargingState = new ChargingMoveState().SetMove("Solar Beam");

            // Act
            var updated = tracker
                .WithSemiInvulnerableState(semiState)
                .WithChargingMoveState(chargingState);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(updated.SemiInvulnerableState.MoveName, Is.EqualTo("Dig"));
                Assert.That(updated.ChargingMoveState.MoveName, Is.EqualTo("Solar Beam"));
            });
        }

        #endregion

        #region Reset Tests

        [Test]
        public void Reset_WithActiveStates_ResetsAll()
        {
            // Arrange
            var tracker = new MoveStateTracker();
            var semiState = new SemiInvulnerableState().SetMove("Dig");
            var chargingState = new ChargingMoveState().SetMove("Solar Beam");
            tracker = tracker.WithSemiInvulnerableState(semiState).WithChargingMoveState(chargingState);

            // Act
            var reset = tracker.Reset();

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(reset.SemiInvulnerableState.IsActive, Is.False);
                Assert.That(reset.ChargingMoveState.IsActive, Is.False);
            });
        }

        [Test]
        public void Reset_Immutable_OriginalUnchanged()
        {
            // Arrange
            var original = new MoveStateTracker();
            var semiState = new SemiInvulnerableState().SetMove("Dig");
            original = original.WithSemiInvulnerableState(semiState);

            // Act
            var reset = original.Reset();

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(original.SemiInvulnerableState.IsActive, Is.True);
                Assert.That(reset.SemiInvulnerableState.IsActive, Is.False);
            });
        }

        #endregion
    }
}
