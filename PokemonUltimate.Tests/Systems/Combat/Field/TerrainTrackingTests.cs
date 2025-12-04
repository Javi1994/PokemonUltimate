using System.Collections.Generic;
using NUnit.Framework;
using PokemonUltimate.Combat;
using PokemonUltimate.Core.Enums;
using PokemonUltimate.Core.Factories;
using PokemonUltimate.Core.Instances;
using PokemonUltimate.Content.Catalogs.Pokemon;
using PokemonUltimate.Content.Catalogs.Terrain;

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
            Assert.AreEqual(Terrain.Electric, _field.Terrain);
            Assert.AreEqual(5, _field.TerrainDuration);
            Assert.AreEqual(terrainData, _field.TerrainData);
        }

        [Test]
        public void SetTerrain_Grassy_SetsCorrectly()
        {
            // Arrange
            var terrainData = TerrainCatalog.GetByTerrain(Terrain.Grassy);

            // Act
            _field.SetTerrain(Terrain.Grassy, 5, terrainData);

            // Assert
            Assert.AreEqual(Terrain.Grassy, _field.Terrain);
            Assert.AreEqual(5, _field.TerrainDuration);
            Assert.AreEqual(terrainData, _field.TerrainData);
        }

        [Test]
        public void SetTerrain_Psychic_SetsCorrectly()
        {
            // Arrange
            var terrainData = TerrainCatalog.GetByTerrain(Terrain.Psychic);

            // Act
            _field.SetTerrain(Terrain.Psychic, 5, terrainData);

            // Assert
            Assert.AreEqual(Terrain.Psychic, _field.Terrain);
            Assert.AreEqual(5, _field.TerrainDuration);
            Assert.AreEqual(terrainData, _field.TerrainData);
        }

        [Test]
        public void SetTerrain_Misty_SetsCorrectly()
        {
            // Arrange
            var terrainData = TerrainCatalog.GetByTerrain(Terrain.Misty);

            // Act
            _field.SetTerrain(Terrain.Misty, 5, terrainData);

            // Assert
            Assert.AreEqual(Terrain.Misty, _field.Terrain);
            Assert.AreEqual(5, _field.TerrainDuration);
            Assert.AreEqual(terrainData, _field.TerrainData);
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
            Assert.AreEqual(Terrain.None, _field.Terrain);
            Assert.AreEqual(0, _field.TerrainDuration);
            Assert.IsNull(_field.TerrainData);
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
            Assert.AreEqual(Terrain.None, _field.Terrain);
            Assert.AreEqual(0, _field.TerrainDuration);
            Assert.IsNull(_field.TerrainData);
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
            Assert.AreEqual(5, _field.TerrainDuration);
            
            _field.DecrementTerrainDuration();
            Assert.AreEqual(4, _field.TerrainDuration);
            
            _field.DecrementTerrainDuration();
            Assert.AreEqual(3, _field.TerrainDuration);
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
            Assert.AreEqual(Terrain.None, _field.Terrain);
            Assert.AreEqual(0, _field.TerrainDuration);
            Assert.IsNull(_field.TerrainData);
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
            Assert.AreEqual(Terrain.Psychic, _field.Terrain);
            Assert.AreEqual(0, _field.TerrainDuration); // Still 0 (infinite)
            Assert.AreEqual(terrainData, _field.TerrainData);
        }

        [Test]
        public void TerrainDuration_NoTerrain_DoesNotDecrement()
        {
            // Arrange - No terrain set

            // Act
            _field.DecrementTerrainDuration();

            // Assert
            Assert.AreEqual(Terrain.None, _field.Terrain);
            Assert.AreEqual(0, _field.TerrainDuration);
        }

        #endregion

        #region Initialization Tests

        [Test]
        public void Initialize_NoTerrain_DefaultsToNone()
        {
            // Act - Already done in SetUp

            // Assert
            Assert.AreEqual(Terrain.None, _field.Terrain);
            Assert.AreEqual(0, _field.TerrainDuration);
            Assert.IsNull(_field.TerrainData);
        }

        #endregion
    }
}

