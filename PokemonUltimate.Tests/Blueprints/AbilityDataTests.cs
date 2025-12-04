using NUnit.Framework;
using PokemonUltimate.Core.Blueprints;
using PokemonUltimate.Content.Builders;
using PokemonUltimate.Core.Enums;
using PokemonUltimate.Content.Catalogs.Abilities;

namespace PokemonUltimate.Tests.Blueprints
{
    /// <summary>
    /// Tests for AbilityData and AbilityBuilder.
    /// </summary>
    [TestFixture]
    public class AbilityDataTests
    {
        #region Builder Tests

        [Test]
        public void Define_CreatesAbilityWithName()
        {
            var ability = Ability.Define("Test Ability").Build();

            Assert.That(ability.Name, Is.EqualTo("Test Ability"));
            Assert.That(ability.Id, Is.EqualTo("test-ability"));
        }

        [Test]
        public void Description_SetsDescription()
        {
            var ability = Ability.Define("Test")
                .Description("A test ability")
                .Build();

            Assert.That(ability.Description, Is.EqualTo("A test ability"));
        }

        [Test]
        public void Gen_SetsGeneration()
        {
            var ability = Ability.Define("Test")
                .Gen(5)
                .Build();

            Assert.That(ability.Generation, Is.EqualTo(5));
        }

        [Test]
        public void OnSwitchIn_SetsTrigger()
        {
            var ability = Ability.Define("Test")
                .OnSwitchIn()
                .Build();

            Assert.That(ability.ListensTo(AbilityTrigger.OnSwitchIn), Is.True);
        }

        [Test]
        public void LowersOpponentStat_ConfiguresCorrectly()
        {
            var ability = Ability.Define("Intimidate Clone")
                .LowersOpponentStat(Stat.Attack, -1)
                .Build();

            Assert.That(ability.Effect, Is.EqualTo(AbilityEffect.LowerOpponentStat));
            Assert.That(ability.TargetStat, Is.EqualTo(Stat.Attack));
            Assert.That(ability.StatStages, Is.EqualTo(-1));
            Assert.That(ability.ListensTo(AbilityTrigger.OnSwitchIn), Is.True);
        }

        [Test]
        public void ChanceToStatusOnContact_ConfiguresCorrectly()
        {
            var ability = Ability.Define("Static Clone")
                .ChanceToStatusOnContact(PersistentStatus.Paralysis, 0.30f)
                .Build();

            Assert.That(ability.Effect, Is.EqualTo(AbilityEffect.ChanceToStatusOnContact));
            Assert.That(ability.StatusEffect, Is.EqualTo(PersistentStatus.Paralysis));
            Assert.That(ability.EffectChance, Is.EqualTo(0.30f));
            Assert.That(ability.ListensTo(AbilityTrigger.OnContactReceived), Is.True);
        }

        [Test]
        public void GroundImmunity_ConfiguresCorrectly()
        {
            var ability = Ability.Define("Levitate Clone")
                .GroundImmunity()
                .Build();

            Assert.That(ability.Effect, Is.EqualTo(AbilityEffect.GroundImmunity));
            Assert.That(ability.AffectedType, Is.EqualTo(PokemonType.Ground));
            Assert.That(ability.IsPassive, Is.True);
        }

        [Test]
        public void BoostsTypeWhenLowHP_ConfiguresCorrectly()
        {
            var ability = Ability.Define("Blaze Clone")
                .BoostsTypeWhenLowHP(PokemonType.Fire, 0.33f, 1.5f)
                .Build();

            Assert.That(ability.Effect, Is.EqualTo(AbilityEffect.TypePowerBoostWhenLowHP));
            Assert.That(ability.AffectedType, Is.EqualTo(PokemonType.Fire));
            Assert.That(ability.HPThreshold, Is.EqualTo(0.33f));
            Assert.That(ability.Multiplier, Is.EqualTo(1.5f));
        }

        [Test]
        public void PassiveStatMultiplier_ConfiguresCorrectly()
        {
            var ability = Ability.Define("Huge Power Clone")
                .PassiveStatMultiplier(Stat.Attack, 2.0f)
                .Build();

            Assert.That(ability.Effect, Is.EqualTo(AbilityEffect.PassiveStatMultiplier));
            Assert.That(ability.TargetStat, Is.EqualTo(Stat.Attack));
            Assert.That(ability.Multiplier, Is.EqualTo(2.0f));
            Assert.That(ability.IsPassive, Is.True);
        }

        [Test]
        public void SummonsWeather_ConfiguresCorrectly()
        {
            var ability = Ability.Define("Drizzle Clone")
                .SummonsWeather(Weather.Rain)
                .Build();

            Assert.That(ability.Effect, Is.EqualTo(AbilityEffect.SummonWeather));
            Assert.That(ability.WeatherCondition, Is.EqualTo(Weather.Rain));
            Assert.That(ability.ListensTo(AbilityTrigger.OnSwitchIn), Is.True);
        }

        #endregion

        #region Catalog Tests

        [Test]
        public void Catalog_ContainsAbilities()
        {
            Assert.That(AbilityCatalog.All.Count, Is.GreaterThan(0));
        }

        [Test]
        public void Catalog_Intimidate_HasCorrectConfiguration()
        {
            var intimidate = AbilityCatalog.Intimidate;

            Assert.That(intimidate.Name, Is.EqualTo("Intimidate"));
            Assert.That(intimidate.Effect, Is.EqualTo(AbilityEffect.LowerOpponentStat));
            Assert.That(intimidate.TargetStat, Is.EqualTo(Stat.Attack));
            Assert.That(intimidate.StatStages, Is.EqualTo(-1));
            Assert.That(intimidate.ListensTo(AbilityTrigger.OnSwitchIn), Is.True);
        }

        [Test]
        public void Catalog_Static_HasCorrectConfiguration()
        {
            var staticAbility = AbilityCatalog.Static;

            Assert.That(staticAbility.Name, Is.EqualTo("Static"));
            Assert.That(staticAbility.Effect, Is.EqualTo(AbilityEffect.ChanceToStatusOnContact));
            Assert.That(staticAbility.StatusEffect, Is.EqualTo(PersistentStatus.Paralysis));
            Assert.That(staticAbility.EffectChance, Is.EqualTo(0.30f));
        }

        [Test]
        public void Catalog_Levitate_HasCorrectConfiguration()
        {
            var levitate = AbilityCatalog.Levitate;

            Assert.That(levitate.Name, Is.EqualTo("Levitate"));
            Assert.That(levitate.Effect, Is.EqualTo(AbilityEffect.GroundImmunity));
            Assert.That(levitate.AffectedType, Is.EqualTo(PokemonType.Ground));
            Assert.That(levitate.IsPassive, Is.True);
        }

        [Test]
        public void Catalog_Blaze_HasCorrectConfiguration()
        {
            var blaze = AbilityCatalog.Blaze;

            Assert.That(blaze.Name, Is.EqualTo("Blaze"));
            Assert.That(blaze.Effect, Is.EqualTo(AbilityEffect.TypePowerBoostWhenLowHP));
            Assert.That(blaze.AffectedType, Is.EqualTo(PokemonType.Fire));
        }

        [Test]
        public void Catalog_Sturdy_HasCorrectConfiguration()
        {
            var sturdy = AbilityCatalog.Sturdy;

            Assert.That(sturdy.Name, Is.EqualTo("Sturdy"));
            Assert.That(sturdy.Effect, Is.EqualTo(AbilityEffect.SurviveFatalHit));
            Assert.That(sturdy.HPThreshold, Is.EqualTo(1.0f));
            Assert.That(sturdy.ListensTo(AbilityTrigger.OnWouldFaint), Is.True);
        }

        [Test]
        public void Catalog_GetByName_ReturnsCorrectAbility()
        {
            var ability = AbilityCatalog.GetByName("Intimidate");

            Assert.That(ability, Is.Not.Null);
            Assert.That(ability.Name, Is.EqualTo("Intimidate"));
        }

        [Test]
        public void Catalog_GetByName_NotFound_ReturnsNull()
        {
            var ability = AbilityCatalog.GetByName("Nonexistent");

            Assert.That(ability, Is.Null);
        }

        [Test]
        public void Catalog_GetById_ReturnsCorrectAbility()
        {
            var ability = AbilityCatalog.GetById("intimidate");

            Assert.That(ability, Is.Not.Null);
            Assert.That(ability.Name, Is.EqualTo("Intimidate"));
        }

        #endregion

        #region ListensTo Tests

        [Test]
        public void ListensTo_MultipleTriggers_WorksCorrectly()
        {
            var ability = Ability.Define("Multi")
                .OnSwitchIn()
                .OnTurnEnd()
                .Build();

            Assert.That(ability.ListensTo(AbilityTrigger.OnSwitchIn), Is.True);
            Assert.That(ability.ListensTo(AbilityTrigger.OnTurnEnd), Is.True);
            Assert.That(ability.ListensTo(AbilityTrigger.OnDamageTaken), Is.False);
        }

        [Test]
        public void IsPassive_WhenPassiveTrigger_ReturnsTrue()
        {
            var ability = Ability.Define("Passive")
                .Passive()
                .Build();

            Assert.That(ability.IsPassive, Is.True);
        }

        [Test]
        public void IsPassive_WhenNotPassive_ReturnsFalse()
        {
            var ability = Ability.Define("Active")
                .OnSwitchIn()
                .Build();

            Assert.That(ability.IsPassive, Is.False);
        }

        #endregion
    }
}

