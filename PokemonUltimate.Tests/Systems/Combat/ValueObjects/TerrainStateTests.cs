using NUnit.Framework;
using PokemonUltimate.Combat.ValueObjects;
using PokemonUltimate.Core.Enums;

namespace PokemonUltimate.Tests.Systems.Combat.ValueObjects
{
    /// <summary>
    /// Tests for TerrainState - Value Object representing terrain state on the battlefield.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.13: Terrain System
    /// **Documentation**: See `docs/features/2-combat-system/2.13-terrain-system/README.md`
    /// </remarks>
    [TestFixture]
    public class TerrainStateTests
    {
        #region Constructor Tests

        [Test]
        public void Constructor_Default_NoTerrainActive()
        {
            // Act
            var state = new TerrainState();

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(state.Terrain, Is.EqualTo(Terrain.None));
                Assert.That(state.Duration, Is.EqualTo(0));
                Assert.That(state.TerrainData, Is.Null);
                Assert.That(state.IsActive, Is.False);
                Assert.That(state.IsInfinite, Is.False);
            });
        }

        #endregion

        #region SetTerrain Tests

        [Test]
        public void SetTerrain_ElectricWithDuration_SetsTerrain()
        {
            // Arrange
            var state = new TerrainState();
            const int duration = 5;

            // Act
            var newState = state.SetTerrain(Terrain.Electric, duration);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(newState.Terrain, Is.EqualTo(Terrain.Electric));
                Assert.That(newState.Duration, Is.EqualTo(duration));
                Assert.That(newState.IsActive, Is.True);
                Assert.That(newState.IsInfinite, Is.False);
            });
        }

        [Test]
        public void SetTerrain_None_ClearsTerrain()
        {
            // Arrange
            var state = new TerrainState();
            state = state.SetTerrain(Terrain.Electric, 5);

            // Act
            var cleared = state.SetTerrain(Terrain.None, 0);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(cleared.Terrain, Is.EqualTo(Terrain.None));
                Assert.That(cleared.Duration, Is.EqualTo(0));
                Assert.That(cleared.IsActive, Is.False);
            });
        }

        [Test]
        public void SetTerrain_ZeroDuration_SetsInfinite()
        {
            // Arrange
            var state = new TerrainState();

            // Act
            var newState = state.SetTerrain(Terrain.Grassy, 0);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(newState.Terrain, Is.EqualTo(Terrain.Grassy));
                Assert.That(newState.Duration, Is.EqualTo(0));
                Assert.That(newState.IsActive, Is.True);
                Assert.That(newState.IsInfinite, Is.True);
            });
        }

        [Test]
        public void SetTerrain_WithTerrainData_SetsTerrainData()
        {
            // Arrange
            var state = new TerrainState();
            // TerrainData can be null for testing purposes
            var terrainData = (PokemonUltimate.Core.Blueprints.TerrainData)null;

            // Act
            var newState = state.SetTerrain(Terrain.Electric, 5, terrainData);

            // Assert
            Assert.That(newState.TerrainData, Is.EqualTo(terrainData));
        }

        [Test]
        public void SetTerrain_Immutable_OriginalUnchanged()
        {
            // Arrange
            var original = new TerrainState();

            // Act
            var modified = original.SetTerrain(Terrain.Electric, 5);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(original.Terrain, Is.EqualTo(Terrain.None));
                Assert.That(modified.Terrain, Is.EqualTo(Terrain.Electric));
            });
        }

        #endregion

        #region Clear Tests

        [Test]
        public void Clear_WithActiveTerrain_ClearsTerrain()
        {
            // Arrange
            var state = new TerrainState();
            state = state.SetTerrain(Terrain.Electric, 5);

            // Act
            var cleared = state.Clear();

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(cleared.Terrain, Is.EqualTo(Terrain.None));
                Assert.That(cleared.Duration, Is.EqualTo(0));
                Assert.That(cleared.IsActive, Is.False);
            });
        }

        [Test]
        public void Clear_AlreadyCleared_RemainsCleared()
        {
            // Arrange
            var state = new TerrainState();

            // Act
            var cleared = state.Clear();

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(cleared.Terrain, Is.EqualTo(Terrain.None));
                Assert.That(cleared.IsActive, Is.False);
            });
        }

        [Test]
        public void Clear_Immutable_OriginalUnchanged()
        {
            // Arrange
            var original = new TerrainState();
            original = original.SetTerrain(Terrain.Electric, 5);

            // Act
            var cleared = original.Clear();

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(original.Terrain, Is.EqualTo(Terrain.Electric));
                Assert.That(cleared.Terrain, Is.EqualTo(Terrain.None));
            });
        }

        #endregion

        #region DecrementDuration Tests

        [Test]
        public void DecrementDuration_WithDuration_Decrements()
        {
            // Arrange
            var state = new TerrainState();
            state = state.SetTerrain(Terrain.Electric, 5);

            // Act
            var decremented = state.DecrementDuration();

            // Assert
            Assert.That(decremented.Duration, Is.EqualTo(4));
        }

        [Test]
        public void DecrementDuration_DurationReachesZero_ClearsTerrain()
        {
            // Arrange
            var state = new TerrainState();
            state = state.SetTerrain(Terrain.Electric, 1);

            // Act
            var decremented = state.DecrementDuration();

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(decremented.Terrain, Is.EqualTo(Terrain.None));
                Assert.That(decremented.IsActive, Is.False);
            });
        }

        [Test]
        public void DecrementDuration_InfiniteDuration_NoChange()
        {
            // Arrange
            var state = new TerrainState();
            state = state.SetTerrain(Terrain.Grassy, 0); // Infinite duration

            // Act
            var decremented = state.DecrementDuration();

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(decremented.Terrain, Is.EqualTo(Terrain.Grassy));
                Assert.That(decremented.Duration, Is.EqualTo(0));
                Assert.That(decremented.IsInfinite, Is.True);
            });
        }

        [Test]
        public void DecrementDuration_NoTerrain_NoChange()
        {
            // Arrange
            var state = new TerrainState();

            // Act
            var decremented = state.DecrementDuration();

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(decremented.Terrain, Is.EqualTo(Terrain.None));
                Assert.That(decremented.IsActive, Is.False);
            });
        }

        [Test]
        public void DecrementDuration_Immutable_OriginalUnchanged()
        {
            // Arrange
            var original = new TerrainState();
            original = original.SetTerrain(Terrain.Electric, 5);

            // Act
            var decremented = original.DecrementDuration();

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(original.Duration, Is.EqualTo(5));
                Assert.That(decremented.Duration, Is.EqualTo(4));
            });
        }

        #endregion
    }
}
