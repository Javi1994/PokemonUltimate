using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using PokemonUltimate.Combat;
using PokemonUltimate.Combat.Actions;
using PokemonUltimate.Core.Enums;
using PokemonUltimate.Core.Factories;
using PokemonUltimate.Core.Instances;
using PokemonUltimate.Content.Catalogs.Pokemon;
using PokemonUltimate.Content.Catalogs.Terrain;

namespace PokemonUltimate.Tests.Systems.Combat.Actions
{
    /// <summary>
    /// Tests for SetTerrainAction - actions that change battlefield terrain.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.13: Terrain System
    /// **Documentation**: See `docs/features/2-combat-system/2.13-terrain-system/README.md`
    /// </remarks>
    [TestFixture]
    public class SetTerrainActionTests
    {
        private BattleField _field;
        private BattleSlot _userSlot;

        [SetUp]
        public void SetUp()
        {
            _field = new BattleField();
            var rules = new BattleRules { PlayerSlots = 1, EnemySlots = 1 };
            _field.Initialize(
                rules,
                new[] { PokemonFactory.Create(PokemonCatalog.Pikachu, 50) },
                new[] { PokemonFactory.Create(PokemonCatalog.Squirtle, 50) });
            
            _userSlot = _field.PlayerSide.Slots[0];
        }

        #region Set Terrain Tests

        [Test]
        public void ExecuteLogic_Electric_SetsTerrain()
        {
            var terrainData = TerrainCatalog.GetByTerrain(Terrain.Electric);
            var action = new SetTerrainAction(_userSlot, Terrain.Electric, 5, terrainData);

            action.ExecuteLogic(_field);

            Assert.That(_field.Terrain, Is.EqualTo(Terrain.Electric));
            Assert.That(_field.TerrainDuration, Is.EqualTo(5));
            Assert.That(_field.TerrainData, Is.EqualTo(terrainData));
        }

        [Test]
        public void ExecuteLogic_Grassy_SetsTerrain()
        {
            var terrainData = TerrainCatalog.GetByTerrain(Terrain.Grassy);
            var action = new SetTerrainAction(_userSlot, Terrain.Grassy, 5, terrainData);

            action.ExecuteLogic(_field);

            Assert.That(_field.Terrain, Is.EqualTo(Terrain.Grassy));
            Assert.That(_field.TerrainDuration, Is.EqualTo(5));
            Assert.That(_field.TerrainData, Is.EqualTo(terrainData));
        }

        [Test]
        public void ExecuteLogic_Psychic_SetsTerrain()
        {
            var terrainData = TerrainCatalog.GetByTerrain(Terrain.Psychic);
            var action = new SetTerrainAction(_userSlot, Terrain.Psychic, 5, terrainData);

            action.ExecuteLogic(_field);

            Assert.That(_field.Terrain, Is.EqualTo(Terrain.Psychic));
            Assert.That(_field.TerrainDuration, Is.EqualTo(5));
            Assert.That(_field.TerrainData, Is.EqualTo(terrainData));
        }

        [Test]
        public void ExecuteLogic_Misty_SetsTerrain()
        {
            var terrainData = TerrainCatalog.GetByTerrain(Terrain.Misty);
            var action = new SetTerrainAction(_userSlot, Terrain.Misty, 5, terrainData);

            action.ExecuteLogic(_field);

            Assert.That(_field.Terrain, Is.EqualTo(Terrain.Misty));
            Assert.That(_field.TerrainDuration, Is.EqualTo(5));
            Assert.That(_field.TerrainData, Is.EqualTo(terrainData));
        }

        [Test]
        public void ExecuteLogic_None_ClearsTerrain()
        {
            // Set terrain first
            var terrainData = TerrainCatalog.GetByTerrain(Terrain.Electric);
            _field.SetTerrain(Terrain.Electric, 5, terrainData);

            // Clear terrain
            var action = new SetTerrainAction(_userSlot, Terrain.None, 0, null);
            action.ExecuteLogic(_field);

            Assert.That(_field.Terrain, Is.EqualTo(Terrain.None));
            Assert.That(_field.TerrainDuration, Is.EqualTo(0));
            Assert.That(_field.TerrainData, Is.Null);
        }

        [Test]
        public void ExecuteLogic_ReplacesExistingTerrain()
        {
            // Set initial terrain
            var electricTerrain = TerrainCatalog.GetByTerrain(Terrain.Electric);
            _field.SetTerrain(Terrain.Electric, 5, electricTerrain);

            // Replace with different terrain
            var grassyTerrain = TerrainCatalog.GetByTerrain(Terrain.Grassy);
            var action = new SetTerrainAction(_userSlot, Terrain.Grassy, 8, grassyTerrain);
            action.ExecuteLogic(_field);

            Assert.That(_field.Terrain, Is.EqualTo(Terrain.Grassy));
            Assert.That(_field.TerrainDuration, Is.EqualTo(8));
            Assert.That(_field.TerrainData, Is.EqualTo(grassyTerrain));
        }

        [Test]
        public void ExecuteLogic_InfiniteDuration_SetsToZero()
        {
            var terrainData = TerrainCatalog.GetByTerrain(Terrain.Electric);
            var action = new SetTerrainAction(_userSlot, Terrain.Electric, 0, terrainData); // 0 = infinite

            action.ExecuteLogic(_field);

            Assert.That(_field.Terrain, Is.EqualTo(Terrain.Electric));
            Assert.That(_field.TerrainDuration, Is.EqualTo(0)); // Infinite duration
            Assert.That(_field.TerrainData, Is.EqualTo(terrainData));
        }

        #endregion

        #region Null User Tests

        [Test]
        public void ExecuteLogic_NullUser_Works()
        {
            var terrainData = TerrainCatalog.GetByTerrain(Terrain.Grassy);
            var action = new SetTerrainAction(null, Terrain.Grassy, 5, terrainData);

            action.ExecuteLogic(_field);

            Assert.That(_field.Terrain, Is.EqualTo(Terrain.Grassy));
        }

        #endregion
    }
}

