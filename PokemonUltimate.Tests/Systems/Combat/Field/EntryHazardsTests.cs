using System.Collections.Generic;
using NUnit.Framework;
using PokemonUltimate.Combat;
using PokemonUltimate.Core.Enums;
using PokemonUltimate.Core.Factories;
using PokemonUltimate.Core.Instances;
using PokemonUltimate.Content.Catalogs.Field;
using PokemonUltimate.Content.Catalogs.Pokemon;

namespace PokemonUltimate.Tests.Systems.Combat.Field
{
    /// <summary>
    /// Tests for Entry Hazards tracking in BattleSide.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.14: Hazards System
    /// **Documentation**: See `docs/features/2-combat-system/2.14-hazards-system/README.md`
    /// </remarks>
    [TestFixture]
    public class EntryHazardsTests
    {
        private BattleSide _side;
        private BattleRules _rules;

        [SetUp]
        public void SetUp()
        {
            _rules = new BattleRules { PlayerSlots = 1, EnemySlots = 1 };
            _side = new BattleSide(1, isPlayer: true);
            var party = new List<PokemonInstance>
            {
                PokemonFactory.Create(PokemonCatalog.Pikachu, 50)
            };
            _side.SetParty(party);
        }

        #region Add Hazard Tests

        [Test]
        public void AddHazard_Spikes_AddsLayer()
        {
            var hazardData = HazardCatalog.GetByType(HazardType.Spikes);

            _side.AddHazard(hazardData);

            Assert.That(_side.HasHazard(HazardType.Spikes), Is.True);
            Assert.That(_side.GetHazardLayers(HazardType.Spikes), Is.EqualTo(1));
        }

        [Test]
        public void AddHazard_Spikes_MaxLayers_ClampsToThree()
        {
            var hazardData = HazardCatalog.GetByType(HazardType.Spikes);

            // Add 4 layers (should clamp to 3)
            _side.AddHazard(hazardData);
            _side.AddHazard(hazardData);
            _side.AddHazard(hazardData);
            _side.AddHazard(hazardData);

            Assert.That(_side.GetHazardLayers(HazardType.Spikes), Is.EqualTo(3));
        }

        [Test]
        public void AddHazard_ToxicSpikes_MaxLayers_ClampsToTwo()
        {
            var hazardData = HazardCatalog.GetByType(HazardType.ToxicSpikes);

            // Add 3 layers (should clamp to 2)
            _side.AddHazard(hazardData);
            _side.AddHazard(hazardData);
            _side.AddHazard(hazardData);

            Assert.That(_side.GetHazardLayers(HazardType.ToxicSpikes), Is.EqualTo(2));
        }

        [Test]
        public void AddHazard_StealthRock_SetsToOne()
        {
            var hazardData = HazardCatalog.GetByType(HazardType.StealthRock);

            _side.AddHazard(hazardData);
            _side.AddHazard(hazardData); // Adding again shouldn't increase layers

            Assert.That(_side.HasHazard(HazardType.StealthRock), Is.True);
            Assert.That(_side.GetHazardLayers(HazardType.StealthRock), Is.EqualTo(1));
        }

        [Test]
        public void AddHazard_StickyWeb_SetsToOne()
        {
            var hazardData = HazardCatalog.GetByType(HazardType.StickyWeb);

            _side.AddHazard(hazardData);
            _side.AddHazard(hazardData); // Adding again shouldn't increase layers

            Assert.That(_side.HasHazard(HazardType.StickyWeb), Is.True);
            Assert.That(_side.GetHazardLayers(HazardType.StickyWeb), Is.EqualTo(1));
        }

        #endregion

        #region Remove Hazard Tests

        [Test]
        public void RemoveHazard_Spikes_RemovesHazard()
        {
            var hazardData = HazardCatalog.GetByType(HazardType.Spikes);
            _side.AddHazard(hazardData);

            _side.RemoveHazard(HazardType.Spikes);

            Assert.That(_side.HasHazard(HazardType.Spikes), Is.False);
            Assert.That(_side.GetHazardLayers(HazardType.Spikes), Is.EqualTo(0));
        }

        [Test]
        public void RemoveHazard_AllHazards_RemovesAll()
        {
            var spikesData = HazardCatalog.GetByType(HazardType.Spikes);
            var stealthRockData = HazardCatalog.GetByType(HazardType.StealthRock);
            var toxicSpikesData = HazardCatalog.GetByType(HazardType.ToxicSpikes);
            var stickyWebData = HazardCatalog.GetByType(HazardType.StickyWeb);

            _side.AddHazard(spikesData);
            _side.AddHazard(stealthRockData);
            _side.AddHazard(toxicSpikesData);
            _side.AddHazard(stickyWebData);

            _side.RemoveAllHazards();

            Assert.That(_side.HasHazard(HazardType.Spikes), Is.False);
            Assert.That(_side.HasHazard(HazardType.StealthRock), Is.False);
            Assert.That(_side.HasHazard(HazardType.ToxicSpikes), Is.False);
            Assert.That(_side.HasHazard(HazardType.StickyWeb), Is.False);
        }

        [Test]
        public void RemoveHazard_NoHazard_DoesNothing()
        {
            // No hazards added

            _side.RemoveHazard(HazardType.Spikes);

            Assert.That(_side.HasHazard(HazardType.Spikes), Is.False);
        }

        #endregion

        #region Has Hazard Tests

        [Test]
        public void HasHazard_NoHazards_ReturnsFalse()
        {
            Assert.That(_side.HasHazard(HazardType.Spikes), Is.False);
            Assert.That(_side.HasHazard(HazardType.StealthRock), Is.False);
            Assert.That(_side.HasHazard(HazardType.ToxicSpikes), Is.False);
            Assert.That(_side.HasHazard(HazardType.StickyWeb), Is.False);
        }

        [Test]
        public void HasHazard_WithHazard_ReturnsTrue()
        {
            var hazardData = HazardCatalog.GetByType(HazardType.Spikes);
            _side.AddHazard(hazardData);

            Assert.That(_side.HasHazard(HazardType.Spikes), Is.True);
        }

        #endregion

        #region Get Hazard Layers Tests

        [Test]
        public void GetHazardLayers_NoHazard_ReturnsZero()
        {
            Assert.That(_side.GetHazardLayers(HazardType.Spikes), Is.EqualTo(0));
        }

        [Test]
        public void GetHazardLayers_WithLayers_ReturnsCorrectCount()
        {
            var hazardData = HazardCatalog.GetByType(HazardType.Spikes);
            
            _side.AddHazard(hazardData);
            Assert.That(_side.GetHazardLayers(HazardType.Spikes), Is.EqualTo(1));
            
            _side.AddHazard(hazardData);
            Assert.That(_side.GetHazardLayers(HazardType.Spikes), Is.EqualTo(2));
            
            _side.AddHazard(hazardData);
            Assert.That(_side.GetHazardLayers(HazardType.Spikes), Is.EqualTo(3));
        }

        #endregion
    }
}

