using NUnit.Framework;
using PokemonUltimate.Content.Catalogs.Weather;
using PokemonUltimate.Core.Blueprints;
using PokemonUltimate.Content.Builders;
using PokemonUltimate.Core.Enums;

namespace PokemonUltimate.Tests.Blueprints
{
    /// <summary>
    /// Tests for WeatherData and WeatherBuilder.
    /// </summary>
    [TestFixture]
    public class WeatherDataTests
    {
        #region Builder Tests

        [Test]
        public void Define_WithName_SetsNameAndId()
        {
            var weather = WeatherEffect.Define("Test Weather")
                .Build();

            Assert.That(weather.Name, Is.EqualTo("Test Weather"));
            Assert.That(weather.Id, Is.EqualTo("test-weather"));
        }

        [Test]
        public void Type_SetsWeatherEnum()
        {
            var weather = WeatherEffect.Define("Rain")
                .Type(Weather.Rain)
                .Build();

            Assert.That(weather.Weather, Is.EqualTo(Weather.Rain));
        }

        [Test]
        public void Duration_SetsDefaultDuration()
        {
            var weather = WeatherEffect.Define("Test")
                .Duration(5)
                .Build();

            Assert.That(weather.DefaultDuration, Is.EqualTo(5));
        }

        [Test]
        public void Indefinite_SetsZeroDuration()
        {
            var weather = WeatherEffect.Define("Test")
                .Indefinite()
                .Build();

            Assert.That(weather.DefaultDuration, Is.EqualTo(0));
        }

        [Test]
        public void Primal_SetsCannotBeOverwritten()
        {
            var weather = WeatherEffect.Define("Test")
                .Primal()
                .Build();

            Assert.That(weather.CanBeOverwritten, Is.False);
            Assert.That(weather.IsPrimal, Is.True);
        }

        [Test]
        public void DealsDamagePerTurn_SetsDamageAndImmunities()
        {
            var weather = WeatherEffect.Define("Test")
                .DealsDamagePerTurn(0.0625f, PokemonType.Rock, PokemonType.Ground)
                .Build();

            Assert.That(weather.EndOfTurnDamage, Is.EqualTo(0.0625f).Within(0.0001f));
            Assert.That(weather.DealsDamage, Is.True);
            Assert.That(weather.DamageImmuneTypes, Contains.Item(PokemonType.Rock));
            Assert.That(weather.DamageImmuneTypes, Contains.Item(PokemonType.Ground));
        }

        [Test]
        public void Boosts_AddsBoostedTypes()
        {
            var weather = WeatherEffect.Define("Test")
                .Boosts(PokemonType.Water)
                .Build();

            Assert.That(weather.BoostedTypes, Contains.Item(PokemonType.Water));
            Assert.That(weather.BoostMultiplier, Is.EqualTo(1.5f));
        }

        [Test]
        public void Weakens_AddsWeakenedTypes()
        {
            var weather = WeatherEffect.Define("Test")
                .Weakens(PokemonType.Fire)
                .Build();

            Assert.That(weather.WeakenedTypes, Contains.Item(PokemonType.Fire));
            Assert.That(weather.WeakenMultiplier, Is.EqualTo(0.5f));
        }

        [Test]
        public void Nullifies_AddsNullifiedTypes()
        {
            var weather = WeatherEffect.Define("Test")
                .Nullifies(PokemonType.Fire)
                .Build();

            Assert.That(weather.NullifiedTypes, Contains.Item(PokemonType.Fire));
        }

        [Test]
        public void BoostsStat_SetsStatBoost()
        {
            var weather = WeatherEffect.Define("Test")
                .BoostsStat(Stat.SpDefense, 1.5f, PokemonType.Rock)
                .Build();

            Assert.That(weather.BoostedStat, Is.EqualTo(Stat.SpDefense));
            Assert.That(weather.StatBoostMultiplier, Is.EqualTo(1.5f));
            Assert.That(weather.StatBoostTypes, Contains.Item(PokemonType.Rock));
        }

        [Test]
        public void PerfectAccuracy_AddsMoves()
        {
            var weather = WeatherEffect.Define("Test")
                .PerfectAccuracy("Thunder", "Hurricane")
                .Build();

            Assert.That(weather.PerfectAccuracyMoves, Contains.Item("Thunder"));
            Assert.That(weather.PerfectAccuracyMoves, Contains.Item("Hurricane"));
        }

        [Test]
        public void InstantCharge_AddsMoves()
        {
            var weather = WeatherEffect.Define("Test")
                .InstantCharge("Solar Beam")
                .Build();

            Assert.That(weather.InstantChargeMoves, Contains.Item("Solar Beam"));
        }

        #endregion

        #region Helper Method Tests

        [Test]
        public void IsTypeImmuneToDamage_ImmuneType_ReturnsTrue()
        {
            var weather = WeatherEffect.Define("Test")
                .DealsDamagePerTurn(0.0625f, PokemonType.Rock)
                .Build();

            Assert.That(weather.IsTypeImmuneToDamage(PokemonType.Rock), Is.True);
            Assert.That(weather.IsTypeImmuneToDamage(PokemonType.Fire), Is.False);
        }

        [Test]
        public void IsTypeBoosted_BoostedType_ReturnsTrue()
        {
            var weather = WeatherEffect.Define("Test")
                .Boosts(PokemonType.Water)
                .Build();

            Assert.That(weather.IsTypeBoosted(PokemonType.Water), Is.True);
            Assert.That(weather.IsTypeBoosted(PokemonType.Fire), Is.False);
        }

        [Test]
        public void IsTypeWeakened_WeakenedType_ReturnsTrue()
        {
            var weather = WeatherEffect.Define("Test")
                .Weakens(PokemonType.Fire)
                .Build();

            Assert.That(weather.IsTypeWeakened(PokemonType.Fire), Is.True);
            Assert.That(weather.IsTypeWeakened(PokemonType.Water), Is.False);
        }

        [Test]
        public void IsTypeNullified_NullifiedType_ReturnsTrue()
        {
            var weather = WeatherEffect.Define("Test")
                .Nullifies(PokemonType.Fire)
                .Build();

            Assert.That(weather.IsTypeNullified(PokemonType.Fire), Is.True);
            Assert.That(weather.IsTypeNullified(PokemonType.Water), Is.False);
        }

        [Test]
        public void GetTypePowerMultiplier_BoostedType_ReturnsBoostMultiplier()
        {
            var weather = WeatherEffect.Define("Test")
                .Boosts(PokemonType.Water)
                .Build();

            Assert.That(weather.GetTypePowerMultiplier(PokemonType.Water), Is.EqualTo(1.5f));
        }

        [Test]
        public void GetTypePowerMultiplier_WeakenedType_ReturnsWeakenMultiplier()
        {
            var weather = WeatherEffect.Define("Test")
                .Weakens(PokemonType.Fire)
                .Build();

            Assert.That(weather.GetTypePowerMultiplier(PokemonType.Fire), Is.EqualTo(0.5f));
        }

        [Test]
        public void GetTypePowerMultiplier_NullifiedType_ReturnsZero()
        {
            var weather = WeatherEffect.Define("Test")
                .Nullifies(PokemonType.Fire)
                .Build();

            Assert.That(weather.GetTypePowerMultiplier(PokemonType.Fire), Is.EqualTo(0f));
        }

        [Test]
        public void GetTypePowerMultiplier_NormalType_ReturnsOne()
        {
            var weather = WeatherEffect.Define("Test")
                .Boosts(PokemonType.Water)
                .Build();

            Assert.That(weather.GetTypePowerMultiplier(PokemonType.Normal), Is.EqualTo(1f));
        }

        [Test]
        public void TypeGetsStatBoost_CorrectType_ReturnsTrue()
        {
            var weather = WeatherEffect.Define("Test")
                .BoostsStat(Stat.SpDefense, 1.5f, PokemonType.Rock)
                .Build();

            Assert.That(weather.TypeGetsStatBoost(PokemonType.Rock), Is.True);
            Assert.That(weather.TypeGetsStatBoost(PokemonType.Fire), Is.False);
        }

        [Test]
        public void HasPerfectAccuracy_CorrectMove_ReturnsTrue()
        {
            var weather = WeatherEffect.Define("Test")
                .PerfectAccuracy("Thunder")
                .Build();

            Assert.That(weather.HasPerfectAccuracy("Thunder"), Is.True);
            Assert.That(weather.HasPerfectAccuracy("thunder"), Is.True); // Case insensitive
            Assert.That(weather.HasPerfectAccuracy("Thunderbolt"), Is.False);
        }

        [Test]
        public void ChargesInstantly_CorrectMove_ReturnsTrue()
        {
            var weather = WeatherEffect.Define("Test")
                .InstantCharge("Solar Beam")
                .Build();

            Assert.That(weather.ChargesInstantly("Solar Beam"), Is.True);
            Assert.That(weather.ChargesInstantly("solar beam"), Is.True);
            Assert.That(weather.ChargesInstantly("Hyper Beam"), Is.False);
        }

        #endregion

        #region Catalog Tests

        [Test]
        public void Catalog_Rain_HasCorrectProperties()
        {
            var rain = WeatherCatalog.Rain;

            Assert.That(rain.Weather, Is.EqualTo(Weather.Rain));
            Assert.That(rain.DefaultDuration, Is.EqualTo(5));
            Assert.That(rain.CanBeOverwritten, Is.True);
            Assert.That(rain.IsTypeBoosted(PokemonType.Water), Is.True);
            Assert.That(rain.IsTypeWeakened(PokemonType.Fire), Is.True);
            Assert.That(rain.HasPerfectAccuracy("Thunder"), Is.True);
        }

        [Test]
        public void Catalog_Sun_HasCorrectProperties()
        {
            var sun = WeatherCatalog.Sun;

            Assert.That(sun.Weather, Is.EqualTo(Weather.Sun));
            Assert.That(sun.IsTypeBoosted(PokemonType.Fire), Is.True);
            Assert.That(sun.IsTypeWeakened(PokemonType.Water), Is.True);
            Assert.That(sun.ChargesInstantly("Solar Beam"), Is.True);
        }

        [Test]
        public void Catalog_Sandstorm_HasCorrectProperties()
        {
            var sandstorm = WeatherCatalog.Sandstorm;

            Assert.That(sandstorm.Weather, Is.EqualTo(Weather.Sandstorm));
            Assert.That(sandstorm.DealsDamage, Is.True);
            Assert.That(sandstorm.EndOfTurnDamage, Is.EqualTo(1f / 16f).Within(0.001f));
            Assert.That(sandstorm.IsTypeImmuneToDamage(PokemonType.Rock), Is.True);
            Assert.That(sandstorm.IsTypeImmuneToDamage(PokemonType.Ground), Is.True);
            Assert.That(sandstorm.IsTypeImmuneToDamage(PokemonType.Steel), Is.True);
            Assert.That(sandstorm.BoostedStat, Is.EqualTo(Stat.SpDefense));
            Assert.That(sandstorm.TypeGetsStatBoost(PokemonType.Rock), Is.True);
        }

        [Test]
        public void Catalog_Hail_HasCorrectProperties()
        {
            var hail = WeatherCatalog.Hail;

            Assert.That(hail.Weather, Is.EqualTo(Weather.Hail));
            Assert.That(hail.DealsDamage, Is.True);
            Assert.That(hail.IsTypeImmuneToDamage(PokemonType.Ice), Is.True);
            Assert.That(hail.HasPerfectAccuracy("Blizzard"), Is.True);
        }

        [Test]
        public void Catalog_Snow_HasCorrectProperties()
        {
            var snow = WeatherCatalog.Snow;

            Assert.That(snow.Weather, Is.EqualTo(Weather.Snow));
            Assert.That(snow.DealsDamage, Is.False); // Snow doesn't damage
            Assert.That(snow.BoostedStat, Is.EqualTo(Stat.Defense));
            Assert.That(snow.TypeGetsStatBoost(PokemonType.Ice), Is.True);
        }

        [Test]
        public void Catalog_HeavyRain_IsPrimal()
        {
            var heavyRain = WeatherCatalog.HeavyRain;

            Assert.That(heavyRain.Weather, Is.EqualTo(Weather.HeavyRain));
            Assert.That(heavyRain.IsPrimal, Is.True);
            Assert.That(heavyRain.CanBeOverwritten, Is.False);
            Assert.That(heavyRain.IsTypeNullified(PokemonType.Fire), Is.True);
            Assert.That(heavyRain.GetTypePowerMultiplier(PokemonType.Fire), Is.EqualTo(0f));
        }

        [Test]
        public void Catalog_ExtremelyHarshSunlight_IsPrimal()
        {
            var extremeSun = WeatherCatalog.ExtremelyHarshSunlight;

            Assert.That(extremeSun.IsPrimal, Is.True);
            Assert.That(extremeSun.IsTypeNullified(PokemonType.Water), Is.True);
            Assert.That(extremeSun.GetTypePowerMultiplier(PokemonType.Water), Is.EqualTo(0f));
        }

        [Test]
        public void Catalog_GetByWeather_ReturnsCorrectData()
        {
            Assert.That(WeatherCatalog.GetByWeather(Weather.Rain), Is.EqualTo(WeatherCatalog.Rain));
            Assert.That(WeatherCatalog.GetByWeather(Weather.Sun), Is.EqualTo(WeatherCatalog.Sun));
            Assert.That(WeatherCatalog.GetByWeather(Weather.None), Is.Null);
        }

        [Test]
        public void Catalog_GetDamagingWeather_ReturnsOnlyDamagingWeather()
        {
            var damaging = new System.Collections.Generic.List<WeatherData>(WeatherCatalog.GetDamagingWeather());

            Assert.That(damaging, Contains.Item(WeatherCatalog.Sandstorm));
            Assert.That(damaging, Contains.Item(WeatherCatalog.Hail));
            Assert.That(damaging, Does.Not.Contain(WeatherCatalog.Rain));
            Assert.That(damaging, Does.Not.Contain(WeatherCatalog.Snow));
        }

        [Test]
        public void Catalog_GetPrimalWeather_ReturnsOnlyPrimalWeather()
        {
            var primal = new System.Collections.Generic.List<WeatherData>(WeatherCatalog.GetPrimalWeather());

            Assert.That(primal, Contains.Item(WeatherCatalog.HeavyRain));
            Assert.That(primal, Contains.Item(WeatherCatalog.ExtremelyHarshSunlight));
            Assert.That(primal, Contains.Item(WeatherCatalog.StrongWinds));
            Assert.That(primal, Does.Not.Contain(WeatherCatalog.Rain));
        }

        [Test]
        public void Catalog_All_Contains9Weathers()
        {
            Assert.That(WeatherCatalog.All.Count, Is.EqualTo(9));
        }

        #endregion
    }
}

