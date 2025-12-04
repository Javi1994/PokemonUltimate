using NUnit.Framework;
using PokemonUltimate.Content.Catalogs.Weather;
using PokemonUltimate.Content.Builders;
using PokemonUltimate.Core.Enums;

namespace PokemonUltimate.Tests.Blueprints
{
    /// <summary>
    /// Edge case tests for WeatherData.
    /// </summary>
    [TestFixture]
    public class WeatherDataEdgeCasesTests
    {
        #region Multiplier Priority Tests

        [Test]
        public void GetTypePowerMultiplier_NullifiedTakesPriority_ReturnsZero()
        {
            // If a type is both boosted and nullified, nullified should take priority
            var weather = WeatherEffect.Define("Test")
                .Boosts(PokemonType.Fire)
                .Nullifies(PokemonType.Fire)
                .Build();

            Assert.That(weather.GetTypePowerMultiplier(PokemonType.Fire), Is.EqualTo(0f));
        }

        [Test]
        public void GetTypePowerMultiplier_BoostedOverWeakened_ReturnsBoost()
        {
            // If a type is both boosted and weakened, boosted should take priority
            var weather = WeatherEffect.Define("Test")
                .Boosts(PokemonType.Water)
                .Weakens(PokemonType.Water)
                .Build();

            // Boosted is checked first in our implementation
            Assert.That(weather.GetTypePowerMultiplier(PokemonType.Water), Is.EqualTo(1.5f));
        }

        #endregion

        #region Empty Collections Tests

        [Test]
        public void Default_NoDamage_DealsDamageIsFalse()
        {
            var weather = WeatherEffect.Define("Test").Build();

            Assert.That(weather.DealsDamage, Is.False);
            Assert.That(weather.EndOfTurnDamage, Is.EqualTo(0f));
        }

        [Test]
        public void Default_NoStatBoost_TypeGetsStatBoostReturnsFalse()
        {
            var weather = WeatherEffect.Define("Test").Build();

            Assert.That(weather.BoostedStat, Is.Null);
            Assert.That(weather.TypeGetsStatBoost(PokemonType.Rock), Is.False);
        }

        [Test]
        public void Default_NoBoostedTypes_IsTypeBoostedReturnsFalse()
        {
            var weather = WeatherEffect.Define("Test").Build();

            Assert.That(weather.IsTypeBoosted(PokemonType.Water), Is.False);
        }

        [Test]
        public void Default_NoMoves_HasPerfectAccuracyReturnsFalse()
        {
            var weather = WeatherEffect.Define("Test").Build();

            Assert.That(weather.HasPerfectAccuracy("Thunder"), Is.False);
            Assert.That(weather.ChargesInstantly("Solar Beam"), Is.False);
        }

        #endregion

        #region Custom Multiplier Tests

        [Test]
        public void CustomBoostMultiplier_AppliesCorrectly()
        {
            var weather = WeatherEffect.Define("Test")
                .Boosts(PokemonType.Water)
                .BoostMultiplier(2.0f)
                .Build();

            Assert.That(weather.BoostMultiplier, Is.EqualTo(2.0f));
        }

        [Test]
        public void CustomWeakenMultiplier_AppliesCorrectly()
        {
            var weather = WeatherEffect.Define("Test")
                .Weakens(PokemonType.Fire)
                .WeakenMultiplier(0.25f)
                .Build();

            Assert.That(weather.WeakenMultiplier, Is.EqualTo(0.25f));
        }

        #endregion

        #region Duration Tests

        [Test]
        public void StandardWeather_CanBeOverwritten()
        {
            var weather = WeatherEffect.Define("Test")
                .Duration(5)
                .Build();

            Assert.That(weather.CanBeOverwritten, Is.True);
            Assert.That(weather.IsPrimal, Is.False);
        }

        [Test]
        public void PrimalWeather_CannotBeOverwritten()
        {
            var weather = WeatherEffect.Define("Test")
                .Primal()
                .Build();

            Assert.That(weather.CanBeOverwritten, Is.False);
            Assert.That(weather.IsPrimal, Is.True);
        }

        [Test]
        public void DefaultDuration_Is5Turns()
        {
            var weather = WeatherEffect.Define("Test").Build();

            Assert.That(weather.DefaultDuration, Is.EqualTo(5));
        }

        #endregion

        #region Multiple Types Tests

        [Test]
        public void MultipleImmuneTypes_AllAreImmune()
        {
            var weather = WeatherEffect.Define("Test")
                .DealsDamagePerTurn(0.0625f, PokemonType.Rock, PokemonType.Ground, PokemonType.Steel)
                .Build();

            Assert.That(weather.IsTypeImmuneToDamage(PokemonType.Rock), Is.True);
            Assert.That(weather.IsTypeImmuneToDamage(PokemonType.Ground), Is.True);
            Assert.That(weather.IsTypeImmuneToDamage(PokemonType.Steel), Is.True);
            Assert.That(weather.IsTypeImmuneToDamage(PokemonType.Fire), Is.False);
        }

        [Test]
        public void MultipleBoostedTypes_AllAreBoosted()
        {
            var weather = WeatherEffect.Define("Test")
                .Boosts(PokemonType.Water, PokemonType.Electric)
                .Build();

            Assert.That(weather.IsTypeBoosted(PokemonType.Water), Is.True);
            Assert.That(weather.IsTypeBoosted(PokemonType.Electric), Is.True);
        }

        [Test]
        public void MultipleStatBoostTypes_AllGetBoost()
        {
            var weather = WeatherEffect.Define("Test")
                .BoostsStat(Stat.Defense, 1.5f, PokemonType.Ice, PokemonType.Water)
                .Build();

            Assert.That(weather.TypeGetsStatBoost(PokemonType.Ice), Is.True);
            Assert.That(weather.TypeGetsStatBoost(PokemonType.Water), Is.True);
            Assert.That(weather.TypeGetsStatBoost(PokemonType.Fire), Is.False);
        }

        #endregion

        #region Ability Lists Tests

        [Test]
        public void AbilityLists_AreStoredCorrectly()
        {
            var weather = WeatherEffect.Define("Test")
                .AbilitiesImmuneToDamage("Overcoat", "Magic Guard")
                .AbilitiesDoubleSpeed("Swift Swim")
                .AbilitiesHeal("Rain Dish")
                .Build();

            Assert.That(weather.DamageImmunityAbilities, Contains.Item("Overcoat"));
            Assert.That(weather.DamageImmunityAbilities, Contains.Item("Magic Guard"));
            Assert.That(weather.SpeedBoostAbilities, Contains.Item("Swift Swim"));
            Assert.That(weather.HealingAbilities, Contains.Item("Rain Dish"));
        }

        #endregion

        #region Case Sensitivity Tests

        [Test]
        public void HasPerfectAccuracy_IsCaseInsensitive()
        {
            var weather = WeatherEffect.Define("Test")
                .PerfectAccuracy("Thunder")
                .Build();

            Assert.That(weather.HasPerfectAccuracy("THUNDER"), Is.True);
            Assert.That(weather.HasPerfectAccuracy("thunder"), Is.True);
            Assert.That(weather.HasPerfectAccuracy("Thunder"), Is.True);
        }

        [Test]
        public void ChargesInstantly_IsCaseInsensitive()
        {
            var weather = WeatherEffect.Define("Test")
                .InstantCharge("Solar Beam")
                .Build();

            Assert.That(weather.ChargesInstantly("SOLAR BEAM"), Is.True);
            Assert.That(weather.ChargesInstantly("solar beam"), Is.True);
            Assert.That(weather.ChargesInstantly("Solar Beam"), Is.True);
        }

        #endregion

        #region Catalog Lookup Edge Cases

        [Test]
        public void GetByWeather_None_ReturnsNull()
        {
            Assert.That(WeatherCatalog.GetByWeather(Weather.None), Is.Null);
        }

        [Test]
        public void GetByName_NonExistent_ReturnsNull()
        {
            Assert.That(WeatherCatalog.GetByName("Nonexistent Weather"), Is.Null);
        }

        [Test]
        public void GetByName_CorrectName_ReturnsWeather()
        {
            Assert.That(WeatherCatalog.GetByName("Rain"), Is.EqualTo(WeatherCatalog.Rain));
            Assert.That(WeatherCatalog.GetByName("Harsh Sunlight"), Is.EqualTo(WeatherCatalog.Sun));
        }

        #endregion

        #region Real Game Scenario Tests

        [Test]
        public void Rain_WaterMoveInRain_Gets1_5xPower()
        {
            var rain = WeatherCatalog.Rain;
            float waterMultiplier = rain.GetTypePowerMultiplier(PokemonType.Water);
            float fireMultiplier = rain.GetTypePowerMultiplier(PokemonType.Fire);

            Assert.That(waterMultiplier, Is.EqualTo(1.5f));
            Assert.That(fireMultiplier, Is.EqualTo(0.5f));
        }

        [Test]
        public void HeavyRain_FireMoveIsNullified()
        {
            var heavyRain = WeatherCatalog.HeavyRain;
            float fireMultiplier = heavyRain.GetTypePowerMultiplier(PokemonType.Fire);

            Assert.That(fireMultiplier, Is.EqualTo(0f));
        }

        [Test]
        public void Sandstorm_NonRockGroundSteel_TakesDamage()
        {
            var sandstorm = WeatherCatalog.Sandstorm;

            // Fire type should take damage
            Assert.That(sandstorm.IsTypeImmuneToDamage(PokemonType.Fire), Is.False);
            Assert.That(sandstorm.DealsDamage, Is.True);

            // Rock/Ground/Steel are immune
            Assert.That(sandstorm.IsTypeImmuneToDamage(PokemonType.Rock), Is.True);
            Assert.That(sandstorm.IsTypeImmuneToDamage(PokemonType.Ground), Is.True);
            Assert.That(sandstorm.IsTypeImmuneToDamage(PokemonType.Steel), Is.True);
        }

        [Test]
        public void Sandstorm_RockGround_DualType_IsImmune()
        {
            var sandstorm = WeatherCatalog.Sandstorm;

            // Rock/Ground dual type (Geodude, Graveler, Golem) should be immune
            // Both types are immune individually, so dual type Pokemon are immune
            Assert.That(sandstorm.IsTypeImmuneToDamage(PokemonType.Rock), Is.True, "Rock type should be immune to Sandstorm");
            Assert.That(sandstorm.IsTypeImmuneToDamage(PokemonType.Ground), Is.True, "Ground type should be immune to Sandstorm");
            // Note: Dual type immunity is handled by checking if either type is immune
        }

        [Test]
        public void Sandstorm_RockTypes_Get1_5xSpDef()
        {
            var sandstorm = WeatherCatalog.Sandstorm;

            Assert.That(sandstorm.BoostedStat, Is.EqualTo(Stat.SpDefense));
            Assert.That(sandstorm.StatBoostMultiplier, Is.EqualTo(1.5f));
            Assert.That(sandstorm.TypeGetsStatBoost(PokemonType.Rock), Is.True);
        }

        #endregion
    }
}

