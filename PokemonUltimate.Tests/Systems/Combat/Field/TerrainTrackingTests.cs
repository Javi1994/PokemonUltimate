using System.Collections.Generic;
using NUnit.Framework;
using PokemonUltimate.Combat;
using PokemonUltimate.Content.Catalogs.Pokemon;
using PokemonUltimate.Content.Catalogs.Terrain;
using PokemonUltimate.Core.Enums;
using PokemonUltimate.Core.Factories;
using PokemonUltimate.Core.Instances;

namespace PokemonUltimate.Tests.Systems.Combat.Field
{
    /// <summary>
    /// Tests for Terrain Tracking in BattleField.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.13: Terrain System
    /// **Documentation**: See `docs/features/2-combat-system/2.13-terrain-system/README.md`
    /// </remarks>
    [TestFixture]
    public class TerrainTrackingTests
    {
        private BattleField _field;
        private List<PokemonInstance> _playerParty;
        private List<PokemonInstance> _enemyParty;
        private BattleRules _rules;

        [SetUp]
        public void SetUp()
        {
            _field = new BattleField();
            _rules = new BattleRules { PlayerSlots = 1, EnemySlots = 1 };

            _playerParty = new List<PokemonInstance>
            {
                PokemonFactory.Create(PokemonCatalog.Pikachu, 25)
            };

            _enemyParty = new List<PokemonInstance>
            {
                PokemonFactory.Create(PokemonCatalog.Squirtle, 25)
            };

            _field.Initialize(_rules, _playerParty, _enemyParty);
        }

        #region SetTerrain Tests

        [Test]
        public void SetTerrain_Electric_SetsCorrectly()
        {
            // Arrange
            var terrainData = TerrainCatalog.GetByTerrain(Terrain.Electric);

            // Act
            _field.SetTerrain(Terrain.Electric, 5, terrainData);

            // Assert
            Assert.That(_field.Terrain, Is.EqualTo(Terrain.Electric));
            Assert.That(_field.TerrainDuration, Is.EqualTo(5));
            Assert.That(_field.TerrainData, Is.EqualTo(terrainData));
        }

        [Test]
        public void SetTerrain_Grassy_SetsCorrectly()
        {
            // Arrange
            var terrainData = TerrainCatalog.GetByTerrain(Terrain.Grassy);

            // Act
            _field.SetTerrain(Terrain.Grassy, 5, terrainData);

            // Assert
            Assert.That(_field.Terrain, Is.EqualTo(Terrain.Grassy));
            Assert.That(_field.TerrainDuration, Is.EqualTo(5));
            Assert.That(_field.TerrainData, Is.EqualTo(terrainData));
        }

        [Test]
        public void SetTerrain_Psychic_SetsCorrectly()
        {
            // Arrange
            var terrainData = TerrainCatalog.GetByTerrain(Terrain.Psychic);

            // Act
            _field.SetTerrain(Terrain.Psychic, 5, terrainData);

            // Assert
            Assert.That(_field.Terrain, Is.EqualTo(Terrain.Psychic));
            Assert.That(_field.TerrainDuration, Is.EqualTo(5));
            Assert.That(_field.TerrainData, Is.EqualTo(terrainData));
        }

        [Test]
        public void SetTerrain_Misty_SetsCorrectly()
        {
            // Arrange
            var terrainData = TerrainCatalog.GetByTerrain(Terrain.Misty);

            // Act
            _field.SetTerrain(Terrain.Misty, 5, terrainData);

            // Assert
            Assert.That(_field.Terrain, Is.EqualTo(Terrain.Misty));
            Assert.That(_field.TerrainDuration, Is.EqualTo(5));
            Assert.That(_field.TerrainData, Is.EqualTo(terrainData));
        }

        [Test]
        public void SetTerrain_None_ClearsTerrain()
        {
            // Arrange
            var terrainData = TerrainCatalog.GetByTerrain(Terrain.Electric);
            _field.SetTerrain(Terrain.Electric, 5, terrainData);

            // Act
            _field.SetTerrain(Terrain.None, 0, null);

            // Assert
            Assert.That(_field.Terrain, Is.EqualTo(Terrain.None));
            Assert.That(_field.TerrainDuration, Is.EqualTo(0));
            Assert.That(_field.TerrainData, Is.Null);
        }

        [Test]
        public void ClearTerrain_RemovesTerrain()
        {
            // Arrange
            var terrainData = TerrainCatalog.GetByTerrain(Terrain.Grassy);
            _field.SetTerrain(Terrain.Grassy, 5, terrainData);

            // Act
            _field.ClearTerrain();

            // Assert
            Assert.That(_field.Terrain, Is.EqualTo(Terrain.None));
            Assert.That(_field.TerrainDuration, Is.EqualTo(0));
            Assert.That(_field.TerrainData, Is.Null);
        }

        #endregion

        #region Terrain Duration Tests

        [Test]
        public void TerrainDuration_DecrementsEachTurn()
        {
            // Arrange
            var terrainData = TerrainCatalog.GetByTerrain(Terrain.Electric);
            _field.SetTerrain(Terrain.Electric, 5, terrainData);

            // Act & Assert
            Assert.That(_field.TerrainDuration, Is.EqualTo(5));

            _field.DecrementTerrainDuration();
            Assert.That(_field.TerrainDuration, Is.EqualTo(4));

            _field.DecrementTerrainDuration();
            Assert.That(_field.TerrainDuration, Is.EqualTo(3));
        }

        [Test]
        public void TerrainDuration_Expires_RemovesTerrain()
        {
            // Arrange
            var terrainData = TerrainCatalog.GetByTerrain(Terrain.Grassy);
            _field.SetTerrain(Terrain.Grassy, 2, terrainData);

            // Act
            _field.DecrementTerrainDuration();
            _field.DecrementTerrainDuration();

            // Assert
            Assert.That(_field.Terrain, Is.EqualTo(Terrain.None));
            Assert.That(_field.TerrainDuration, Is.EqualTo(0));
            Assert.That(_field.TerrainData, Is.Null);
        }

        [Test]
        public void TerrainDuration_Infinite_DoesNotDecrement()
        {
            // Arrange
            var terrainData = TerrainCatalog.GetByTerrain(Terrain.Psychic);
            _field.SetTerrain(Terrain.Psychic, 0, terrainData); // 0 = infinite

            // Act
            _field.DecrementTerrainDuration();
            _field.DecrementTerrainDuration();

            // Assert
            Assert.That(_field.Terrain, Is.EqualTo(Terrain.Psychic));
            Assert.That(_field.TerrainDuration, Is.EqualTo(0)); // Still 0 (infinite)
            Assert.That(_field.TerrainData, Is.EqualTo(terrainData));
        }

        [Test]
        public void TerrainDuration_NoTerrain_DoesNotDecrement()
        {
            // Arrange - No terrain set

            // Act
            _field.DecrementTerrainDuration();

            // Assert
            Assert.That(_field.Terrain, Is.EqualTo(Terrain.None));
            Assert.That(_field.TerrainDuration, Is.EqualTo(0));
        }

        #endregion

        #region Initialization Tests

        [Test]
        public void Initialize_NoTerrain_DefaultsToNone()
        {
            // Act - Already done in SetUp

            // Assert
            Assert.That(_field.Terrain, Is.EqualTo(Terrain.None));
            Assert.That(_field.TerrainDuration, Is.EqualTo(0));
            Assert.That(_field.TerrainData, Is.Null);
        }

        #endregion
    }
}

