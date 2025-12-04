using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using PokemonUltimate.Combat;
using PokemonUltimate.Combat.Actions;
using PokemonUltimate.Core.Enums;
using PokemonUltimate.Core.Factories;
using PokemonUltimate.Core.Instances;
using PokemonUltimate.Content.Catalogs.Pokemon;
using PokemonUltimate.Content.Catalogs.Weather;

namespace PokemonUltimate.Tests.Systems.Combat.Actions
{
    /// <summary>
    /// Tests for SetWeatherAction - actions that change battlefield weather.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.12: Weather System
    /// **Documentation**: See `docs/features/2-combat-system/2.12-weather-system/README.md`
    /// </remarks>
    [TestFixture]
    public class SetWeatherActionTests
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

        #region Set Weather Tests

        [Test]
        public void ExecuteLogic_Sun_SetsWeather()
        {
            var action = new SetWeatherAction(_userSlot, Weather.Sun, 5, WeatherCatalog.Sun);

            action.ExecuteLogic(_field);

            Assert.That(_field.Weather, Is.EqualTo(Weather.Sun));
            Assert.That(_field.WeatherDuration, Is.EqualTo(5));
            Assert.That(_field.WeatherData, Is.EqualTo(WeatherCatalog.Sun));
        }

        [Test]
        public void ExecuteLogic_Rain_SetsWeather()
        {
            var action = new SetWeatherAction(_userSlot, Weather.Rain, 5, WeatherCatalog.Rain);

            action.ExecuteLogic(_field);

            Assert.That(_field.Weather, Is.EqualTo(Weather.Rain));
            Assert.That(_field.WeatherDuration, Is.EqualTo(5));
            Assert.That(_field.WeatherData, Is.EqualTo(WeatherCatalog.Rain));
        }

        [Test]
        public void ExecuteLogic_Sandstorm_SetsWeather()
        {
            var action = new SetWeatherAction(_userSlot, Weather.Sandstorm, 5, WeatherCatalog.Sandstorm);

            action.ExecuteLogic(_field);

            Assert.That(_field.Weather, Is.EqualTo(Weather.Sandstorm));
            Assert.That(_field.WeatherDuration, Is.EqualTo(5));
            Assert.That(_field.WeatherData, Is.EqualTo(WeatherCatalog.Sandstorm));
        }

        [Test]
        public void ExecuteLogic_Hail_SetsWeather()
        {
            var action = new SetWeatherAction(_userSlot, Weather.Hail, 5, WeatherCatalog.Hail);

            action.ExecuteLogic(_field);

            Assert.That(_field.Weather, Is.EqualTo(Weather.Hail));
            Assert.That(_field.WeatherDuration, Is.EqualTo(5));
            Assert.That(_field.WeatherData, Is.EqualTo(WeatherCatalog.Hail));
        }

        [Test]
        public void ExecuteLogic_ClearWeather_ClearsWeather()
        {
            // Set weather first
            _field.SetWeather(Weather.Sun, 5, WeatherCatalog.Sun);
            var action = new SetWeatherAction(_userSlot, Weather.None, 0);

            action.ExecuteLogic(_field);

            Assert.That(_field.Weather, Is.EqualTo(Weather.None));
            Assert.That(_field.WeatherDuration, Is.EqualTo(0));
            Assert.That(_field.WeatherData, Is.Null);
        }

        #endregion

        #region Primal Weather Tests

        [Test]
        public void ExecuteLogic_PrimalWeather_CannotBeOverwrittenByNormal()
        {
            // Set primal weather first
            _field.SetWeather(Weather.ExtremelyHarshSunlight, 0, WeatherCatalog.ExtremelyHarshSunlight);
            
            // Try to overwrite with normal weather
            var action = new SetWeatherAction(_userSlot, Weather.Rain, 5, WeatherCatalog.Rain);
            action.ExecuteLogic(_field);

            // Primal weather should remain
            Assert.That(_field.Weather, Is.EqualTo(Weather.ExtremelyHarshSunlight));
            Assert.That(_field.WeatherData, Is.EqualTo(WeatherCatalog.ExtremelyHarshSunlight));
        }

        [Test]
        public void ExecuteLogic_PrimalWeather_CanOverwriteNormal()
        {
            // Set normal weather first
            _field.SetWeather(Weather.Sun, 5, WeatherCatalog.Sun);
            
            // Primal weather can overwrite normal
            var action = new SetWeatherAction(_userSlot, Weather.ExtremelyHarshSunlight, 0, WeatherCatalog.ExtremelyHarshSunlight);
            action.ExecuteLogic(_field);

            Assert.That(_field.Weather, Is.EqualTo(Weather.ExtremelyHarshSunlight));
            Assert.That(_field.WeatherData, Is.EqualTo(WeatherCatalog.ExtremelyHarshSunlight));
        }

        [Test]
        public void ExecuteLogic_PrimalWeather_CanOverwriteAnotherPrimal()
        {
            // Set one primal weather
            _field.SetWeather(Weather.ExtremelyHarshSunlight, 0, WeatherCatalog.ExtremelyHarshSunlight);
            
            // Another primal can overwrite
            var action = new SetWeatherAction(_userSlot, Weather.HeavyRain, 0, WeatherCatalog.HeavyRain);
            action.ExecuteLogic(_field);

            Assert.That(_field.Weather, Is.EqualTo(Weather.HeavyRain));
            Assert.That(_field.WeatherData, Is.EqualTo(WeatherCatalog.HeavyRain));
        }

        #endregion

        #region Return Actions Tests

        [Test]
        public void ExecuteLogic_ReturnsEmptyEnumerable()
        {
            var action = new SetWeatherAction(_userSlot, Weather.Sun, 5, WeatherCatalog.Sun);

            var result = action.ExecuteLogic(_field);

            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.Empty);
        }

        #endregion
    }
}

