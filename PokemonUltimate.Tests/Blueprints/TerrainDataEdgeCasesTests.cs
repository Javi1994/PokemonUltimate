using NUnit.Framework;
using PokemonUltimate.Content.Catalogs.Terrain;
using PokemonUltimate.Core.Blueprints;
using PokemonUltimate.Content.Builders;
using PokemonUltimate.Core.Enums;

namespace PokemonUltimate.Tests.Blueprints
{
    /// <summary>
    /// Edge case tests for TerrainData.
    /// </summary>
    [TestFixture]
    public class TerrainDataEdgeCasesTests
    {
        #region Default Values Tests

        [Test]
        public void Default_NoBoostedType_GetTypePowerMultiplierReturnsOne()
        {
            var terrain = TerrainEffect.Define("Test").Build();

            Assert.That(terrain.BoostedType, Is.Null);
            Assert.That(terrain.GetTypePowerMultiplier(PokemonType.Grass), Is.EqualTo(1f));
        }

        [Test]
        public void Default_NoReducedDamageType_GetDamageReceivedMultiplierReturnsOne()
        {
            var terrain = TerrainEffect.Define("Test").Build();

            Assert.That(terrain.ReducedDamageType, Is.Null);
            Assert.That(terrain.GetDamageReceivedMultiplier(PokemonType.Dragon), Is.EqualTo(1f));
        }

        [Test]
        public void Default_NoHealing_HealsEachTurnIsFalse()
        {
            var terrain = TerrainEffect.Define("Test").Build();

            Assert.That(terrain.EndOfTurnHealing, Is.EqualTo(0f));
            Assert.That(terrain.HealsEachTurn, Is.False);
        }

        [Test]
        public void Default_NoStatusPrevention_PreventsStatusesIsFalse()
        {
            var terrain = TerrainEffect.Define("Test").Build();

            Assert.That(terrain.PreventedStatuses, Is.Empty);
            Assert.That(terrain.PreventsStatuses, Is.False);
        }

        [Test]
        public void Default_BlocksPriorityMoves_IsFalse()
        {
            var terrain = TerrainEffect.Define("Test").Build();

            Assert.That(terrain.BlocksPriorityMoves, Is.False);
        }

        [Test]
        public void Default_HalvedDamageMoves_IsEmpty()
        {
            var terrain = TerrainEffect.Define("Test").Build();

            Assert.That(terrain.HalvedDamageMoves, Is.Empty);
        }

        #endregion

        #region IsGrounded Edge Cases

        [Test]
        public void IsGrounded_LevitateCase_Insensitive()
        {
            bool grounded1 = TerrainData.IsGrounded(PokemonType.Ghost, null, "LEVITATE", null);
            bool grounded2 = TerrainData.IsGrounded(PokemonType.Ghost, null, "levitate", null);
            bool grounded3 = TerrainData.IsGrounded(PokemonType.Ghost, null, "Levitate", null);

            Assert.That(grounded1, Is.False);
            Assert.That(grounded2, Is.False);
            Assert.That(grounded3, Is.False);
        }

        [Test]
        public void IsGrounded_AirBalloon_CaseInsensitive()
        {
            bool grounded1 = TerrainData.IsGrounded(PokemonType.Steel, null, null, "AIR BALLOON");
            bool grounded2 = TerrainData.IsGrounded(PokemonType.Steel, null, null, "air balloon");
            bool grounded3 = TerrainData.IsGrounded(PokemonType.Steel, null, null, "Air Balloon");

            Assert.That(grounded1, Is.False);
            Assert.That(grounded2, Is.False);
            Assert.That(grounded3, Is.False);
        }

        [Test]
        public void IsGrounded_EmptyAbilityName_ReturnsTrue()
        {
            bool grounded = TerrainData.IsGrounded(PokemonType.Normal, null, "", null);

            Assert.That(grounded, Is.True);
        }

        [Test]
        public void IsGrounded_EmptyItemName_ReturnsTrue()
        {
            bool grounded = TerrainData.IsGrounded(PokemonType.Normal, null, null, "");

            Assert.That(grounded, Is.True);
        }

        [Test]
        public void IsGrounded_FlyingSecondary_ReturnsFalse()
        {
            // Water/Flying like Gyarados - should not be grounded
            bool grounded = TerrainData.IsGrounded(PokemonType.Water, PokemonType.Flying, null, null);
            Assert.That(grounded, Is.False, "Gyarados (Water/Flying) should not be grounded");

            Assert.That(grounded, Is.False);
        }

        [Test]
        public void IsGrounded_FlyingPrimary_ReturnsFalse()
        {
            // Flying/Ground like Gligar (Flying primary)
            bool grounded = TerrainData.IsGrounded(PokemonType.Flying, PokemonType.Ground, null, null);

            Assert.That(grounded, Is.False);
        }

        [Test]
        public void IsGrounded_NonLevitateAbility_ReturnsTrue()
        {
            bool grounded = TerrainData.IsGrounded(PokemonType.Ghost, null, "Shadow Tag", null);

            Assert.That(grounded, Is.True);
        }

        [Test]
        public void IsGrounded_NonAirBalloonItem_ReturnsTrue()
        {
            bool grounded = TerrainData.IsGrounded(PokemonType.Steel, null, null, "Leftovers");

            Assert.That(grounded, Is.True);
        }

        #endregion

        #region Case Sensitivity Tests

        [Test]
        public void HalvesMoveDamage_CaseInsensitive()
        {
            var terrain = TerrainEffect.Define("Test")
                .HalvesDamageFrom("Earthquake")
                .Build();

            Assert.That(terrain.HalvesMoveDamage("EARTHQUAKE"), Is.True);
            Assert.That(terrain.HalvesMoveDamage("earthquake"), Is.True);
            Assert.That(terrain.HalvesMoveDamage("EarthQuake"), Is.True);
        }

        #endregion

        #region Catalog Lookup Edge Cases

        [Test]
        public void GetByTerrain_None_ReturnsNull()
        {
            Assert.That(TerrainCatalog.GetByTerrain(Terrain.None), Is.Null);
        }

        [Test]
        public void GetByName_NonExistent_ReturnsNull()
        {
            Assert.That(TerrainCatalog.GetByName("Nonexistent Terrain"), Is.Null);
        }

        [Test]
        public void GetByName_CorrectName_ReturnsTerrain()
        {
            Assert.That(TerrainCatalog.GetByName("Grassy Terrain"), Is.EqualTo(TerrainCatalog.Grassy));
            Assert.That(TerrainCatalog.GetByName("Electric Terrain"), Is.EqualTo(TerrainCatalog.Electric));
        }

        [Test]
        public void GetTerrainsByBoostedType_Grass_ReturnsGrassy()
        {
            var grassTerrains = new System.Collections.Generic.List<TerrainData>(
                TerrainCatalog.GetTerrainsByBoostedType(PokemonType.Grass));

            Assert.That(grassTerrains, Contains.Item(TerrainCatalog.Grassy));
            Assert.That(grassTerrains.Count, Is.EqualTo(1));
        }

        [Test]
        public void GetTerrainsByBoostedType_Fire_ReturnsEmpty()
        {
            var fireTerrains = new System.Collections.Generic.List<TerrainData>(
                TerrainCatalog.GetTerrainsByBoostedType(PokemonType.Fire));

            Assert.That(fireTerrains, Is.Empty);
        }

        #endregion

        #region Real Game Scenario Tests

        [Test]
        public void GrassyTerrain_GrassMove_Gets1_3xBoost()
        {
            var grassy = TerrainCatalog.Grassy;

            Assert.That(grassy.GetTypePowerMultiplier(PokemonType.Grass), Is.EqualTo(1.3f));
        }

        [Test]
        public void GrassyTerrain_Earthquake_IsHalved()
        {
            var grassy = TerrainCatalog.Grassy;

            Assert.That(grassy.HalvesMoveDamage("Earthquake"), Is.True);
            Assert.That(grassy.HalvesMoveDamage("Bulldoze"), Is.True);
            Assert.That(grassy.HalvesMoveDamage("Magnitude"), Is.True);
        }

        [Test]
        public void GrassyTerrain_Heals1_16PerTurn()
        {
            var grassy = TerrainCatalog.Grassy;

            Assert.That(grassy.EndOfTurnHealing, Is.EqualTo(1f / 16f).Within(0.001f));
        }

        [Test]
        public void ElectricTerrain_PreventsSleepOnly()
        {
            var electric = TerrainCatalog.Electric;

            Assert.That(electric.PreventsStatus(PersistentStatus.Sleep), Is.True);
            Assert.That(electric.PreventsStatus(PersistentStatus.Paralysis), Is.False);
            Assert.That(electric.PreventsStatus(PersistentStatus.Burn), Is.False);
        }

        [Test]
        public void PsychicTerrain_BlocksPriority()
        {
            var psychic = TerrainCatalog.Psychic;

            Assert.That(psychic.BlocksPriorityMoves, Is.True);
            Assert.That(psychic.PreventsStatuses, Is.False);
        }

        [Test]
        public void MistyTerrain_HalvesDragonDamage()
        {
            var misty = TerrainCatalog.Misty;

            Assert.That(misty.GetDamageReceivedMultiplier(PokemonType.Dragon), Is.EqualTo(0.5f));
            Assert.That(misty.GetDamageReceivedMultiplier(PokemonType.Fire), Is.EqualTo(1f));
        }

        [Test]
        public void MistyTerrain_PreventsAllMajorStatuses()
        {
            var misty = TerrainCatalog.Misty;

            Assert.That(misty.PreventsStatus(PersistentStatus.Burn), Is.True);
            Assert.That(misty.PreventsStatus(PersistentStatus.Paralysis), Is.True);
            Assert.That(misty.PreventsStatus(PersistentStatus.Sleep), Is.True);
            Assert.That(misty.PreventsStatus(PersistentStatus.Poison), Is.True);
            Assert.That(misty.PreventsStatus(PersistentStatus.Freeze), Is.True);
        }

        [Test]
        public void AllTerrains_HaveSetterAbilities()
        {
            Assert.That(TerrainCatalog.Grassy.SetterAbilities, Contains.Item("Grassy Surge"));
            Assert.That(TerrainCatalog.Electric.SetterAbilities, Contains.Item("Electric Surge"));
            Assert.That(TerrainCatalog.Psychic.SetterAbilities, Contains.Item("Psychic Surge"));
            Assert.That(TerrainCatalog.Misty.SetterAbilities, Contains.Item("Misty Surge"));
        }

        [Test]
        public void AllTerrains_HaveNaturePowerMoves()
        {
            Assert.That(TerrainCatalog.Grassy.NaturePowerMove, Is.EqualTo("Energy Ball"));
            Assert.That(TerrainCatalog.Electric.NaturePowerMove, Is.EqualTo("Thunderbolt"));
            Assert.That(TerrainCatalog.Psychic.NaturePowerMove, Is.EqualTo("Psychic"));
            Assert.That(TerrainCatalog.Misty.NaturePowerMove, Is.EqualTo("Moonblast"));
        }

        [Test]
        public void AllTerrains_HaveCamouflageTypes()
        {
            Assert.That(TerrainCatalog.Grassy.CamouflageType, Is.EqualTo(PokemonType.Grass));
            Assert.That(TerrainCatalog.Electric.CamouflageType, Is.EqualTo(PokemonType.Electric));
            Assert.That(TerrainCatalog.Psychic.CamouflageType, Is.EqualTo(PokemonType.Psychic));
            Assert.That(TerrainCatalog.Misty.CamouflageType, Is.EqualTo(PokemonType.Fairy));
        }

        #endregion
    }
}

