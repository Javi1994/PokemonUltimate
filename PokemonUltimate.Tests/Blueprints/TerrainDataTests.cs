using NUnit.Framework;
using PokemonUltimate.Content.Catalogs.Terrain;
using PokemonUltimate.Core.Blueprints;
using PokemonUltimate.Core.Builders;
using PokemonUltimate.Core.Enums;

namespace PokemonUltimate.Tests.Blueprints
{
    /// <summary>
    /// Tests for TerrainData and TerrainBuilder.
    /// </summary>
    [TestFixture]
    public class TerrainDataTests
    {
        #region Builder Tests

        [Test]
        public void Define_WithName_SetsNameAndId()
        {
            var terrain = TerrainEffect.Define("Test Terrain")
                .Build();

            Assert.That(terrain.Name, Is.EqualTo("Test Terrain"));
            Assert.That(terrain.Id, Is.EqualTo("test-terrain"));
        }

        [Test]
        public void Type_SetsTerrainEnum()
        {
            var terrain = TerrainEffect.Define("Test")
                .Type(Terrain.Grassy)
                .Build();

            Assert.That(terrain.Terrain, Is.EqualTo(Terrain.Grassy));
        }

        [Test]
        public void Duration_SetsDefaultDuration()
        {
            var terrain = TerrainEffect.Define("Test")
                .Duration(8)
                .Build();

            Assert.That(terrain.DefaultDuration, Is.EqualTo(8));
        }

        [Test]
        public void Default_Duration_Is5Turns()
        {
            var terrain = TerrainEffect.Define("Test").Build();

            Assert.That(terrain.DefaultDuration, Is.EqualTo(5));
        }

        [Test]
        public void Boosts_SetsBoostedTypeAndMultiplier()
        {
            var terrain = TerrainEffect.Define("Test")
                .Boosts(PokemonType.Grass)
                .Build();

            Assert.That(terrain.BoostedType, Is.EqualTo(PokemonType.Grass));
            Assert.That(terrain.BoostMultiplier, Is.EqualTo(1.3f));
        }

        [Test]
        public void Boosts_CustomMultiplier_SetsCorrectly()
        {
            var terrain = TerrainEffect.Define("Test")
                .Boosts(PokemonType.Grass, 1.5f)
                .Build();

            Assert.That(terrain.BoostMultiplier, Is.EqualTo(1.5f));
        }

        [Test]
        public void ReducesDamageFrom_SetsTypeAndMultiplier()
        {
            var terrain = TerrainEffect.Define("Test")
                .ReducesDamageFrom(PokemonType.Dragon)
                .Build();

            Assert.That(terrain.ReducedDamageType, Is.EqualTo(PokemonType.Dragon));
            Assert.That(terrain.DamageReductionMultiplier, Is.EqualTo(0.5f));
        }

        [Test]
        public void HealsEachTurn_SetsHealing()
        {
            var terrain = TerrainEffect.Define("Test")
                .HealsEachTurn(0.0625f)
                .Build();

            Assert.That(terrain.EndOfTurnHealing, Is.EqualTo(0.0625f).Within(0.0001f));
            Assert.That(terrain.HealsEachTurn, Is.True);
        }

        [Test]
        public void PreventsSleep_AddsSleepToPreventedStatuses()
        {
            var terrain = TerrainEffect.Define("Test")
                .PreventsSleep()
                .Build();

            Assert.That(terrain.PreventedStatuses, Contains.Item(PersistentStatus.Sleep));
            Assert.That(terrain.PreventsStatuses, Is.True);
        }

        [Test]
        public void PreventsAllStatuses_AddsAllMajorStatuses()
        {
            var terrain = TerrainEffect.Define("Test")
                .PreventsAllStatuses()
                .Build();

            Assert.That(terrain.PreventedStatuses, Contains.Item(PersistentStatus.Burn));
            Assert.That(terrain.PreventedStatuses, Contains.Item(PersistentStatus.Paralysis));
            Assert.That(terrain.PreventedStatuses, Contains.Item(PersistentStatus.Sleep));
            Assert.That(terrain.PreventedStatuses, Contains.Item(PersistentStatus.Poison));
            Assert.That(terrain.PreventedStatuses, Contains.Item(PersistentStatus.Freeze));
        }

        [Test]
        public void BlocksPriorityMoves_SetsFlag()
        {
            var terrain = TerrainEffect.Define("Test")
                .BlocksPriorityMoves()
                .Build();

            Assert.That(terrain.BlocksPriorityMoves, Is.True);
        }

        [Test]
        public void HalvesDamageFrom_AddsMoves()
        {
            var terrain = TerrainEffect.Define("Test")
                .HalvesDamageFrom("Earthquake", "Bulldoze")
                .Build();

            Assert.That(terrain.HalvedDamageMoves, Contains.Item("Earthquake"));
            Assert.That(terrain.HalvedDamageMoves, Contains.Item("Bulldoze"));
        }

        [Test]
        public void NaturePowerBecomes_SetsMove()
        {
            var terrain = TerrainEffect.Define("Test")
                .NaturePowerBecomes("Energy Ball")
                .Build();

            Assert.That(terrain.NaturePowerMove, Is.EqualTo("Energy Ball"));
        }

        [Test]
        public void CamouflageChangesTo_SetsType()
        {
            var terrain = TerrainEffect.Define("Test")
                .CamouflageChangesTo(PokemonType.Grass)
                .Build();

            Assert.That(terrain.CamouflageType, Is.EqualTo(PokemonType.Grass));
        }

        #endregion

        #region Helper Method Tests

        [Test]
        public void GetTypePowerMultiplier_BoostedType_ReturnsBoost()
        {
            var terrain = TerrainEffect.Define("Test")
                .Boosts(PokemonType.Grass)
                .Build();

            Assert.That(terrain.GetTypePowerMultiplier(PokemonType.Grass), Is.EqualTo(1.3f));
        }

        [Test]
        public void GetTypePowerMultiplier_NonBoostedType_ReturnsOne()
        {
            var terrain = TerrainEffect.Define("Test")
                .Boosts(PokemonType.Grass)
                .Build();

            Assert.That(terrain.GetTypePowerMultiplier(PokemonType.Fire), Is.EqualTo(1f));
        }

        [Test]
        public void GetDamageReceivedMultiplier_ReducedType_ReturnsReduction()
        {
            var terrain = TerrainEffect.Define("Test")
                .ReducesDamageFrom(PokemonType.Dragon)
                .Build();

            Assert.That(terrain.GetDamageReceivedMultiplier(PokemonType.Dragon), Is.EqualTo(0.5f));
        }

        [Test]
        public void GetDamageReceivedMultiplier_NormalType_ReturnsOne()
        {
            var terrain = TerrainEffect.Define("Test")
                .ReducesDamageFrom(PokemonType.Dragon)
                .Build();

            Assert.That(terrain.GetDamageReceivedMultiplier(PokemonType.Fire), Is.EqualTo(1f));
        }

        [Test]
        public void PreventsStatus_PreventedStatus_ReturnsTrue()
        {
            var terrain = TerrainEffect.Define("Test")
                .PreventsSleep()
                .Build();

            Assert.That(terrain.PreventsStatus(PersistentStatus.Sleep), Is.True);
            Assert.That(terrain.PreventsStatus(PersistentStatus.Burn), Is.False);
        }

        [Test]
        public void HalvesMoveDamage_HalvedMove_ReturnsTrue()
        {
            var terrain = TerrainEffect.Define("Test")
                .HalvesDamageFrom("Earthquake")
                .Build();

            Assert.That(terrain.HalvesMoveDamage("Earthquake"), Is.True);
            Assert.That(terrain.HalvesMoveDamage("earthquake"), Is.True); // Case insensitive
            Assert.That(terrain.HalvesMoveDamage("Thunderbolt"), Is.False);
        }

        #endregion

        #region IsGrounded Tests

        [Test]
        public void IsGrounded_NormalType_ReturnsTrue()
        {
            bool grounded = TerrainData.IsGrounded(PokemonType.Normal, null, null, null);

            Assert.That(grounded, Is.True);
        }

        [Test]
        public void IsGrounded_FlyingType_ReturnsFalse()
        {
            bool grounded = TerrainData.IsGrounded(PokemonType.Flying, null, null, null);

            Assert.That(grounded, Is.False);
        }

        [Test]
        public void IsGrounded_DualFlyingType_ReturnsFalse()
        {
            bool grounded = TerrainData.IsGrounded(PokemonType.Normal, PokemonType.Flying, null, null);

            Assert.That(grounded, Is.False);
        }

        [Test]
        public void IsGrounded_LevitateAbility_ReturnsFalse()
        {
            bool grounded = TerrainData.IsGrounded(PokemonType.Ghost, null, "Levitate", null);

            Assert.That(grounded, Is.False);
        }

        [Test]
        public void IsGrounded_AirBalloon_ReturnsFalse()
        {
            bool grounded = TerrainData.IsGrounded(PokemonType.Steel, null, null, "Air Balloon");

            Assert.That(grounded, Is.False);
        }

        [Test]
        public void IsGrounded_GroundType_ReturnsTrue()
        {
            bool grounded = TerrainData.IsGrounded(PokemonType.Ground, null, null, null);

            Assert.That(grounded, Is.True);
        }

        #endregion

        #region Catalog Tests

        [Test]
        public void Catalog_Grassy_HasCorrectProperties()
        {
            var grassy = TerrainCatalog.Grassy;

            Assert.That(grassy.Terrain, Is.EqualTo(Terrain.Grassy));
            Assert.That(grassy.BoostedType, Is.EqualTo(PokemonType.Grass));
            Assert.That(grassy.BoostMultiplier, Is.EqualTo(1.3f));
            Assert.That(grassy.HealsEachTurn, Is.True);
            Assert.That(grassy.HalvesMoveDamage("Earthquake"), Is.True);
            Assert.That(grassy.NaturePowerMove, Is.EqualTo("Energy Ball"));
        }

        [Test]
        public void Catalog_Electric_HasCorrectProperties()
        {
            var electric = TerrainCatalog.Electric;

            Assert.That(electric.Terrain, Is.EqualTo(Terrain.Electric));
            Assert.That(electric.BoostedType, Is.EqualTo(PokemonType.Electric));
            Assert.That(electric.PreventsStatus(PersistentStatus.Sleep), Is.True);
            Assert.That(electric.NaturePowerMove, Is.EqualTo("Thunderbolt"));
        }

        [Test]
        public void Catalog_Psychic_HasCorrectProperties()
        {
            var psychic = TerrainCatalog.Psychic;

            Assert.That(psychic.Terrain, Is.EqualTo(Terrain.Psychic));
            Assert.That(psychic.BoostedType, Is.EqualTo(PokemonType.Psychic));
            Assert.That(psychic.BlocksPriorityMoves, Is.True);
            Assert.That(psychic.NaturePowerMove, Is.EqualTo("Psychic"));
        }

        [Test]
        public void Catalog_Misty_HasCorrectProperties()
        {
            var misty = TerrainCatalog.Misty;

            Assert.That(misty.Terrain, Is.EqualTo(Terrain.Misty));
            Assert.That(misty.ReducedDamageType, Is.EqualTo(PokemonType.Dragon));
            Assert.That(misty.GetDamageReceivedMultiplier(PokemonType.Dragon), Is.EqualTo(0.5f));
            Assert.That(misty.PreventsStatuses, Is.True);
            Assert.That(misty.PreventsStatus(PersistentStatus.Burn), Is.True);
            Assert.That(misty.PreventsStatus(PersistentStatus.Sleep), Is.True);
        }

        [Test]
        public void Catalog_GetByTerrain_ReturnsCorrectData()
        {
            Assert.That(TerrainCatalog.GetByTerrain(Terrain.Grassy), Is.EqualTo(TerrainCatalog.Grassy));
            Assert.That(TerrainCatalog.GetByTerrain(Terrain.Electric), Is.EqualTo(TerrainCatalog.Electric));
            Assert.That(TerrainCatalog.GetByTerrain(Terrain.Psychic), Is.EqualTo(TerrainCatalog.Psychic));
            Assert.That(TerrainCatalog.GetByTerrain(Terrain.Misty), Is.EqualTo(TerrainCatalog.Misty));
            Assert.That(TerrainCatalog.GetByTerrain(Terrain.None), Is.Null);
        }

        [Test]
        public void Catalog_All_Contains4Terrains()
        {
            Assert.That(TerrainCatalog.All.Count, Is.EqualTo(4));
        }

        [Test]
        public void Catalog_GetStatusPreventingTerrains_ReturnsCorrectTerrains()
        {
            var preventingTerrains = new System.Collections.Generic.List<TerrainData>(
                TerrainCatalog.GetStatusPreventingTerrains());

            Assert.That(preventingTerrains, Contains.Item(TerrainCatalog.Electric)); // Prevents Sleep
            Assert.That(preventingTerrains, Contains.Item(TerrainCatalog.Misty));    // Prevents All
        }

        #endregion
    }
}

