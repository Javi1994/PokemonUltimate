using NUnit.Framework;
using PokemonUltimate.Content.Catalogs.Field;
using PokemonUltimate.Core.Builders;
using PokemonUltimate.Core.Enums;

namespace PokemonUltimate.Tests.Blueprints
{
    /// <summary>
    /// Tests for HazardData and HazardBuilder.
    /// </summary>
    [TestFixture]
    public class HazardDataTests
    {
        #region Builder Tests

        [Test]
        public void Define_WithName_SetsNameAndId()
        {
            var hazard = Hazard.Define("Test Hazard").Build();

            Assert.That(hazard.Name, Is.EqualTo("Test Hazard"));
            Assert.That(hazard.Id, Is.EqualTo("test-hazard"));
        }

        [Test]
        public void MaxLayers_SetsCorrectly()
        {
            var hazard = Hazard.Define("Test")
                .MaxLayers(3)
                .Build();

            Assert.That(hazard.MaxLayers, Is.EqualTo(3));
            Assert.That(hazard.HasLayers, Is.True);
        }

        [Test]
        public void AffectsFlying_SetsCorrectly()
        {
            var hazard = Hazard.Define("Test")
                .AffectsFlying()
                .Build();

            Assert.That(hazard.AffectsFlying, Is.True);
            Assert.That(hazard.AffectsLevitate, Is.True);
            Assert.That(hazard.RequiresGrounded, Is.False);
        }

        [Test]
        public void GroundedOnly_SetsCorrectly()
        {
            var hazard = Hazard.Define("Test")
                .GroundedOnly()
                .Build();

            Assert.That(hazard.AffectsFlying, Is.False);
            Assert.That(hazard.RequiresGrounded, Is.True);
        }

        [Test]
        public void DealsDamage_WithType_SetsCorrectly()
        {
            var hazard = Hazard.Define("Test")
                .DealsDamage(PokemonType.Rock, 0.125f)
                .Build();

            Assert.That(hazard.DamageType, Is.EqualTo(PokemonType.Rock));
            Assert.That(hazard.UseTypeEffectiveness, Is.True);
            Assert.That(hazard.GetDamage(1), Is.EqualTo(0.125f));
        }

        [Test]
        public void DealsFixedDamage_MultipleLayers_SetsCorrectly()
        {
            var hazard = Hazard.Define("Test")
                .DealsFixedDamage(0.125f, 0.167f, 0.25f)
                .Build();

            Assert.That(hazard.GetDamage(1), Is.EqualTo(0.125f));
            Assert.That(hazard.GetDamage(2), Is.EqualTo(0.167f));
            Assert.That(hazard.GetDamage(3), Is.EqualTo(0.25f));
        }

        [Test]
        public void AppliesStatus_SetsCorrectly()
        {
            var hazard = Hazard.Define("Test")
                .AppliesStatus(PersistentStatus.Poison, PersistentStatus.BadlyPoisoned)
                .Build();

            Assert.That(hazard.AppliesStatus, Is.True);
            Assert.That(hazard.GetStatus(1), Is.EqualTo(PersistentStatus.Poison));
            Assert.That(hazard.GetStatus(2), Is.EqualTo(PersistentStatus.BadlyPoisoned));
        }

        [Test]
        public void LowersStat_SetsCorrectly()
        {
            var hazard = Hazard.Define("Test")
                .LowersStat(Stat.Speed, -1)
                .Build();

            Assert.That(hazard.LowersStat, Is.True);
            Assert.That(hazard.StatToLower, Is.EqualTo(Stat.Speed));
            Assert.That(hazard.StatStages, Is.EqualTo(-1));
        }

        #endregion

        #region Helper Methods Tests

        [Test]
        public void AffectsPokemon_FlyingType_ReturnsFalseForGroundedOnly()
        {
            var hazard = Hazard.Define("Test").GroundedOnly().Build();

            bool affects = hazard.AffectsPokemon(PokemonType.Flying, null, null);

            Assert.That(affects, Is.False);
        }

        [Test]
        public void AffectsPokemon_FlyingType_ReturnsTrueForStealthRock()
        {
            var hazard = Hazard.Define("Test").AffectsFlying().Build();

            bool affects = hazard.AffectsPokemon(PokemonType.Flying, null, null);

            Assert.That(affects, Is.True);
        }

        [Test]
        public void AffectsPokemon_LevitateAbility_ReturnsFalse()
        {
            var hazard = Hazard.Define("Test").GroundedOnly().Build();

            bool affects = hazard.AffectsPokemon(PokemonType.Ghost, null, "Levitate");

            Assert.That(affects, Is.False);
        }

        [Test]
        public void AffectsPokemon_MagicGuard_ReturnsFalseForDamageHazards()
        {
            var hazard = Hazard.Define("Test")
                .DealsFixedDamage(0.125f)
                .Build();

            bool affects = hazard.AffectsPokemon(PokemonType.Normal, null, "Magic Guard");

            Assert.That(affects, Is.False);
        }

        [Test]
        public void AffectsPokemon_MagicGuard_ReturnsTrueForStatHazards()
        {
            var hazard = Hazard.Define("Test")
                .LowersStat(Stat.Speed)
                .Build();

            bool affects = hazard.AffectsPokemon(PokemonType.Normal, null, "Magic Guard");

            Assert.That(affects, Is.True);
        }

        [Test]
        public void IsPoisonAbsorber_PoisonType_ReturnsTrue()
        {
            var hazard = Hazard.Define("Test")
                .AppliesStatus(PersistentStatus.Poison)
                .AbsorbedByPoisonTypes()
                .Build();

            Assert.That(hazard.IsPoisonAbsorber(PokemonType.Poison, null), Is.True);
            Assert.That(hazard.IsPoisonAbsorber(PokemonType.Normal, PokemonType.Poison), Is.True);
            Assert.That(hazard.IsPoisonAbsorber(PokemonType.Normal, null), Is.False);
        }

        #endregion

        #region Catalog Tests

        [Test]
        public void Catalog_StealthRock_HasCorrectProperties()
        {
            var sr = HazardCatalog.StealthRock;

            Assert.That(sr.Type, Is.EqualTo(HazardType.StealthRock));
            Assert.That(sr.MaxLayers, Is.EqualTo(1));
            Assert.That(sr.AffectsFlying, Is.True);
            Assert.That(sr.UseTypeEffectiveness, Is.True);
            Assert.That(sr.DamageType, Is.EqualTo(PokemonType.Rock));
        }

        [Test]
        public void Catalog_Spikes_HasCorrectProperties()
        {
            var spikes = HazardCatalog.Spikes;

            Assert.That(spikes.Type, Is.EqualTo(HazardType.Spikes));
            Assert.That(spikes.MaxLayers, Is.EqualTo(3));
            Assert.That(spikes.AffectsFlying, Is.False);
            Assert.That(spikes.GetDamage(1), Is.EqualTo(0.125f).Within(0.001f));
            Assert.That(spikes.GetDamage(3), Is.EqualTo(0.25f).Within(0.001f));
        }

        [Test]
        public void Catalog_ToxicSpikes_HasCorrectProperties()
        {
            var ts = HazardCatalog.ToxicSpikes;

            Assert.That(ts.Type, Is.EqualTo(HazardType.ToxicSpikes));
            Assert.That(ts.MaxLayers, Is.EqualTo(2));
            Assert.That(ts.AppliesStatus, Is.True);
            Assert.That(ts.AbsorbedByPoisonTypes, Is.True);
            Assert.That(ts.GetStatus(1), Is.EqualTo(PersistentStatus.Poison));
            Assert.That(ts.GetStatus(2), Is.EqualTo(PersistentStatus.BadlyPoisoned));
        }

        [Test]
        public void Catalog_StickyWeb_HasCorrectProperties()
        {
            var sw = HazardCatalog.StickyWeb;

            Assert.That(sw.Type, Is.EqualTo(HazardType.StickyWeb));
            Assert.That(sw.MaxLayers, Is.EqualTo(1));
            Assert.That(sw.LowersStat, Is.True);
            Assert.That(sw.StatToLower, Is.EqualTo(Stat.Speed));
            Assert.That(sw.StatStages, Is.EqualTo(-1));
        }

        [Test]
        public void Catalog_All_Contains4Hazards()
        {
            Assert.That(HazardCatalog.All.Count, Is.EqualTo(4));
        }

        [Test]
        public void Catalog_GetByType_ReturnsCorrectHazard()
        {
            Assert.That(HazardCatalog.GetByType(HazardType.StealthRock), Is.EqualTo(HazardCatalog.StealthRock));
            Assert.That(HazardCatalog.GetByType(HazardType.Spikes), Is.EqualTo(HazardCatalog.Spikes));
            Assert.That(HazardCatalog.GetByType(HazardType.None), Is.Null);
        }

        #endregion
    }
}

